using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace LifeTimer.Logic.Models
{
    public class PomodoroViewModel
    {
        public int PomodoroMinutes { get; set; } = 20;
        public string PomodoroTitle { get; set; } = "Pomodoro";
        public string PomodoroFinsished { get; set; } = "Finished";
    }
}
