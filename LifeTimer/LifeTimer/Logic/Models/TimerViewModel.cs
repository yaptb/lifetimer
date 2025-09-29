using System;

namespace LifeTimer.Logic.Models
{

    public class TimerDefinition
    {
        public string Title { get; set; } = string.Empty;
        public DateTime TargetDateTime { get; set; } = DateTime.Now;
        public bool DisplayDaysOnly { get; set; }
        public bool DisplayHours { get; set; }
        public bool DisplayMinutes { get; set; }
        public bool DisplaySeconds { get; set; }
    }
}
