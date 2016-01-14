using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public class TestController
    {
        #region Private Members
        TestParams Parameters { get; set; }
        IDataAccessor DataAccessor { get; set; }
        TestProgress Progress { get; set; }
        #endregion

        #region Constructor
        internal TestController(TestParams parameters)
        {
            Parameters = parameters;
            DataAccessor = GetDataAccessor();
        }
        #endregion

        #region Public Methods
        public void NextQuestion()
        {
            Progress.NextQuestion();
        }

        public void PreviousQuestion()
        {
            Progress.PreviousQuestion();
        }

        public void AnswerQuestion(IAnswer answer)
        {
            Progress.SelectAnswer(answer);
        }
        #endregion

        #region Internal Methods
        internal ParamsValidationResult ValidateParameters()
        {
            if (Parameters == null)
            {
                return ParamsValidationResult.ParamsNull;
            }

            var ticketNums = DataAccessor.GetTicketNums();

            switch (Parameters.Mode)
            {
                case TestMode.RandomTicket:
                    return ParamsValidationResult.Valid;
                case TestMode.SelectedTickets:
                    if (!Parameters.TicketNums.Any())
                    {
                        return ParamsValidationResult.NoTickets;
                    }
                    if (Parameters.TicketNums.GroupBy(ticketNum => ticketNum).Select(group => group.Count()).Any(count => count > 1))
                    {
                        return ParamsValidationResult.DuplicateTickets;
                    }
                    return ParamsValidationResult.Valid;
                default:
                    throw new NotImplementedException(String.Format("Selected test mode \"{0}\" is not implemented yet", Parameters.Mode.ToString()));
            }
        }

        internal void StartTest()
        {
            Progress = new TestProgress(DataAccessor, Parameters);
        }
        #endregion

        #region Private Methods
        private IDataAccessor GetDataAccessor()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
