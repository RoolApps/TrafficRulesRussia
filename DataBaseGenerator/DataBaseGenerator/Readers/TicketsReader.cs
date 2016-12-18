using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;

namespace DataBaseGenerator.Readers
{
    public static class TicketReader
    {
        const string BaseUrl = "http://pdd.drom.ru/";
        const string NoImageUrl = "http://c.rdrom.ru/skin/pdd_no_question.jpg";

        public static Ticket[] Read()
        {
            Common.Log("Downloading tickets...");
            CsQuery.Config.HtmlEncoder = new CsQuery.Output.HtmlEncoderNone();
            var baseDocument = CsQuery.CQ.CreateFromUrl(BaseUrl);
            Common.Log("Parsing tickets...");
            var ticketLinks = baseDocument.Find("div.numbers a");
            List<Task<Ticket>> taskList = new List<Task<Ticket>>();

            foreach (var ticketLink in ticketLinks)
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

                    foreach (var questionBlock in questionBlocks)
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
                        foreach (var answerBlock in answerBlocks)
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
            Common.Log("Tickets successfully parsed.");
            return taskList.Select(task => task.Result).ToArray();
        }
    }
}
