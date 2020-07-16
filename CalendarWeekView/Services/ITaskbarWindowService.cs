using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt;
using TaskBarExt.Utils;

namespace CalendarWeekView.Services
{
    /// <summary>
    /// Manages taskbar extension windows
    /// </summary>
    interface ITaskbarWindowService
    {
        List<TaskbarRef> GetTaskBars();

        List<WeekWindow> GetWindows();

        void RecreateAll();

        void InvalidateAll();

    }
}
