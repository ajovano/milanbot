using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilanBot
{
    public class AdoItem
    {
        public string Title { get; set; }
        public string ID { get; set; }
        public string BranchName { get; set; }
        public TimeSpan Cost { get; set; }

        public AdoItem(string title, string iD)
        {
            Title = title;
            ID = iD;
        }
    }
}
