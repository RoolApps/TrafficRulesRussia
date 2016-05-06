using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogic.Interfaces;

namespace AppLogic
{
    public class SessionStatistics : ISessionStatistics
    {
        private const int PercentageMultiplier = 100;

        public int TakenTickets { get; private set; }
        public int PassedTickets { get; private set; }
        public int PassedTicketsPercentage { get; private set; }

        public int TakenQuestions { get; private set; }
        public int PassedQuestions { get; private set; }
        public int PassedQuestionsPercentage { get; private set; }

        internal SessionStatistics(ISession session)
        {
            TakenTickets = session.Tickets.Count();
            PassedTickets = session.PassedTickets().Count();
            PassedTicketsPercentage = (PassedTickets * PercentageMultiplier) / TakenTickets;

            TakenQuestions = session.Questions().Count();
            PassedQuestions = session.PassedQuestions().Count();
            PassedQuestionsPercentage = (PassedQuestions * PercentageMultiplier) / TakenQuestions;
        }
    }

    static class SessionStatisticsExtensions
    {
        private const int PercentageMultiplier = 100;
        private const int TicketPassPercentage = 90;

        public static IEnumerable<ITicket> PassedTickets(this ISession session)
        {
            return session.Tickets.Where(TicketPassedRule);
        }

        public static IEnumerable<IQuestion> Questions(this ISession session)
        {
            return session.Tickets.Where(ticket => ticket.Questions != null).SelectMany(ticket => ticket.Questions);
        }

        public static IEnumerable<IQuestion> PassedQuestions(this ISession session)
        {
            return session.Questions().Where(QuestionPassedRule);
        }


        private static Func<ITicket, bool> ticketPassedRule;
        private static Func<ITicket, bool> TicketPassedRule
        {
            get
            {
                return ticketPassedRule ?? (ticketPassedRule = ticket =>
                    (ticket.Questions.Where(QuestionPassedRule).Count() * PercentageMultiplier) / ticket.Questions.Count() > TicketPassPercentage);
            }
        }

        private static Func<IQuestion, bool> questionPassedRule;
        private static Func<IQuestion, bool> QuestionPassedRule
        {
            get
            {
                return questionPassedRule ?? (questionPassedRule = question =>
                    question.SelectedAnswered != null && question.SelectedAnswered.IsRight);
            }
        }
    }
}
