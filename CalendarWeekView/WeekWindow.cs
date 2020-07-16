using CalendarWeekView.Components;
using CalendarWeekView.Renderers;
using CalendarWeekView.Services;
using CalendarWeekView.Types;
using CalendarWeekView.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt;
using TaskBarExt.Utils;

namespace CalendarWeekView
{
    public partial class WeekWindow : TaskbarWindow
    {
        IDialogService DialogService { get; }

        public WeekWindow(TaskbarRef targetTaskbar, AppSettings settings, IDialogService dialogService)
            : base(targetTaskbar, settings.Placement, new CalendarWeekComponent(settings))
        {
            DialogService = dialogService;

            InitializeComponent();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void WeekWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                contextMenu.Show(this, 0, 0);                
            }            
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogService.ShowSettingsDialog();
        }
    }
}
