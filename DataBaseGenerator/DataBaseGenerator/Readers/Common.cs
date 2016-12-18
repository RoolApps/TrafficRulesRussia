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
            if (node.NodeName == NodeTypes.Text && (node.NodeValue ?? String.Empty) != String.Empty)
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
                    var content = String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                    result = String.Format(pattern, content);
                }
                else
                {
                    result = String.Format(pattern, node.NodeValue);
                }
            }
            if (node.NodeName == NodeTypes.Bold)
            {
                var pattern = "<Bold>{0}</Bold>";
                if (node.ChildNodes.Any())
                {
                    var content = String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                    result = String.Format(pattern, content);
                }
                else
                {
                    result = String.Format(pattern, node.NodeValue);
                }
            }
            if (node.NodeName == NodeTypes.Font)
            {
                if (node.ChildNodes.Any())
                {
                    result = String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode, level + 1)));
                }
            }
            if (node.NodeName == NodeTypes.Link)
            {
                var ruleObjects = new string[] { "signs", "marks" };
                if (ruleObjects.Any(ruleObject => (node.GetAttribute("href") ?? String.Empty).Contains(ruleObject)))
                {
                    result = String.Format("<Run>{0}</Run>", ReplaceUrl(node.ChildNodes.Single(chileNode => chileNode.NodeName == NodeTypes.Text).NodeValue,
                        ruleObjects.Single(ruleObject => node.GetAttribute("href").Contains(ruleObject))));
                }
            }
            if (level == 0 && result != String.Empty && !(new string[] { NodeTypes.Paragraph, NodeTypes.Header }).Contains(node.NodeName))
            {
                result = String.Format(@"<Paragraph xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">{0}</Paragraph>", result);
            }
            return result;
        }

        static string ReplaceUrl(string text, string replace)
        {
            text = text.Replace('_', '.');
            return regex.Replace(text, String.Format("@@{{{0}}}${{num}}@@", replace));
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
        public const String Header = "H4";
        public const String Paragraph = "P";
        public const String Link = "A";
        public const String Bold = "STRONG";
        public const String Text = "#text";
        public const String Td = "TD";
        public const String Font = "FONT";
    }
}
