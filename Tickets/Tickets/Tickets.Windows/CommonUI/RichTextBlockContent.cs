using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Tickets.CommonUI {

    public class HLContent : EventArgs {
        private string type;
        private string data;

        public HLContent( string type, string data ) {
            this.type = type;
            this.data = data;
        }

        public string Type {
            get {
                return type;
            }
        }

        public string Data {
            get {
                return data;
            }
        }
    }

    public class RichTextBlockContent {
        public static String GetContent( DependencyObject obj ) {
            return (String)obj.GetValue(ContentProperty);
        }

        public static void SetContent( DependencyObject obj, String value ) {
            obj.SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
           DependencyProperty.RegisterAttached("Content", typeof(string),
           typeof(RichTextBlockContent), new PropertyMetadata(String.Empty, OnContentChanged));

        public static event EventHandler<HLContent> onBlockTapped;
        protected static void RaiseEventBlockTapped( object sender, HLContent content ) {
            if(onBlockTapped != null) {
                onBlockTapped(sender, content);
            }
        }

        private static void OnContentChanged( DependencyObject d, DependencyPropertyChangedEventArgs e ) {
            RichTextBlock rtb = d as RichTextBlock;
            if(rtb != null) {
                string content = prepareContent(e.NewValue.ToString());
                rtb.Blocks.Clear();
                foreach(Block b in collectBlocks(content)) {
                    rtb.Blocks.Add(b);
                }
            }
        }

        private static string prepareContent( string content ) {
            Regex regex = new Regex(@"(?<!<Hyperlink>)@@\{\w+\}.*?[.\d]+@@(?!<Hyperlink>)");
            foreach(var match in regex.Matches(content)) {
                content = regex.Replace(content, String.Format("</Run><Hyperlink>{0}</Hyperlink><Run>", match), 1);
            }
            return content;
        }

        private static List<Block> collectBlocks( string content ) {
            string xmlTag = "xml";
            List<Block> blocks = new List<Block>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(String.Format("<{0}>{1}</{0}>", xmlTag, content));
            XmlNodeList xmlNodeList = xmlDoc.SelectNodes(xmlTag + "/*");
            foreach(IXmlNode xmlNode in xmlNodeList) {
                if(xmlNode.NodeName == "Paragraph") {
                    blocks.Add(generateParagraph(xmlNode));
                }
            }
            return blocks;
        }

        private static Inline getInlineNode( IXmlNode node ) {
            switch(node.NodeName) {
                case "Bold": {
                        return generateBold(node);
                    }
                case "Italic": {
                        return generateItalic(node);
                    }
                case "Run": {
                        return generateRun(node);
                    }
                case "Hyperlink": {
                        return generateHyperlink(node);
                    }
                default:
                    break;
            }
            return null;
        }

        private static void addChildren( Paragraph parent, IXmlNode node ) {
            foreach(IXmlNode child in node.ChildNodes) {
                Inline inline = getInlineNode(child);
                parent.Inlines.Add(inline != null ? inline : new Run() { Text = String.Empty });
            }
        }

        private static void addChildren( Span parent, IXmlNode node ) {
            foreach(IXmlNode child in node.ChildNodes) {
                Inline inline = getInlineNode(child);
                parent.Inlines.Add(inline != null ? inline : new Run() { Text = String.Empty });
            }
        }

        private static Block generateParagraph( IXmlNode node ) {
            Paragraph paragraph = new Paragraph();
            addChildren(paragraph, node);
            return paragraph;
        }

        private static Inline generateBold( IXmlNode node ) {
            Bold bold = new Bold();
            addChildren(bold, node);
            return bold;
        }

        private static Inline generateItalic(IXmlNode node) {
            Italic italic = new Italic();
            addChildren(italic, node);
            return italic;
        }

        private static Inline generateRun( IXmlNode node ) {
            Run run = new Run();
            run.Text = node.InnerText;
            return run;
        }

        private static Inline generateHyperlink( IXmlNode node ) {
            Hyperlink hyperlink = new Hyperlink();
            Run run = new Run();
            Regex regex = new Regex(@"(?<=@@)\{(?<type>\w+)\}.*?(?<text>[.\d]+)(?=@@)");
            Match match = regex.Match(node.InnerText);
            string type = match.Groups["type"].Value;
            string text = match.Groups["text"].Value;
            run.Text = text;
            hyperlink.Inlines.Add(run);
            hyperlink.Click += ( s, e ) => {
                RaiseEventBlockTapped(s, new HLContent(type, text));
            };
            addChildren(hyperlink, node);
            return hyperlink;
        }
    }
}
