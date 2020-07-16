using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt.Utils;

namespace CalendarWeekView.Types
{     

    static class ReadableEnumDisplayTexts
    {
        public static string GetDisplayText(this TaskbarWindowPlacement p)
        {
            switch (p)
            {
                case TaskbarWindowPlacement.RightOfTaskButtons: return "Left of taskbar notifications icons";
                case TaskbarWindowPlacement.BetweenTrayAndClock: return "Between taskbar notification icons and clock";
                case TaskbarWindowPlacement.EndOfTaskbar: return "At the end of the task bar";
                default: return p.ToString();
            }
        }

        public static string GetDisplayText(this CalendarWeekCalculationRule rule)
        {
            switch(rule)
            {
                case CalendarWeekCalculationRule.ISO8601: return "ISO 8601";
                case CalendarWeekCalculationRule.US: return "United States";
                default: return rule.ToString();
            }
        }
    }
}
