using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteShared;
using SQLiteShared.Models;
using AppLogic.Interfaces;
using AppLogic.Constants;

namespace AppLogic {
    class TicketLoader : ITicketLoader {
        public IEnumerable<ITicket> LoadTickets( ISessionParameters parameters ) {
            SQLiteDataAccessor dataAccessor = null;
            try {
                dataAccessor = new SQLiteDataAccessor();
                
                Tickets[] selectedTickets;
                IEnumerable<Questions> questionList;
                if(new Enums.QuestionsGenerationMode[] {Enums.QuestionsGenerationMode.RandomTicket, Enums.QuestionsGenerationMode.SelectedTickets}.Contains(parameters.Mode)) {
                    if(parameters.Mode == Enums.QuestionsGenerationMode.RandomTicket) {
                        var tickets = dataAccessor.CreateQuery<Tickets>();
                        Random rand = new Random();
                        selectedTickets = new Tickets[] { tickets.ElementAt(rand.Next(tickets.Count())) };
                    } else {
                        selectedTickets = dataAccessor.CreateQuery<Tickets>().Where(ticket => parameters.TicketNums.Contains(ticket.num)).ToArray();
                    }
                    questionList = dataAccessor.CreateQuery<Questions>().Where(question => selectedTickets.Any(ticket => ticket.id == question.ticket_id));
                } else if(parameters.Mode == Enums.QuestionsGenerationMode.ExamTicket) {
                    var tickets = dataAccessor.CreateQuery<Tickets>();
                    selectedTickets = new Tickets[] { tickets.ElementAt(0) };
                    Random rnd = new Random();

                    var randomTickets = Enumerable.Range(1, GlobalConstants.ticketsCount).OrderBy(i => rnd.Next()).Select(( item, index ) => new { id = index, num = item }).Take(GlobalConstants.questionsCount).ToArray();
                    var randomQuestions = Enumerable.Range(1, GlobalConstants.questionsCount).OrderBy(i => rnd.Next()).Select(( item, index ) => new { id = index, num = item }).ToArray();
                    questionList = dataAccessor.CreateQuery<Questions>().Join(randomTickets, question => question.ticket_id, ticket => ticket.num, ( question, ticket ) => new { Id = ticket.id, Question = question }).Where(question => question.Question.num == randomQuestions.ElementAt(question.Id).num).OrderBy(item => item.Question.num).Select(i => i.Question);
                } else {
                    throw new NotImplementedException(String.Format("Not supported mode", parameters.Mode));
                }

                var questionIdList = questionList.Select(question => question.id).ToArray();
                var answerList = dataAccessor.CreateQuery<Answers>().Where(answer => questionIdList.Contains(answer.question_id));

                var itickets = selectedTickets.Select(ticket => {
                    var iticket = new Ticket {
                        Number = ticket.num
                    };
                    var iticketQuestion = parameters.Mode != Enums.QuestionsGenerationMode.ExamTicket ? questionList.Where(question => question.ticket_id == ticket.id) : questionList;
                    iticket.Questions = iticketQuestion.Select(question => {
                        var iquestion = new Question {
                            Ticket = iticket,
                            Number = question.num,
                            Image = question.image,
                            Text = question.question
                        };
                        iquestion.Answers = answerList.Where(answer => answer.question_id == question.id).Select(answer => {
                            var ianswer = new Answer() {
                                Question = iquestion,
                                IsRight = answer.is_right,
                                Text = answer.answer,
                            };
                            return ianswer;
                        }).ToArray();
                        return iquestion;
                    }).ToArray();
                    return iticket;
                }).ToArray();

                if(parameters.Shuffle) {
                    Random rand = new Random();
                    foreach(var iticket in itickets) {
                        iticket.Questions = iticket.Questions.OrderBy(iquestion => rand.Next()).ToArray();
                    }
                }
                return itickets;
            } catch {
                throw;
            } finally {
                if(dataAccessor != null) {
                    dataAccessor.Dispose();
                }
            }
        }
    }
}
