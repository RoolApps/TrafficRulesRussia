using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Session : ISession
    {
        #region Constructor
        internal Session(ISessionParameters parameters)
        {
            LoadQuestions(parameters);
        }
        #endregion

        #region Public Methods
        public IEnumerable<IQuestion> Questions { get; private set; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void LoadQuestions(ISessionParameters parameters)
        {
            int[] selectedTicketIds;
            if(parameters.Mode == Enums.QuestionsGenerationMode.RandomTicket)
            {
                var ticketIds = Queries.GetTicketIds();
                Random rand = new Random();
                selectedTicketIds = new int[] { ticketIds.ElementAt(rand.Next(ticketIds.Count())) };
            }
            else if (parameters.Mode == Enums.QuestionsGenerationMode.SelectedTickets)
            {
                selectedTicketIds = Queries.GetTicketIdsByNums(parameters.TicketNums).ToArray();
            }
            else
            {
                throw new NotImplementedException(String.Format("Not supported mode", parameters.Mode));
            }
            var questions = Queries.GetQuestionsByTicketIds(selectedTicketIds);
            var answers = Queries.GetAnswersByQuestionIds(questions.Select(question => question.id));
            Questions = questions.Select(question =>
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
        }
        #endregion
    }
}
