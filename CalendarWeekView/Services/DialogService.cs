using CalendarWeekView.Types;
using CalendarWeekView.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarWeekView.Services
{
    class DialogService : IDialogService
    {
        protected IAppSettings Settings { get; }

        protected ITaskbarWindowService TaskbarWindowService { get; }

        protected SettingsWindow OpenedSettingsWindow { get; set; }

        public DialogService(IAppSettings settings, ITaskbarWindowService taskbarWindowService)
        {
            Settings = settings;
            TaskbarWindowService = taskbarWindowService;
        }

        public void ShowSettingsDialog()
        {
            if (OpenedSettingsWindow == null)
            {
                OpenedSettingsWindow = new SettingsWindow(new ViewModels.SettingsViewModel(Settings, TaskbarWindowService, this));
                OpenedSettingsWindow.FormClosed += OpenedSettingsWindow_FormClosed;
                OpenedSettingsWindow.Show();
            } else
            {
                OpenedSettingsWindow.BringToFront();
            }
        }

        private void OpenedSettingsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (OpenedSettingsWindow != null)
            {
                OpenedSettingsWindow.FormClosed -= OpenedSettingsWindow_FormClosed;
            }
            OpenedSettingsWindow = null;
        }

        public bool ShowFontDialog(Font initialFont, Color initialColor, out Tuple<Font, Color> selection)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true;
            fontDialog.Font = initialFont;
            fontDialog.Color = initialColor;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                selection = new Tuple<Font, Color>(fontDialog.Font, fontDialog.Color);
                return true;
            }

            selection = null;
            return false;
        }

    }
}
