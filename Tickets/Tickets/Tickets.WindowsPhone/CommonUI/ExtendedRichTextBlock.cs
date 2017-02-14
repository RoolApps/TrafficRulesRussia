using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Tickets.CommonUI
{
    public sealed class ExtendedRichTextBlock : ContentControl
    {
        RichTextBlock richTextBlock = null;
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                        typeof(String),
                        typeof(ExtendedRichTextBlock),
                        new PropertyMetadata(0, TextChangedCallback));

        public ExtendedRichTextBlock()
        {
            richTextBlock = new RichTextBlock();
            richTextBlock.IsTextSelectionEnabled = false;
            this.Content = richTextBlock;
        }

        public event EventHandler<HLContent> onBlockTapped;
        private void RaiseEventBlockTapped(object sender, HLContent content)
        {
            if (onBlockTapped != null)
            {
                onBlockTapped(sender, content);
            }
        }

        private static void TextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ExtendedRichTextBlock ertb = dependencyObject as ExtendedRichTextBlock;
            if (ertb != null && args.NewValue != null)
            {
                string content = ertb.prepareContent(args.NewValue.ToString());
                ertb.richTextBlock.Blocks.Clear();
                foreach (Block b in ertb.collectBlocks(content))
                {
                    ertb.richTextBlock.Blocks.Add(b);
                }
            }
        }

        private string prepareContent(string content)
        {
            Regex regex = new Regex(@"(?<!<Hyperlink>)@@\{\w+\}.*?[.\d]+@@(?!<Hyperlink>)");
            foreach (var match in regex.Matches(content))
            {
                content = regex.Replace(content, String.Format("</Run><Hyperlink>{0}</Hyperlink><Run>", match), 1);
            }
            return content;
        }

        private List<Block> collectBlocks(string content)
        {
            string xmlTag = "xml";
            List<Block> blocks = new List<Block>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(String.Format("<{0}>{1}</{0}>", xmlTag, content));
            XmlNodeList xmlNodeList = xmlDoc.SelectNodes(xmlTag + "/*");
            foreach (IXmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.NodeName == "Paragraph")
                {
                    blocks.Add(generateParagraph(xmlNode));
                }
            }
            return blocks;
        }

        private Inline getInlineNode(IXmlNode node)
        {
            switch (node.NodeName)
            {
                case "Bold":
                    {
                        return generateBold(node);
                    }
                case "Run":
                    {
                        return generateRun(node);
                    }
                case "Hyperlink":
                    {
                        return generateHyperlink(node);
                    }
                default:
                    break;
            }
            return null;
        }

        private void addChildren(Paragraph parent, IXmlNode node)
        {
            foreach (IXmlNode child in node.ChildNodes)
            {
                Inline inline = getInlineNode(child);
                parent.Inlines.Add(inline != null ? inline : new Run() { Text = String.Empty });
            }
        }

        private void addChildren(Span parent, IXmlNode node)
        {
            foreach (IXmlNode child in node.ChildNodes)
            {
                Inline inline = getInlineNode(child);
                parent.Inlines.Add(inline != null ? inline : new Run() { Text = String.Empty });
            }
        }

        private Block generateParagraph(IXmlNode node)
        {
            Paragraph paragraph = new Paragraph();
            addChildren(paragraph, node);
            return paragraph;
        }

        private Inline generateBold(IXmlNode node)
        {
            Bold bold = new Bold();
            addChildren(bold, node);
            return bold;
        }

        private Inline generateRun(IXmlNode node)
        {
            Run run = new Run();
            run.Text = node.InnerText;
            run.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            run.FontSize = 18;
            return run;
        }

        private Inline generateHyperlink(IXmlNode node)
        {
            Hyperlink hyperlink = new Hyperlink();
            Run run = new Run();
            Regex regex = new Regex(@"(?<=@@)\{(?<type>\w+)\}.*?(?<text>[.\d]+)(?=@@)");
            Match match = regex.Match(node.InnerText);
            string type = match.Groups["type"].Value;
            string text = match.Groups["text"].Value;
            run.Text = text;
            run.FontSize = 18;
            run.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 165, 42, 42));
            hyperlink.Inlines.Add(run);
            hyperlink.Click += (s, e) =>
            {
                RaiseEventBlockTapped(s, new HLContent(type, text));
            };
            addChildren(hyperlink, node);
            return hyperlink;
        }
    }
}
