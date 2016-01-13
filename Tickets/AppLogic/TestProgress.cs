using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    public class TestProgress
    {
        #region Private Members
        private enum Direction { Forward, Back };
        private event EventHandler<Direction> CurrentQuestionChanged;
        private object linkedListLock = new object();

        private IDataAccessor DataAccessor { get; set; }
        private LinkedList<int> QuestionsToLoad { get; set; }
        private LinkedListNode<int> LeftBoundary { get; set; }
        private LinkedListNode<int> RightBoundary { get; set; }
        private LinkedList<IQuestion> Questions { get; set; }
        private LinkedListNode<IQuestion> CurrentQuestion { get; set; }

        private const int Paging = 10;
        #endregion

        #region Constructor
        internal TestProgress(IDataAccessor dataAccessor, TestParams parameters)
        {
            DataAccessor = dataAccessor;
            var questions = dataAccessor.GetQuestionsByTickets(parameters.TicketNums);
            if(parameters.Shuffle)
            {
                Random rand = new Random((int)DateTime.Now.ToBinary());
                QuestionsToLoad = new LinkedList<int>(questions.OrderBy(questionId => rand.Next()));
            }
            else
            {
                QuestionsToLoad = new LinkedList<int>(questions);
            }
            Questions = new LinkedList<IQuestion>();
            LoadQuestions();
            CurrentQuestion = Questions.First;
            CurrentQuestionChanged += LoadQuestionsAsync;
        }
        #endregion

        #region Internal Methods
        internal void NextQuestion()
        {
            if(CurrentQuestion != Questions.Last)
            {
                CurrentQuestion = CurrentQuestion.Next;
                if(CurrentQuestionChanged != null)
                {
                    CurrentQuestionChanged(this, Direction.Forward);
                }
            }
        }

        internal void PreviousQuestion()
        {
            if(CurrentQuestion != Questions.First)
            {
                CurrentQuestion = CurrentQuestion.Previous;
                if(CurrentQuestionChanged != null)
                {
                    CurrentQuestionChanged(this, Direction.Back);
                }
            }
        }

        internal void SelectAnswer(IAnswer answer)
        {
            throw new NotImplementedException();
            CurrentQuestion.Value.SelectedAnswer = answer;
            NextQuestion();
        }
        #endregion

        #region Private Methods
        private void LoadQuestions()
        {
            var question = QuestionsToLoad.First;
            if(question != null)
            {
                while (Questions.Count < Paging || question.Next != null)
                {
                    Questions.AddLast(DataAccessor.GetQuestionById(question.Value));
                    question = question.Next;
                }
                Questions.AddLast(DataAccessor.GetQuestionById(question.Value));
            }
            LeftBoundary = QuestionsToLoad.First;
            RightBoundary = QuestionsToLoad.Last;
        }

        private async void LoadQuestionsAsync(object sender, Direction direction)
        {
            await Task.Run(() =>
            {
                lock(linkedListLock)
                {
                    switch(direction)
                    {
                        case Direction.Forward:
                            if(CurrentQuestion.ElementsBefore() > Paging)
                            {
                                Questions.RemoveFirst();
                            }
                            if(RightBoundary.Next != null)
                            {
                                RightBoundary = RightBoundary.Next;
                            }
                            if(LeftBoundary.Next != null && LeftBoundary.ElementsAfter() > Paging)
                            {
                                LeftBoundary = LeftBoundary.Next;
                                Questions.AddFirst(DataAccessor.GetQuestionById(LeftBoundary.Value));
                            }
                            break;
                        case Direction.Back:
                            if(CurrentQuestion.ElementsAfter() > Paging)
                            {
                                Questions.RemoveLast();
                            }
                            if(RightBoundary.Previous != null && RightBoundary.ElementsBefore() > Paging)
                            {
                                RightBoundary = RightBoundary.Previous;
                                Questions.AddLast(DataAccessor.GetQuestionById(RightBoundary.Value));
                            }
                            if(LeftBoundary.Previous != null)
                            {
                                LeftBoundary = LeftBoundary.Previous;
                            }
                            break;
                        default:
                            throw new ArgumentException("Unexpected direction");
                    }
                }
            });
        }
        #endregion
    }

    #region Additional Classes
    static class LinkedListExtensions
    {
        public static int ElementsBefore<T>(this LinkedListNode<T> node)
        {
            int counter = 0;
            var currentNode = node.Previous;
            while(currentNode != null)
            {
                counter++;
                currentNode = currentNode.Previous;
            }
            return counter;
        }

        public static int ElementsAfter<T>(this LinkedListNode<T> node)
        {
            int counter = 0;
            var currentNode = node.Next;
            while (currentNode != null)
            {
                counter++;
                currentNode = currentNode.Next;
            }
            return counter;
        }
    }
    #endregion
}
