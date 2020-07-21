using CalendarWeekView.Components;
using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt;
using TaskBarExt.Native;
using TaskBarExt.Utils;

namespace CalendarWeekView.Services
{
    class DefaultTaskbarWindowService : ITaskbarWindowService
    {
        static ITaskbarWindowService _hookInstance;

        IAppSettings Settings { get; }
        IDialogService DialogService { get; set; }

        static List<TaskbarRef> taskbars;
        List<WeekWindow> windows = new List<WeekWindow>();

        TaskbarWindowPlacement ActivePlacement;

        static WinEventHook.WinEventDelegate hookDelegate;

        public DefaultTaskbarWindowService(IAppSettings settings)
        {
            _hookInstance = this;

            Settings = settings;
            ActivePlacement = settings.Placement;            

            // install a win event hook to track taskbar resize/movement
            hookDelegate = new WinEventHook.WinEventDelegate(WinEventProc);
            var winHook = WinEventHook.SetHook(WinEventHook.EVENT_OBJECT_LOCATIONCHANGE, hookDelegate);

            Application.ApplicationExit += (s, e) =>
                    {
                        if (winHook != IntPtr.Zero)
                        {
                            WinEventHook.RemoveHook(winHook);
                            winHook = IntPtr.Zero;

                            foreach (var f in GetWindows())
                            {
                                f.Close();
                                f.RestoreTaskbar();
                            }
                        }
                    };
        }

        public void SetDialogService(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        public List<TaskbarRef> GetTaskBars()
        {
            if (taskbars == null)
            {
                taskbars = TaskbarUtils.ListTaskbars().ToList();
            }

            return taskbars;
        }

        public List<WeekWindow> GetWindows()
        {
            return windows;
        }

        public void InvalidateAll()
        {
            if (ActivePlacement != Settings.Placement)
            {
                RecreateAll();
            }
            else
            {
                foreach (var w in windows)
                {
                    (w.TaskbarComponent as CalendarWeekComponent)?.UpdateRenderer();
                }
            }
        }

        public void Register(WeekWindow window)
        {
            windows.Add(window);
        }

        public void Deregister(WeekWindow window)
        {
            windows.Remove(window);
        }

        public void RecreateAll()
        {
            taskbars = TaskbarUtils.ListTaskbars().ToList();

            ActivePlacement = Settings.Placement;

            // recreate all windows
            foreach (var w in windows)
            {
                w.RestoreTaskbar();
                w.Close();
            }

            windows.Clear();

            // add the component to each taskbar (primary and secondary)
            foreach (var taskbar in taskbars)
            {
                var f = new WeekWindow(taskbar, Settings, DialogService);
                f.Show();

                Register(f);
            }
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND namechanges
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            // find the taskbar this event belongs to
            foreach (var taskbar in _hookInstance.GetTaskBars().Where(x => x.Handle == hwnd || x.ObservedChildBoundChanges.ContainsKey(hwnd)))
            {
                taskbar.Update(taskbar.Handle == hwnd ? IntPtr.Zero : hwnd);
            }
        }
    }
}
