using DataBaseGenerator.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerator.Readers
{
    public static class MarksReader
    {
        class MarksParser : RuleObjectParser
        {
            static string pageUrl = "https://pdd.am.ru/road-mark/";

            public override string Name
            {
                get 
                {
                    return "Marks";
                }
            }

            protected override IRuleObject Create()
            {
                return new Mark();
            }

            public override IEnumerable<CsQuery.IDomObject> DownloadData()
            {
                CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
                var document = CsQuery.CQ.CreateFromUrl(pageUrl);

                return document.Find(".au-accordion__table tbody tr");
            }

            protected override string GetNumberText(CsQuery.CQ node)
            {
                var p = node.Find("p");
                if (p.Any())
                {
                    node = p;
                }
                return node.Single().InnerText;
            }

            protected override string GetDescription(CsQuery.CQ descriptionNode)
            {
                var nodes = descriptionNode.Find(@"[style=""font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: black;""]");
                return String.Join(Environment.NewLine, nodes.Select(node => Common.ParseNode(node)));
            }
        }

        public static Mark[] Read()
        {
            var parser = new MarksParser();
            return RuleObjectReader.Read(parser).Cast<Mark>().ToArray();
        }
    }
}
