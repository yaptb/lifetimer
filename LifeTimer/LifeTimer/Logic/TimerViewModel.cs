using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeTimer.Logic
{

    internal class TimerDefinition
    {
        public string Title { get; set; }  = string.Empty;
        public DateTime TargetDateTime { get; set; } = DateTime.Now;
        public bool DisplayDaysOnly { get; set; }
        public bool DisplaySeconds { get; set; }
    }
}
