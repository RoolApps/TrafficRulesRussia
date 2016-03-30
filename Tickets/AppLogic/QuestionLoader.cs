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
    class QuestionLoader : IQuestionLoader
    {
        public IEnumerable<IQuestion> LoadQuestions(ISessionParameters parameters)
        {
            SQLiteDataAccessor dataAccessor = null;
            try
            {
                dataAccessor = new SQLiteDataAccessor();

                int[] selectedTicketIds;
                if (parameters.Mode == Enums.QuestionsGenerationMode.RandomTicket)
                {
                    var ticketIds = dataAccessor.CreateQuery<Tickets>().Select(ticket => ticket.id);
                    Random rand = new Random();
                    selectedTicketIds = new int[] { ticketIds.ElementAt(rand.Next(ticketIds.Count())) };
                }
                else if (parameters.Mode == Enums.QuestionsGenerationMode.SelectedTickets)
                {
                    selectedTicketIds = dataAccessor.CreateQuery<Tickets>().Where(ticket => parameters.TicketNums.Contains(ticket.num)).Select(ticket => ticket.id).ToArray();
                }
                else
                {
                    throw new NotImplementedException(String.Format("Not supported mode", parameters.Mode));
                }

                var questions = dataAccessor.CreateQuery<Questions>().Where(question => selectedTicketIds.Contains(question.ticket_id));
                var questionIds = questions.Select(question => question.id).ToArray();
                var answers = dataAccessor.CreateQuery<Answers>().Where(answer => questionIds.Contains(answer.questions_id));

                var iquestions = questions.Select(question =>
                {
                    return new Question()
                    {
                        Answers = answers.Where(answer => answer.questions_id == question.id).Select(answer =>
                        {
                            return new Answer()
                            {
                                IsRight = answer.is_right,
                                Text = answer.answer
                            };
                        }),
                        Image = question.image,
                        Text = question.question
                    };
                }).ToArray(); //TODO: check if we really need toArray here

                return iquestions;
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
