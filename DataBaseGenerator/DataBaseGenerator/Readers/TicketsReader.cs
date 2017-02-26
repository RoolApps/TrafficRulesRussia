using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataBaseGenerator.DataBase;

using Newtonsoft.Json;
using System.IO;

namespace DataBaseGenerator.Readers {
    public static class TicketReader {
        private const int maxTicketCount = 40;
        private const int maxQuestionCount = 20;
        private const string baseUrl = "https://pdd.am.ru/int/controls/get-question/?category=1&type=1&object=";

        public static Ticket[] Read() {
            Common.Log("Downloading tickets...");
            List<Task<Ticket>> taskList = new List<Task<Ticket>>();
            Dictionary<int, CookieCollection> savedCookie = new Dictionary<int, CookieCollection>();

            foreach (var currentTicket in Enumerable.Range(1, maxTicketCount)) {
                var task = new Task<Ticket>(() => {
                    Ticket ticket = new Ticket();
                    ticket.Num = currentTicket;
                    List<Question> questions = new List<Question>();
                    foreach (var currentQuestion in Enumerable.Range(1, maxQuestionCount)) {
                        string url = baseUrl + currentTicket.ToString();
                        if (currentQuestion != 1) {
                            url += "&answer=1";
                        }
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.CookieContainer = new CookieContainer();
                        request.CookieContainer.Add(savedCookie.ContainsKey(currentTicket) ? savedCookie[currentTicket] : new CookieCollection());
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var jsonObject = JsonConvert.DeserializeObject<jsonModel.jsonRootObject>(responseStreamReader.ReadToEnd());
                        Question question = new Question();
                        question.Text = jsonObject.data.question.text;
                        question.Num = jsonObject.data.question.orderNumber;
                        Thread downloadImg = new Thread(new ThreadStart(() => {
                            var imageUrl = jsonObject.data.question.image;
                            if (imageUrl != string.Empty) {
                                question.Image = new WebClient().DownloadData(imageUrl);
                            } else {
                                question.Image = null;
                            }
                        }));
                        downloadImg.Start();

                        question.Ticket = ticket;
                        List<Answer> answers = new List<Answer>();
                        foreach (var a in jsonObject.data.question.answers) {
                            var answer = new Answer();
                            answer.Text = a.text;
                            answer.IsRight = a.isRight;
                            answer.Question = question;
                            answers.Add(answer);
                        }
                        question.Answers = answers.ToArray();

                        downloadImg.Join();
                        questions.Add(question);

                        if (currentQuestion == 1) {
                            savedCookie[currentTicket] = response.Cookies;
                        }
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
