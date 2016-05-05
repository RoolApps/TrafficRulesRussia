using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteShared;
using SQLiteShared.Models;
using AppLogic.Interfaces;

namespace AppLogic
{
    class TicketLoader : ITicketLoader
    {
        public IEnumerable<ITicket> LoadTickets(ISessionParameters parameters)
        {
            SQLiteDataAccessor dataAccessor = null;
            try
            {
                dataAccessor = new SQLiteDataAccessor();

                Tickets[] selectedTickets;
                if (parameters.Mode == Enums.QuestionsGenerationMode.RandomTicket)
                {
                    var tickets = dataAccessor.CreateQuery<Tickets>();
                    Random rand = new Random();
                    selectedTickets = new Tickets[] { tickets.ElementAt(rand.Next(tickets.Count())) };
                }
                else if (parameters.Mode == Enums.QuestionsGenerationMode.SelectedTickets)
                {
                    selectedTickets = dataAccessor.CreateQuery<Tickets>().Where(ticket => parameters.TicketNums.Contains(ticket.num)).ToArray();
                }
                else
                {
                    throw new NotImplementedException(String.Format("Not supported mode", parameters.Mode));
                }

                var questions = dataAccessor.CreateQuery<Questions>().Where(question => selectedTickets.Any(ticket => ticket.id == question.ticket_id));
                var questionIds = questions.Select(question => question.id).ToArray();
                var answers = dataAccessor.CreateQuery<Answers>().Where(answer => questionIds.Contains(answer.question_id));

                var itickets = selectedTickets.Select(ticket =>
                {
                    var iticket = new Ticket
                    {
                        Number = ticket.num
                    };
                    iticket.Questions = questions.Where(question => question.ticket_id == ticket.id).Select(question =>
                    {
                        var iquestion = new Question
                        {
                            Ticket = iticket,
                            Number = question.num,
                            Image = question.image,
                            Text = question.question
                        };
                        iquestion.Answers = answers.Where(answer => answer.question_id == question.id).Select(answer =>
                        {
                            var ianswer = new Answer()
                            {
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
                return itickets;
            }
            catch
            {
                throw;
            }
            finally
            {
                if(dataAccessor != null)
                {
                    dataAccessor.Dispose();
                }
            }
        }
    }
}
