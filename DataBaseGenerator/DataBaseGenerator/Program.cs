using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace DataBaseGenerator
{
    static class NodeTypes
    {
        public const String Header = "H4";
        public const String Paragraph = "P";
        public const String Link = "A";
        public const String Bold = "STRONG";
        public const String Text = "#text";
        public const String Td = "TD";
        public const String Font = "FONT";
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
            if(chapter != null)
            {
                chapter.Content = Content.ToString();
                Content = new StringBuilder();
                Chapter = null;
            }
            return chapter;
        }
    }

    class Program
    {
        static SQLiteConnection Connection = null;
        const string BaseUrl = "http://pdd.drom.ru/";
        const string RulesUrl = "http://pdd.drom.ru/pdd/";
        const string SignsUrl = "http://pdd.drom.ru/pdd/signs/";
        const string MarksUrl = "http://pdd.drom.ru/pdd/marking/";
        const string NoImageUrl = "http://c.rdrom.ru/skin/pdd_no_question.jpg";
        const int MaximumTimeout = 60000;
        static Regex exactRegex = new Regex(@"^\d+(\.\d+)*$");
        static Regex regex = new Regex(@"(?<num>\d+(\.\d+)*)");
        static List<RuleChapter> Chapters = new List<RuleChapter>();
        static string[] dashArray = new string[] { "-", "—", "–" };


        static void Main(string[] args)
        {
            var data = GetData();
            CreateDB(data);
            Console.WriteLine("Successfully generated db. Press any key to finish...");
            Console.ReadKey();
        }

        static Data GetData()
        {
            return new Data()
            {
                Tickets = GetTickets(),
                Signs = GetSigns(),
                Marks = GetMarks(),
                Chapters = GetChapters()
            };
        }

        static RuleChapter[] GetChapters()
        {
            string ignoredHeader = "Правила дорожного движения";
            bool ignoreUntilHeader = true;

            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(RulesUrl);
            var rulesDiv = baseDocument.Find(@"div[style=""overflow-x: auto""]").Single();
            foreach(var node in rulesDiv.ChildNodes)
            {
                if (ignoreUntilHeader)
                {
                    if(node.NodeName != NodeTypes.Header || node.InnerHTML.Contains(ignoredHeader))
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
                var content = ParseNode(node);
                if(content != null)
                {
                    ChapterBuilder.AppendContent(content);
                }
            }
            return Chapters.ToArray();
        }

        static String ParseNode(CsQuery.IDomObject node)
        {
            if (node.NodeName == NodeTypes.Text)
            {
                return node.NodeValue;
            }
            if (node.NodeName == NodeTypes.Header)
            {
                var currentChapter = ChapterBuilder.PopChapter();
                if(currentChapter != null)
                {
                    Chapters.Add(currentChapter);
                }
                ChapterBuilder.StartChapter(node.ChildNodes.Single(n => n.NodeName != NodeTypes.Link).NodeValue);
            }
            if (node.NodeName == NodeTypes.Paragraph)
            {
                var pattern = "<Paragraph>{0}</Paragraph>";
                if (node.ChildNodes.Any())
                {
                    var content = String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode)));
                    return String.Format(pattern, content);
                }
                else
                {
                    return String.Format(pattern, node.NodeValue);
                }
            }
            if (node.NodeName == NodeTypes.Bold)
            {
                var pattern = "<Bold>{0}</Bold>";
                if (node.ChildNodes.Any())
                {
                    var content = String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode)));
                    return String.Format(pattern, content);
                }
                else
                {
                    return String.Format(pattern, node.NodeValue);
                }
            }
            if (node.NodeName == NodeTypes.Font)
            {
                if (node.ChildNodes.Any())
                {
                    return String.Join("", node.ChildNodes.Select(childNode => ParseNode(childNode)));
                }
            }
            if (node.NodeName == NodeTypes.Link)
            {
                if ((node.GetAttribute("href") ?? String.Empty).Contains("signs"))
                {
                    return ReplaceUrl(node.ChildNodes.Single(chileNode => chileNode.NodeName == NodeTypes.Text).NodeValue, "sign");
                }
                else if ((node.GetAttribute("href") ?? String.Empty).Contains("marks"))
                {
                    return ReplaceUrl(node.ChildNodes.Single(chileNode => chileNode.NodeName == NodeTypes.Text).NodeValue, "mark");
                }
            }
            return String.Empty;
        }

        static IEnumerable<CsQuery.IDomObject> FindResursive(CsQuery.IDomObject node, Func<CsQuery.IDomObject, bool> condition)
        {
            IEnumerable<CsQuery.IDomObject> result = new CsQuery.IDomObject[0];
            if(condition(node))
            {
                result = new CsQuery.IDomObject[] { node };
            }
            if(node.ChildNodes != null)
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

        static IEnumerable<String> Patch(IEnumerable<String> numbers)
        {
            var array = numbers.ToArray();
            List<String> result = new List<String>();
            int counter = 0;

            for (int i = 0; i < numbers.Count();i++ )
            {
                if (array[i].StartsWith("."))
                {
                    result[counter - 1] += array[i];
                }
                else if (dashArray.Any(dash => array[i].Contains(dash)))
                {
                    if(dashArray.Any(dash => array[i].Equals(dash)))
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
                    Console.WriteLine(String.Format("Skipped: {0}", array[i]));
                }

            }
            return result;
        }

        static bool Verify(IEnumerable<String> numbers)
        {
            return numbers.All(number => exactRegex.IsMatch(number));
        }

        static string ReplaceUrl(string text, string replace)
        {
            text = text.Replace('_', '.');
            return regex.Replace(text, String.Format("@@{{{0}}}${{num}}@@", replace));
        }

        static Ticket[] GetTickets()
        {
            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(BaseUrl);
            var ticketLinks = baseDocument.Find("div.numbers a");
            List<Task<Ticket>> taskList = new List<Task<Ticket>>();

            foreach(var ticketLink in ticketLinks)
            {
                var task = new Task<Ticket>(() =>
                {
                    var link = ticketLink;
                    var ticketNum = link.Cq().Find("span").First().Single().InnerText;

                    Ticket ticket = new Ticket();
                    ticket.Num = Convert.ToInt32(ticketNum);

                    var webClient = new WebClient();
                    webClient.Encoding = Encoding.GetEncoding(1251);
                    var url = String.Format("http://pdd.drom.ru/bilet_{0}", ticketNum);
                    var documentContent = webClient.DownloadString(url);
                    CsQuery.CQ document = new CsQuery.CQ(documentContent);
                    var questionBlocks = document.Find(".pdd-question-block");

                    List<Question> questions = new List<Question>();

                    foreach(var questionBlock in questionBlocks)
                    {
                        var question = new Question();

                        var csQuestionBlock = new CsQuery.CQ(questionBlock);
                        Thread thread = new Thread(new ThreadStart(() =>
                        {
                            var imageUrl = csQuestionBlock.Find(".question img").Attr("src");
                            if (imageUrl != NoImageUrl)
                            {
                                question.Image = new WebClient().DownloadData(imageUrl);
                            }
                        }));
                        thread.Start();

                        question.Ticket = ticket;
                        question.Num = Convert.ToInt32(csQuestionBlock.Find("a").Attr("name"));
                        question.Text = csQuestionBlock.Find(".question p").Single().InnerText;
                        

                        var answerBlocks = csQuestionBlock.Find(".answer");
                        List<Answer> answers = new List<Answer>();
                        foreach(var answerBlock in answerBlocks)
                        {
                            var answer = new Answer();
                            answer.Question = question;
                            var csAnswerBlock = new CsQuery.CQ(answerBlock);
                            answer.Text = csAnswerBlock.Find("span").Single().InnerText;
                            answer.IsRight = answerBlock.HasAttribute("id");
                            answers.Add(answer);
                        }
                        question.Answers = answers.ToArray();
                        
                        thread.Join();
                        questions.Add(question);
                    }

                    ticket.Questions = questions.ToArray();
                    return ticket;
                });
                task.Start();
                taskList.Add(task);
            }
            Task.WaitAll(taskList.ToArray());
            return taskList.Select(task => task.Result).ToArray();
        }

        static Sign[] GetSigns()
        {
            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(SignsUrl);
            var signsTables = baseDocument.Find(@"div[style=""overflow-x: auto""] table tr");

            return signsTables.SelectMany(signNode =>
            {
                var leftNode = signNode.ChildNodes.First(node => node.NodeName == NodeTypes.Td).Cq();
                var rightNode = signNode.ChildNodes.Last(node => node.NodeName == NodeTypes.Td).Cq();
                IEnumerable<String> signsCaptions;
                var captions = leftNode.Find("strong");
                signsCaptions = String.Join(" ", captions.SelectMany(node => FindResursive(node, n => n.NodeName == NodeTypes.Text)).Select(node => node.NodeValue))
                    .Split(new char[] { ' ', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(caption => caption.Trim()).Where(caption => !String.Empty.Equals(caption));
                if(!signsCaptions.Any())
                {
                    return new Sign[0];
                }
                if(!Verify(signsCaptions))
                {
                    signsCaptions = Patch(signsCaptions);
                }

                var signs = new List<Sign>();
                var description = String.Join(Environment.NewLine, rightNode.Single().ChildNodes.Select(node => ParseNode(node)));
                var name = rightNode.Find("strong").First().Single().InnerHTML;
                var imagesUrls = leftNode.Find("img").Select(node => node.GetAttribute("src"));
                var images = imagesUrls.Select(url => new WebClient().DownloadData(url)).ToArray();
                if (images.Length < signsCaptions.Count())
                {
                    images = Enumerable.Range(0, signsCaptions.Count()).Select(i => images.First()).ToArray();
                }
                for (int i = 0; i < signsCaptions.Count(); i++)
                {
                    var sign = new Sign();
                    sign.Description = description;
                    sign.Num = signsCaptions.ElementAt(i);
                    sign.Image = images[i];
                    signs.Add(sign);
                }

                return signs.ToArray();
            }).ToArray();
        }

        static Mark[] GetMarks()
        {
            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(MarksUrl);
            var marksTables = baseDocument.Find(@"div[style=""overflow-x: auto""] table tr");

            return marksTables.SelectMany(signNode =>
            {
                var leftNode = signNode.ChildNodes.First(node => node.NodeName == NodeTypes.Td).Cq();
                var rightNode = signNode.ChildNodes.Last(node => node.NodeName == NodeTypes.Td).Cq();
                IEnumerable<String> marksCaptions;
                var captions = rightNode.Find("strong");
                marksCaptions = String.Join(" ", captions.SelectMany(node => FindResursive(node, n => n.NodeName == NodeTypes.Text)).Select(node => node.NodeValue))
                   .Split(new char[] { ' ', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(caption => caption.Trim()).Where(caption => !String.Empty.Equals(caption));
                if (!marksCaptions.Any())
                {
                    return new Mark[0];
                }
                if (!Verify(marksCaptions))
                {
                    marksCaptions = Patch(marksCaptions);
                }

                var marks = new List<Mark>();
                var description = String.Join(Environment.NewLine, rightNode.Single().ChildNodes.Select(node => ParseNode(node)));
                var imagesUrls = leftNode.Find("img").Select(node => node.GetAttribute("src"));
                var images = imagesUrls.Select(url => new WebClient().DownloadData(url)).ToArray();
                if (images.Length < marksCaptions.Count())
                {
                    images = Enumerable.Range(0, marksCaptions.Count()).Select(i => images.First()).ToArray();
                }
                for (int i = 0; i < marksCaptions.Count(); i++)
                {
                    var mark = new Mark();
                    mark.Num = marksCaptions.ElementAt(i);
                    mark.Description = description;
                    mark.Image = images[i];
                    marks.Add(mark);
                }

                return marks.ToArray();
            }).ToArray();
        }

        static void CreateDB(Data data)
        {
            System.IO.File.Delete("tickets.db");
            Connection = new SQLiteConnection("Data Source=tickets.db;Version=3;");
            Connection.Open();

            CreateSchema();
            InsertData(data);

            Connection.Close();
        }

        static void CreateSchema()
        {
            ExecuteCommand("CREATE TABLE Tickets (id integer primary key, num integer)");
            ExecuteCommand("CREATE TABLE Questions (id integer primary key, num integer, question text, image blob, ticket_id integer)");
            ExecuteCommand("CREATE TABLE Answers (id integer primary key, answer text, is_right integer, question_id integer)");
            ExecuteCommand("CREATE TABLE Chapters (id integer primary key, name text, content text)");
            ExecuteCommand("CREATE TABLE Marks (id integer primary key, num text, description text, image blob)");
            ExecuteCommand("CREATE TABLE Signs (id integer primary key, num text, description text, image blob)");
        }

        static void InsertData(Data data)
        {
            using(var transaction = Connection.BeginTransaction())
            {
                using (var ticketCommand = Connection.CreateCommand())
                {
                    ticketCommand.CommandText = "INSERT INTO Tickets (num) Values (?)";
                    ticketCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.Int32 });
                    foreach (var ticket in data.Tickets)
                    {
                        ticketCommand.Parameters["@num"].Value = ticket.Num;
                        ticketCommand.ExecuteNonQuery();

                        using (var questionCommand = Connection.CreateCommand())
                        {
                            questionCommand.CommandText = "INSERT INTO Questions (num, question, image, ticket_id) Values (?,?,?,?)";
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.Int32 });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@questions", DbType = System.Data.DbType.String });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                            questionCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@ticket_id", DbType = System.Data.DbType.Int32, Value = ticket.Num });

                            using (var answerCommand = Connection.CreateCommand())
                            {
                                answerCommand.CommandText = "INSERT INTO Answers (answer, is_right, question_id) Values (?,?,?)";
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@answer", DbType = System.Data.DbType.String });
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@is_right", DbType = System.Data.DbType.Int32 });
                                answerCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@question_id", DbType = System.Data.DbType.Int32 });

                                foreach (var question in ticket.Questions)
                                {
                                    questionCommand.Parameters["@num"].Value = question.Num;
                                    questionCommand.Parameters["@questions"].Value = question.Text;
                                    questionCommand.Parameters["@image"].Value = question.Image;
                                    if (question.Image != null)
                                    {
                                        questionCommand.Parameters["@image"].Size = question.Image.Length;
                                    }
                                    questionCommand.ExecuteNonQuery();

                                    answerCommand.Parameters["@question_id"].Value = (ticket.Num - 1) * 20 + question.Num;
                                    foreach (var answer in question.Answers)
                                    {
                                        answerCommand.Parameters["@answer"].Value = answer.Text;
                                        answerCommand.Parameters["@is_right"].Value = answer.IsRight;
                                        answerCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                    }
                }

                using (var chapterCommand = Connection.CreateCommand())
                {
                    chapterCommand.CommandText = "INSERT INTO Chapters (name, content) Values (?,?)";
                    chapterCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@name", DbType = System.Data.DbType.String });
                    chapterCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@content", DbType = System.Data.DbType.String });
                    foreach (var chapter in data.Chapters)
                    {
                        chapterCommand.Parameters["@name"].Value = chapter.Name;
                        chapterCommand.Parameters["@content"].Value = chapter.Content;
                        chapterCommand.ExecuteNonQuery();
                    }
                }

                using (var markCommand = Connection.CreateCommand())
                {
                    markCommand.CommandText = "INSERT INTO Marks (num, description, image) Values (?,?,?)";
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.String });
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@description", DbType = System.Data.DbType.String });
                    markCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                    foreach(var mark in data.Marks)
                    {
                        markCommand.Parameters["@num"].Value = mark.Num;
                        markCommand.Parameters["@description"].Value = mark.Description;
                        markCommand.Parameters["@image"].Value = mark.Image;
                        markCommand.Parameters["@image"].Size = mark.Image.Length;
                        markCommand.ExecuteNonQuery();
                    }
                }

                using (var signCommand = Connection.CreateCommand())
                {
                    signCommand.CommandText = "INSERT INTO Signs (num, description, image) Values (?,?,?)";
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@num", DbType = System.Data.DbType.String });
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@description", DbType = System.Data.DbType.String });
                    signCommand.Parameters.Add(new SQLiteParameter() { ParameterName = "@image", DbType = System.Data.DbType.Binary });
                    foreach (var sign in data.Signs)
                    {
                        signCommand.Parameters["@num"].Value = sign.Num;
                        signCommand.Parameters["@description"].Value = sign.Description;
                        signCommand.Parameters["@image"].Value = sign.Image;
                        signCommand.Parameters["@image"].Size = sign.Image.Length;
                        signCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            
        }

        static void ExecuteCommand(String text)
        {
            using(var command = Connection.CreateCommand())
            {
                command.CommandText = text;
                command.ExecuteNonQuery();
            }
        }
    }
}
