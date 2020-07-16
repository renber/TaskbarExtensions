using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Utils;

namespace CalendarWeekView.Types
{
    public class AppSettings
    {

        public Font DisplayFont
        {
            get => Properties.Settings.Default.DisplayFont;
            set => Properties.Settings.Default.DisplayFont = value;
        }       

        public Color FontColor
        {
            get => Properties.Settings.Default.FontColor;
            set => Properties.Settings.Default.FontColor = value;
        }

        public string DisplayFormatString
        {
            get => Properties.Settings.Default.DisplayFormatString;
            set => Properties.Settings.Default.DisplayFormatString = value;
        }

        public TaskbarWindowPlacement Placement
        {
            get => Properties.Settings.Default.Placement;
            set => Properties.Settings.Default.Placement = value;
        }

        public CalendarWeekCalculationRule CalendarWeekRule
        {
            get => Properties.Settings.Default.CalendarWeekRule;
            set => Properties.Settings.Default.CalendarWeekRule = value;
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
