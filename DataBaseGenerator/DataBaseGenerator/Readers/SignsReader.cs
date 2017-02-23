using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;
using System.Threading;

namespace DataBaseGenerator.Readers
{
    public static class SignsReader
    {
        class SignParser : RuleObjectParser
        {
            static string basePageUrl = "https://pdd.am.ru/road-signs";
            static string baseUrl = "https://pdd.am.ru";

            public override String Name
            {
                get
                {
                    return "Signs";
                }
            }
            
            protected override IRuleObject Create()
            {
                return new Sign();
            }

            public override IEnumerable<CsQuery.IDomObject> DownloadData()
            {
                CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
                var baseDocument = CsQuery.CQ.CreateFromUrl(basePageUrl);
                Common.Log(String.Format("Parsing main {0} page...", Name));
                var ruleObjectsPages = baseDocument.Find(".pdd-index-blocks .pdd-index-block");
                Common.Log(String.Format("{0} {1} pages found", ruleObjectsPages.Length, Name));

                var documentsDownloaded = 0;

                var tasks = ruleObjectsPages.Select(a =>
                {
                    var pageUrl = a.GetAttribute("href");
                    var task = new Task<CsQuery.CQ>(() =>
                    {
                        var document = CsQuery.CQ.CreateFromUrl(baseUrl + pageUrl);
                        Common.Log(String.Format("{0} pages downloaded: {1}", Name, Interlocked.Increment(ref documentsDownloaded)));
                        var signsTable = document.Find(".au-accordion__table tbody tr");
                        return signsTable;
                    });
                    task.Start();
                    return task;
                }).ToArray();
                Task.WaitAll(tasks);

                return tasks.SelectMany(task => task.Result);
            }

            protected override String GetNumberText(CsQuery.CQ node)
            {
                var text = node.Single().InnerText.Trim();
                if(text == String.Empty)
                {
                    return node.Find("p").Single().InnerText.Trim();
                }
                else
                {
                    return text;
                }
            }

            protected override String GetDescription(CsQuery.CQ descriptionNode)
            {
                var caption = descriptionNode.Find("h3").Single().InnerText.Trim();
                if (caption == String.Empty)
                {
                    caption = descriptionNode.Find("h3 span").Single().InnerText.Trim();
                }
                caption = Common.WrapText(caption);
                var detailedDescription = descriptionNode.Find(".au-accordion__target");
                if (detailedDescription.Any())
                {
                    return String.Format("{0}{1}{2}", caption, Environment.NewLine, String.Join(Environment.NewLine, detailedDescription.Single().ChildNodes.Select(node => Common.ParseNode(node))));
                }
                else
                {
                    return caption;
                }
            }
        }

        public static Sign[] Read()
        {
            var parser = new SignParser();
            return RuleObjectReader.Read(parser).Cast<Sign>().ToArray();
        }
    }
}
