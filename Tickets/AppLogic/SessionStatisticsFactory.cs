using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Interfaces;
using AppLogic.Enums;

namespace AppLogic
{
    public static class SessionStatisticsFactory
    {
        public static SessionValidationResult CreateSessionStatistics(ISession session, out ISessionStatistics sessionStatistics)
        {
            sessionStatistics = null;
            if(session == null)
            {
                return SessionValidationResult.SessionNull;
            }
            else if(session.Tickets == null || !session.Tickets.Any())
            {
                return SessionValidationResult.NoTickets;
            } 
            else
            {
                var questions = session.Questions();
                if(questions == null || !questions.Any())
                {
                    return SessionValidationResult.NoQuestions;
                }
                else
                {
                    sessionStatistics = new SessionStatistics(session);
                    return SessionValidationResult.Valid;
                }
            }
        }
    }
}
