using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteShared.Models;

namespace AppLogic.Static
{
    public static class PreloadedContent
    {
        public static PreloadedTable<Chapters> RuleChapters;
        public static PreloadedTable<Signs> Signs;
        public static PreloadedTable<Marks> Marks;

        public static void LoadData()
        {
            RuleChapters = new PreloadedTable<Chapters>();
            Signs = new PreloadedTable<Signs>();
            Marks = new PreloadedTable<Marks>();
        }
    }
}
