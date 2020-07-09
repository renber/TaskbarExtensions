using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt;
using TaskBarExt.Native;
using TaskBarExt.Utils;

namespace CalendarWeekView
{
    static class Program
    {
        static List<TaskbarRef> taskbars;
        static List<TaskbarWindow> windows;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // create a clock window for each secondary taskbar
            taskbars = TaskbarUtils.ListTaskbars().ToList();

            if (taskbars.Count > 0)
            {
                windows = new List<TaskbarWindow>();
                // add a clock to each secondary taskbar
                //var viewModel = new ViewModels.ClockViewModel();
                foreach (var taskbar in taskbars)
                {
                    var f = new WeekWindow(taskbar);
                    f.Show();
                    windows.Add(f);
                }

                // install a win event hook to track taskbar resize/movement
                var hook = WinEventHook.SetHook(WinEventHook.EVENT_OBJECT_LOCATIONCHANGE, WinEventProc);

                Application.ApplicationExit += (s, e) =>
                {
                    WinEventHook.RemoveHook(hook);

                    foreach(var f in windows)
                    {
                        f.Close();
                        f.RestoreTaskbar();
                    }
                };

                Application.Run();
            }
            else
                MessageBox.Show("CalendarWeekView", "No taskbars found. Application will terminate.");
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND namechanges
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            // if this event belongs to a taskbar, update its Bounds and DockPosition
            //taskbars.FirstOrDefault(x => )?.Update();

            foreach (var taskbar in taskbars.Where(x => x.Handle == hwnd || x.ObservedChildBoundChanges.ContainsKey(hwnd)))
            {
                taskbar.Update(taskbar.Handle == hwnd ? IntPtr.Zero : hwnd);
            }
        }
    }
}
