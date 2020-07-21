using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Utils;

namespace CalendarWeekView.Services
{
    public interface IAppSettings
    {
        TaskbarWindowPlacement Placement { get; set; }

        bool Autostart { get; set; }

        Font DisplayFont { get; set; }        

        Color FontColor { get; set; }

        string DisplayFormatString { get; set; }        

        CalendarWeekCalculationRule CalendarWeekRule { get; set; }

        void Save();        
    }
}
