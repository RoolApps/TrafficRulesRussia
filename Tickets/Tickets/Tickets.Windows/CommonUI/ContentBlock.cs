using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Markup;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Windows.Data.Xml.Dom;

namespace Tickets.CommonUI {
    public class ContentBlock : Grid, INotifyPropertyChanged {
        private RichTextBlock richTextBlock;

        private String content = String.Empty;
        private String ns = String.Empty;

        public ContentBlock() {
            richTextBlock = new RichTextBlock();
            PropertyChanged += ContentBlock_PropertyChanged;
            this.Children.Add(richTextBlock);
        }

        private void ContentBlock_PropertyChanged( object sender, PropertyChangedEventArgs e ) {
            if(e.PropertyName == "Content") {
                updateContent();
            }
        }

        private void updateContent() {
            if(ns == String.Empty) {
                return;
            }
            richTextBlock.Blocks.Clear();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<xml>" + Content + "</xml>");
            XmlNodeList xmlList = doc.SelectNodes("/xml/*");
            foreach(IXmlNode node in xmlList) {
                if(node.NodeName == "Paragraph") {
                    
                    String p = parseParagraph(node.GetXml());
                    Paragraph paragraph = (Paragraph)XamlReader.Load(p);
                    richTextBlock.Blocks.Add(paragraph);
                }
            }
        }

        private String parseParagraph( String paragraph ) {
            String xmlnsLocal = ns;
            String text = "";
            String type = "";

            Regex pattern = new Regex(@"(?<=@@)\{(\w{1,})\}.*?([.\d]{1,})(?=@@)");
            var matches = pattern.Matches(paragraph);

            foreach(Match match in matches) {
                type = match.Groups[1].Value;
                text = match.Groups[2].Value;
                String mytextblock = String.Format(@"</Run><InlineUIContainer xmlns:common=""{0}"">
                                                        <common:InlineRichElement Text=""{1}"" Type=""{2}""/>
                                                     </InlineUIContainer><Run>", xmlnsLocal, text, type);
                paragraph = Regex.Replace(paragraph, "@@" + match.Value + "@@", mytextblock);
            }
            return paragraph;
        }

        public String Content {
            get {
                return content;
            }
            set {
                content = value;
                RaisePropertyChanged("Content");
            }
        }

        public String Ns {
            get {
                return ns;
            }
            set {
                ns = value;
                RaisePropertyChanged("ns");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged( String name ) {
            if(PropertyChanged != null) {
                PropertyChanged(null, new PropertyChangedEventArgs(name));
            }
        }
    }
}
