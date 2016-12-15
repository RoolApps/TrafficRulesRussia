using AppLogic.Enums;
using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public static class SessionFactory
    {
        public static ParametersValidationResult CreateSession(ISessionParameters parameters, out ISession session)
        {
            ParametersValidator validator = new ParametersValidator(parameters);
            var validationResult = validator.Validate();
            if(validationResult == ParametersValidationResult.Valid)
            {
                session = new Session(parameters);
                System.Diagnostics.Debug.WriteLine("validtionResult: {0}", validationResult);
            }
            else
            {
                session = null;
            }
            return validationResult;
        }

        public static SessionRestoreResult RestoreSession(byte[] sessionData, out ISession session)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        class ParametersValidator
        {
            ISessionParameters parameters;

            internal ParametersValidator(ISessionParameters parameters)
            {
                this.parameters = parameters;
            }

            internal ParametersValidationResult Validate()
            {
                if (parameters == null)
                {
                    return ParametersValidationResult.ParamsNull;
                }
                else if(parameters.Mode == QuestionsGenerationMode.RandomTicket)
                {
                    return ParametersValidationResult.Valid;
                }
                else if(parameters.Mode == QuestionsGenerationMode.ExamTicket) {
                    return ParametersValidationResult.Valid;
                }
                else if(parameters.Mode == QuestionsGenerationMode.Questions)
                {
                    return parameters.Questions == null ? ParametersValidationResult.NoTickets : ParametersValidationResult.Valid;
                }
                else if(parameters.TicketNums == null || !parameters.TicketNums.Any())
                {
                    return ParametersValidationResult.NoTickets;
                }
                else if(parameters.TicketNums.GroupBy(ticketNum => ticketNum).Select(group => group.Count()).Any(count => count > 1))
                {
                    return ParametersValidationResult.DuplicateTickets;
                }
                else
                {
                    return ParametersValidationResult.Valid;
                }
            }
        }
    }
}
