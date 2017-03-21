using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseGenerator.DataBase {
    public class jsonModel {
        public class jsonProgress {
            public int count { get; set; }
            public int status { get; set; }
        }

        public class jsonAnswer {
            public int orderNumber { get; set; }
            public string text { get; set; }
            public bool isRight { get; set; }
        }

        public class jsonQuestion {
            public int orderNumber { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string hint { get; set; }
            public object userAnswer { get; set; }
            public List<jsonAnswer> answers { get; set; }
        }

        public class jsonData {
            public bool examMode { get; set; }
            public int type { get; set; }
            public object leftTime { get; set; }
            public List<jsonProgress> progress { get; set; }
            public int testLength { get; set; }
            public jsonQuestion question { get; set; }
        }

        public class jsonRootObject {
            public bool result { get; set; }
            public jsonData data { get; set; }
        }
    }
}
