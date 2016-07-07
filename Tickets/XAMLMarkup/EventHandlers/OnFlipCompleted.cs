using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAMLMarkup.Enums;

namespace XAMLMarkup.EventHandlers {

    public class OnFlipCompleted : EventArgs {
        #region private Members
        private MoveDirection direction;
        #endregion

        #region public Properties
        public MoveDirection Direction {
            get {
                return direction;
            }
            private set {
                direction = value;
            }
        }
        #endregion

        #region Constructor
        public OnFlipCompleted(MoveDirection d) {
            direction = d;
        }
        #endregion
    }
}
