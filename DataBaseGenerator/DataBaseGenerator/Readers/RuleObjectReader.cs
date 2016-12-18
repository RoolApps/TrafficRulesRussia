using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;

namespace DataBaseGenerator.Readers
{
    public static class RuleObjectReader
    {
        static Regex exactRegex = new Regex(@"^\d+(\.\d+)*$");
        static string[] dashArray = new string[] { "-", "—", "–" };

        public static T[] Read<T>(string pageUrl) where T : IRuleObject, new()
        {
            Common.Log(String.Format("Downloading {0}s...", typeof(T).Name));
            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(pageUrl);
            var ruleObjectsTables = baseDocument.Find(@"div[style=""overflow-x: auto""] table tr");
            Common.Log(String.Format("Parsing {0}s...", typeof(T).Name));

            var result = ruleObjectsTables.SelectMany(ruleObjectNode =>
            {
                var leftNode = ruleObjectNode.ChildNodes.First(node => node.NodeName == NodeTypes.Td).Cq();
                var rightNode = ruleObjectNode.ChildNodes.Last(node => node.NodeName == NodeTypes.Td).Cq();
                IEnumerable<String> ruleObjectCaptions;
                var captions = FindCaptions<T>(leftNode, rightNode);
                ruleObjectCaptions = String.Join(" ", captions.SelectMany(node => FindResursive(node, n => n.NodeName == NodeTypes.Text)).Select(node => node.NodeValue))
                    .Split(new char[] { ' ', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(caption => caption.Trim()).Where(caption => !String.Empty.Equals(caption));
                if (!ruleObjectCaptions.Any())
                {
                    return new T[0];
                }
                if (!Verify(ruleObjectCaptions))
                {
                    ruleObjectCaptions = Patch(ruleObjectCaptions);
                }

                var ruleObjects = new List<T>();
                var description = String.Join(Environment.NewLine, rightNode.Single().ChildNodes.Select(node => Common.ParseNode(node)));
                var imagesUrls = leftNode.Find("img").Select(node => node.GetAttribute("src"));
                var images = imagesUrls.Select(url => new WebClient().DownloadData(url)).ToArray();
                if (images.Length < ruleObjectCaptions.Count())
                {
                    images = Enumerable.Range(0, ruleObjectCaptions.Count()).Select(i => images.First()).ToArray();
                }
                for (int i = 0; i < ruleObjectCaptions.Count(); i++)
                {
                    var ruleObject = new T();
                    ruleObject.Description = description;
                    ruleObject.Num = ruleObjectCaptions.ElementAt(i);
                    ruleObject.Image = images[i];
                    ruleObjects.Add(ruleObject);
                }

                return ruleObjects.ToArray();
            }).ToArray();
            Common.Log(String.Format("{0}s successfully parsed.", typeof(T).Name));
            return result;
        }

        static CsQuery.CQ FindCaptions<T>(CsQuery.CQ leftNode, CsQuery.CQ rightNode) where T : IRuleObject
        {
            if (typeof(T) == typeof(Mark))
            {
                return rightNode.Find("strong");
            }
            else if (typeof(T) == typeof(Sign))
            {
                return leftNode.Find("strong");
            }
            else throw new NotImplementedException();
        }

        static IEnumerable<String> Patch(IEnumerable<String> numbers)
        {
            var array = numbers.ToArray();
            List<String> result = new List<String>();
            int counter = 0;

            for (int i = 0; i < numbers.Count(); i++)
            {
                if (array[i].StartsWith("."))
                {
                    result[counter - 1] += array[i];
                }
                else if (dashArray.Any(dash => array[i].Contains(dash)))
                {
                    if (dashArray.Any(dash => array[i].Equals(dash)))
                    {
                        var lower = array[i - 1];
                        var upper = array[i + 1];
                        var nums = GetNumbers(lower, upper);
                        result.AddRange(nums);
                        counter += nums.Count();
                    }
                    else
                    {
                        var splitter = dashArray.Single(dash => array[i].Contains(dash)).Single();
                        var bounds = array[i].Split(splitter);
                        var lower = bounds.First();
                        var upper = bounds.Last();
                        var nums = GetNumbers(lower, upper);
                        result.Add(lower);
                        result.AddRange(nums);
                        result.Add(upper);
                        counter += nums.Count() + 2;
                    }
                }
                else if (exactRegex.IsMatch(array[i]))
                {
                    result.Add(array[i]);
                    counter++;
                }
                else
                {
                    Common.Log(String.Format("Skipped: {0}", array[i]));
                }

            }
            return result;
        }

        static bool Verify(IEnumerable<String> numbers)
        {
            return numbers.All(number => exactRegex.IsMatch(number));
        }

        static IEnumerable<CsQuery.IDomObject> FindResursive(CsQuery.IDomObject node, Func<CsQuery.IDomObject, bool> condition)
        {
            IEnumerable<CsQuery.IDomObject> result = new CsQuery.IDomObject[0];
            if (condition(node))
            {
                result = new CsQuery.IDomObject[] { node };
            }
            if (node.ChildNodes != null)
            {
                result = result.Concat(node.ChildNodes.SelectMany(n => FindResursive(n, condition)));
            }
            return result;
        }

        static IEnumerable<String> GetNumbers(String lower, String upper)
        {
            var dotIndex = lower.LastIndexOf('.');
            var constPart = lower.Substring(0, dotIndex);
            var lowerInt = Convert.ToInt32(lower.Substring(dotIndex + 1));
            var upperInt = Convert.ToInt32(upper.Substring(dotIndex + 1));
            return Enumerable.Range(lowerInt + 1, upperInt - lowerInt - 1).Select(number => String.Format("{0}.{1}", constPart, number));
        }
    }
}
