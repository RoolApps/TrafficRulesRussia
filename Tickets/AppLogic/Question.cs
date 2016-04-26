using AppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogic
{
    class Question : IQuestion
    {
        public byte[] Image { get; internal set; }

        private IEnumerable<IAnswer> answers = null;
        public IEnumerable<IAnswer> Answers 
        {
            get
            {
                return answers;
            }
            internal set
            {
                if(value != answers)
                {
                    if(answers != null)
                    {
                        foreach (var answer in answers)
                        {
                            answer.IsSelectedChanged -= answer_IsSelectedChanged;
                        }
                    }
                    answers = value;
                    foreach(var answer in answers)
                    {
                        answer.IsSelectedChanged += answer_IsSelectedChanged;
                    }
                }
            }
        }

        void answer_IsSelectedChanged(object sender, IsSelectedChangedEventArgs e)
        {
            if(e.IsSelected == true)
            {
                foreach(var answer in Answers)
                {
                    if(answer != e.Answer)
                    {
                        answer.IsSelected = false;
                    }
                }
            }
        }

        public bool IsAnswered 
        {
            get
            {
                return Answers.Any(answer => answer.IsSelected);
            }
        }

        public String Text { get; internal set; }
    }
}
