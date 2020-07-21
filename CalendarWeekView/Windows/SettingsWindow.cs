using CalendarWeekView.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarWeekView.Windows
{
    partial class SettingsWindow : Form
    {
        SettingsViewModel DataContext { get; }

        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            SetupBindings();
            SetupCommands();

            viewModel.ViewCloseRequested += ViewModel_ViewCloseRequested;
        }

        protected void SetupBindings()
        {            
            comboPlacement.DataSource = new BindingSource(DataContext, "AvailablePlacements");
            comboPlacement.DataBindings.Add("SelectedItem", DataContext, "SelectedPlacement");
            
            cbAutostart.DataBindings.Add("Checked", DataContext, "Autostart");

            comboWeekRule.DataSource = new BindingSource(DataContext, "AvailableWeekRules");
            comboWeekRule.DataBindings.Add("SelectedItem", DataContext, "SelectedWeekRule");

            lblFontSample.DataBindings.Add("Text", DataContext, "DisplayFontDescription");
            panelFontColor.DataBindings.Add("BackColor", DataContext, "DisplayFontColor");

            txtDisplayFormat.DataBindings.Add("Text", DataContext, "DisplayFormatString");
        }

        protected void SetupCommands()
        {
            btnApply.Click += (s, e) => DataContext.ApplyCommand.Invoke();
            btnOk.Click += (s, e) => DataContext.OkCommand.Invoke();
            btnCancel.Click += (s, e) => DataContext.CancelCommand.Invoke();
            btnChangeFont.Click += (s, e) => DataContext.ChangeFontCommand.Invoke();
        }

        private void ViewModel_ViewCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DataContext.ViewCloseRequested -= ViewModel_ViewCloseRequested;
        }
    }
}
