using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SecondaryTaskbarClock.ViewModels
{
    public class ClockViewModel : ViewModelBase
    {
        Timer timer;        

        public DateTime CurrentDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public ClockViewModel()
        {
            timer = new Timer(450);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Signal that the time has changed
            OnPropertyChanged("CurrentDateTime");
        }
    }
}
