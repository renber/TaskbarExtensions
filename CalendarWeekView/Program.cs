using CalendarWeekView.Services;
using CalendarWeekView.Types;
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
        // declare as field to keep the WinHook delegate in memory
        static ITaskbarWindowService taskbarService;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppSettings settings = new AppSettings();
            taskbarService = new DefaultTaskbarWindowService(settings);
            IDialogService dialogService = new DialogService(settings, taskbarService);
            (taskbarService as DefaultTaskbarWindowService).SetDialogService(dialogService);
            
            var taskbars = taskbarService.GetTaskBars();
            if (taskbars.Count > 0)
            {
                taskbarService.RecreateAll();

                // install a win event hook to track taskbar resize/movement
                var winHook = WinEventHook.SetHook(WinEventHook.EVENT_OBJECT_LOCATIONCHANGE, WinEventProc);

                Application.ApplicationExit += (s, e) =>
                {
                    if (winHook != IntPtr.Zero)
                    {
                        WinEventHook.RemoveHook(winHook);
                        winHook = IntPtr.Zero;

                        foreach (var f in taskbarService.GetWindows())
                        {
                            f.Close();
                            f.RestoreTaskbar();
                        }
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

            // find the taskbar this event belongs to
            foreach (var taskbar in taskbarService.GetTaskBars().Where(x => x.Handle == hwnd || x.ObservedChildBoundChanges.ContainsKey(hwnd)))
            {
                taskbar.Update(taskbar.Handle == hwnd ? IntPtr.Zero : hwnd);
            }
        }
    }
}
