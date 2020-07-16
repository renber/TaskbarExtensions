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
        AppSettings Settings { get; }
        IDialogService DialogService { get; set; }

        static List<TaskbarRef> taskbars;
        List<WeekWindow> windows = new List<WeekWindow>();

        TaskbarWindowPlacement ActivePlacement;

        public DefaultTaskbarWindowService(AppSettings settings)
        {
            Settings = settings;
            ActivePlacement = settings.Placement;            
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
    }
}
