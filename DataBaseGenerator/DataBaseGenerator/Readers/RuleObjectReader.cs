using DataBaseGenerator.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerator.Readers
{
    public static class RuleObjectReader
    {
        public static IRuleObject[] Read(IRuleObjectParser parser)
        {
            Common.Log(String.Format("Downloading {0}...", parser.Name));
            var unparsed = parser.DownloadData();
            Common.Log(String.Format("Successfully downloaded {0}.", parser.Name));

            Common.Log(String.Format("Parsing {0}...", parser.Name));
            var parsed = unparsed.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).SelectMany(node => parser.Parse(node)).ToArray();
            Common.Log(String.Format("Successfully parsed {0}.", parser.Name));

            return parsed;
        }
    }

    public interface IRuleObjectParser
    {
        String Name { get; }
        IEnumerable<CsQuery.IDomObject> DownloadData();
        IRuleObject[] Parse(CsQuery.IDomObject trNode);
    }

    public abstract class RuleObjectParser : IRuleObjectParser
    {
        private static char[] dashArray = new char[] { '-', '—', '–' };

        public abstract String Name { get; }
        public abstract IEnumerable<CsQuery.IDomObject> DownloadData();

        protected abstract IRuleObject Create();
        protected abstract String GetNumberText(CsQuery.CQ node);
        protected abstract String GetDescription(CsQuery.CQ descriptionNode);

        public IRuleObject[] Parse(CsQuery.IDomObject trNode)
        {
            var leftNode = trNode.ChildNodes.First(node => node.NodeName == NodeTypes.Td).Cq();
            var rightNode = trNode.ChildNodes.Last(node => node.NodeName == NodeTypes.Td).Cq();

            var numberText = GetNumberText(leftNode);
            var numbers = GetNumbers(numberText);

            var urls = GetUrls(rightNode);
            var images = GetImages(urls);
            var description = GetDescription(rightNode);

            var numbersStack = new Stack<String>(numbers);
            var imagesStack = new Stack<byte[]>(images);
            List<IRuleObject> ruleObjects = new List<IRuleObject>();
            while(numbersStack.Any())
            {
                var number = numbersStack.Pop();
                var image = imagesStack.Pop();

                var ruleObject = Create();
                ruleObject.Num = number;
                ruleObject.Image = image;
                ruleObject.Description = description;

                ruleObjects.Add(ruleObject);
            }
            return ruleObjects.ToArray();

        }

        private String[] GetUrls(CsQuery.CQ node)
        {
            return node.Find("img").Not(".pdd-inline-sign__icon").Not("p span img").Select(img => img.GetAttribute("src")).ToArray();
        }

        private byte[][] GetImages(String[] urls)
        {
            var tasks = urls.Select(url =>
            {
                var link = url;

                var task = new Task<byte[]>(() =>
                {
                    return new WebClient().DownloadData(link);
                });
                task.Start();
                return task;
            }).ToArray();
            Task.WaitAll(tasks);
            return tasks.Select(task => task.Result).ToArray();
        }

        static String[] GetNumbers(String text)
        {
            if (dashArray.Any(dash => text.Contains(dash)))
            {
                var splitter = dashArray.Single(dash => text.Contains(dash));
                var borders = text.Split(splitter).Select(border => border.Trim());
                var first = Convert.ToInt32(borders.First().Split('.').Last());
                var last = Convert.ToInt32(borders.Last().Split('.').Last());
                var prefix = borders.First().Substring(0, borders.First().LastIndexOf('.'));
                return Enumerable.Range(first, last - first + 1).Select(number => String.Format("{0}.{1}", prefix, number)).ToArray();
            }
            else if (text.Contains(","))
            {
                return text.Split(',').Select(number => number.Trim()).ToArray();
            }
            else
            {
                return new String[] { text.Trim() };
            }
        }
    }
}
