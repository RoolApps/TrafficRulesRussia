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
        const string RulesUrl = "https://pdd.am.ru/rules/";
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
                ChapterBuilder.StartChapter(e.Node.InnerHTML.Replace("<br>", String.Empty));
            };

            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(RulesUrl);
            Common.Log("Parsing chapters...");
            var rulesDiv = baseDocument.Find(@".au-accordion").Single();
            foreach (var node in rulesDiv.ChildNodes)
            {
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
            Content.AppendLine(content);
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
