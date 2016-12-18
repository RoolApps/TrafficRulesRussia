using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;

namespace DataBaseGenerator.Readers
{
    public static class ChapterReader
    {
        const string RulesUrl = "http://pdd.drom.ru/pdd/";
        static List<RuleChapter> Chapters = new List<RuleChapter>();

        public static RuleChapter[] Read()
        {
            Common.Log("Downloading chapters...");
            Common.HeaderAppeared += (s, e) =>
            {
                var currentChapter = ChapterBuilder.PopChapter();
                if (currentChapter != null)
                {
                    Chapters.Add(currentChapter);
                }
                ChapterBuilder.StartChapter(e.Node.ChildNodes.Single(n => n.NodeName != NodeTypes.Link).NodeValue);
            };

            string ignoredHeader = "Правила дорожного движения";
            bool ignoreUntilHeader = true;

            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(RulesUrl);
            Common.Log("Parsing chapters...");
            var rulesDiv = baseDocument.Find(@"div[style=""overflow-x: auto""]").Single();
            foreach (var node in rulesDiv.ChildNodes)
            {
                if (ignoreUntilHeader)
                {
                    if (node.NodeName != NodeTypes.Header || node.InnerHTML.Contains(ignoredHeader))
                    {
                        continue;
                    }
                    else
                    {
                        ignoreUntilHeader = false;
                    }
                }
                else
                {
                    if (node.NodeName == NodeTypes.Header && node.InnerHTML.Contains(ignoredHeader))
                    {
                        ignoreUntilHeader = true;
                        continue;
                    }
                }
                var content = Common.ParseNode(node);
                if ((content ?? String.Empty) != String.Empty)
                {
                    ChapterBuilder.AppendContent(content);
                }
            }
            Common.Log("Chapters successfully parsed.");
            return Chapters.ToArray();
        }
    }

    public static class ChapterBuilder
    {
        private static StringBuilder Content = new StringBuilder();
        private static RuleChapter Chapter = null;

        public static void StartChapter(String chapterName)
        {
            Chapter = new RuleChapter()
            {
                Name = chapterName
            };
        }

        public static void AppendContent(String content)
        {
            Content.Append(content);
        }

        public static RuleChapter PopChapter()
        {
            var chapter = Chapter;
            if (chapter != null)
            {
                chapter.Content = Content.ToString();
                Content = new StringBuilder();
                Chapter = null;
            }
            return chapter;
        }
    }
}
