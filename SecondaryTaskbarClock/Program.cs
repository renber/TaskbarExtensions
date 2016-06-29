using SecondaryTaskbarClock.Native;
using SecondaryTaskbarClock.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondaryTaskbarClock
{
    static class Program
    {
        static List<TaskbarRef> taskbars;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            

            // create a clock window for each secondary taskbar
            taskbars = TaskbarUtils.ListTaskbars().Where(x => !x.IsPrimary).ToList();            

            if (taskbars.Count > 0)
            {
                // add a clock to each secondary taskbar
                var viewModel = new ViewModels.ClockViewModel();
                foreach (var taskbar in taskbars)
                {
                    var f = new ClockWindow(taskbar, viewModel);
                    f.Show();
                }

                // install a win event hook to track taskbar resize/movement
                var hook = WinEventHook.SetHook(WinEventHook.EVENT_OBJECT_LOCATIONCHANGE, WinEventProc);

                Application.ApplicationExit += (s, e) =>
                {
                    WinEventHook.RemoveHook(hook);
                };

                Application.Run();                
            }
            else
                MessageBox.Show("SecondaryTaskbarClock", "No secondary taskbars found. Application will terminate.");            
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND namechanges
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            // if this event belongs to a taskbar, update its Bounds and DockPosition
            taskbars.FirstOrDefault(x => x.Handle == hwnd)?.Update();
        }
    }
}
