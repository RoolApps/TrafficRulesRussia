using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseGenerator.Readers
{
    public static class Common
    {
        public static event EventHandler<HeaderEventArgs> HeaderAppeared;
        static Regex regex = new Regex(@"(?<num>\d+(\.\d+)*)");

        public static void Log(String text)
        {
            Console.WriteLine(String.Format("{0}: {1}", DateTime.Now, text));
        }

        public static String ParseNode(CsQuery.IDomObject node, int level = 0)
        {
            var result = String.Empty;
            if (node.NodeName == NodeTypes.Text && (node.NodeValue ?? String.Empty).Trim() != String.Empty)
            {
                result = String.Format("<Run>{0}</Run>", node.NodeValue);
            }
            if (node.NodeName == NodeTypes.Header)
            {
                if (HeaderAppeared != null)
                {
                    HeaderAppeared(null, new HeaderEventArgs(node));
                }
            }
            if (node.NodeName == NodeTypes.Paragraph)
            {
                var pattern = @"<Paragraph xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">{0}</Paragraph>";
                if (node.ChildNodes.Any())
                {
                    var content = String.Join(GetIdent(level + 1), node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                    result = String.Format(pattern, content);
                }
                else
                {
                    result = String.Format(pattern, node.NodeValue);
                }
            }
            if (new String[] { NodeTypes.Bold, NodeTypes.Italic }.Contains(node.NodeName))
            {
                var pattern = PatternFactory.Get(node.NodeName);
                if (node.ChildNodes.Any())
                {
                    var content = String.Join(GetIdent(level + 1), node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                    result = String.Format(pattern, content);
                }
                else
                {
                    result = String.Format(pattern, node.NodeValue);
                }
            }
            if (new String[] { NodeTypes.Font, NodeTypes.Span }.Contains(node.NodeName))
            {
                if(node.Classes.Contains("pdd-inline-sign"))
                {
                    var match = new Regex(@"https://media.am.ru/pdd/(?<type>\w+)/xs/(?<num>[\w\.]+).png").Match(node.InnerHTML);
                    if(match != null)
                    {
                        var type = match.Groups["type"].Value;
                        var num = match.Groups["num"].Value;
                        if(type == "marking")
                        {
                            type = "mark";
                        }
                        return String.Format("<Run> @@{{{0}s}}{1}@@ </Run>", match.Groups["type"].Value, match.Groups["num"].Value);
                    }
                }
                else if (node.ChildNodes.Any())
                {
                    result = String.Join(GetIdent(level + 1), node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                }
            }
            if(node.NodeName == NodeTypes.List)
            {
                result = String.Join(Environment.NewLine, node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
            }
            if(node.NodeName == NodeTypes.ListItem)
            {
                result = String.Join(GetIdent(level + 1), node.ChildNodes.Select(childNode => "<Run>-</Run>" + ParseNode(childNode, level + 1)));
            }
            if (level == 0 && result != String.Empty && !(new string[] { NodeTypes.Paragraph, NodeTypes.Header }).Contains(node.NodeName))
            {
                result = String.Format(@"<Paragraph xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">{0}</Paragraph>", result);
            }
            return result;
        }

        static string GetIdent(int level)
        {
            return Environment.NewLine + new String(Enumerable.Range(0, level * 2).Select(i => ' ').ToArray());
        }

        static string ReplaceUrl(string text, string replace)
        {
            text = text.Replace('_', '.');
            return regex.Replace(text, String.Format("@@{{{0}}}${{num}}@@", replace));
        }

        public static String WrapText(String text)
        {
            return String.Format(@"<Paragraph xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">{0}</Paragraph>", text);
        }
    }

    public class HeaderEventArgs : EventArgs
    {
        public CsQuery.IDomObject Node { get; private set; }

        public HeaderEventArgs(CsQuery.IDomObject node)
        {
            this.Node = node;
        }
    }

    public static class NodeTypes
    {
        public const String Header = "H2";
        public const String Paragraph = "P";
        public const String Link = "A";
        public const String Bold = "STRONG";
        public const String Text = "#text";
        public const String Td = "TD";
        public const String Font = "FONT";
        public const String Span = "SPAN";
        public const String Italic = "EM";
        public const String List = "UL";
        public const String ListItem = "LI";
    }

    public static class PatternFactory
    {
        private static Dictionary<String, String> values = new Dictionary<String, String>()
        {
            { NodeTypes.Bold, "<Bold>{0}</Bold>" },
            { NodeTypes.Italic, "<Italic>{0}</Italic>"}
        };

        public static String Get(String nodeType)
        {
            if(values.ContainsKey(nodeType))
            {
                return values[nodeType];
            }
            else
            {
                return "{0}";
            }
        }
    }
}
