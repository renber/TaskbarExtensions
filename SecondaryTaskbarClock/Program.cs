using SecondaryTaskbarClock.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondaryTaskbarClock
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // create a clock window for each secondary taskbar
            var taskbars = TaskbarUtils.ListTaskbars().Where(x => !x.IsPrimary).ToList();
            if (taskbars.Count > 0)
            {
                // add a clock to each secondary taskbar
                var viewModel = new ViewModels.ClockViewModel();

                foreach (var taskbar in taskbars)
                {
                    var f = new ClockWindow(taskbar, viewModel);
                    f.Show();
                }

                Application.Run();
            }
            else
                MessageBox.Show("No secondary taskbars found.");            
        }
    }
}
