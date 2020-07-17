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
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;

            AppSettings settings = new AppSettings();
            taskbarService = new DefaultTaskbarWindowService(settings);
            IDialogService dialogService = new DialogService(settings, taskbarService);
            (taskbarService as DefaultTaskbarWindowService).SetDialogService(dialogService);
            
            var taskbars = taskbarService.GetTaskBars();
            if (taskbars.Count > 0)
            {
                taskbarService.RecreateAll();                

                Application.Run();
            }
            else
                MessageBox.Show("No taskbars found. Application will terminate.", "CalendarWeekView", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("An exception occured: " + e.Exception.Message, "CalendarWeekView", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }        
    }
}
