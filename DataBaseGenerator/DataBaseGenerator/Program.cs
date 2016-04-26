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
    class Program
    {
        static SQLiteConnection Connection = null;
        const string BaseUrl = "http://pdd.drom.ru/";
        const string NoImageUrl = "http://c.rdrom.ru/skin/pdd_no_question.jpg";
        const int MaximumTimeout = 60000;

        static void Main(string[] args)
        {
            var data = GetData();
            CreateDB(data);
        }

        static Data GetData()
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
                    String url;
                    if(!link.TryGetAttribute("href", out url))
                    {
                        throw new Exception(String.Format("Cannot extract url from element: {0}", link.ToString()));
                    }
                    var ticketNum = link.InnerText;

                    Ticket ticket = new Ticket();
                    ticket.Num = Convert.ToInt32(ticketNum);

                    var webClient = new WebClient();
                    webClient.Encoding = Encoding.GetEncoding(1251);
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
            return new Data() { Tickets = taskList.Select(task => task.Result).ToArray() };
        }

        static void CreateDB(Data data)
        {
            System.IO.File.Delete("tickets.db");
            Connection = new SQLiteConnection("tickets.db");
            Connection.Open();

            using (Connection)
            {
                CreateSchema();

            }

            Connection.Clone();
        }

        static void CreateSchema()
        {
            ExecuteCommand("CREATE TABLE Tickets (id integer primary key, num integer)");
            ExecuteCommand("CREATE TABLE Questions (id integer primary key, num integer, question text, image blob, ticket_id integer)");
            ExecuteCommand("CREATE TABLE Answers (id integer primary key, answer text, is_right integer, question_id integer)");
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
