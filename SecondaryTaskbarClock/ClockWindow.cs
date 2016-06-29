using SecondaryTaskbarClock.Native;
using SecondaryTaskbarClock.Renderers;
using SecondaryTaskbarClock.Utils;
using SecondaryTaskbarClock.ViewModels;
using SecondaryTaskbarClock.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace SecondaryTaskbarClock
{
    public class ClockWindow : TaskbarWindow
    {
        ClockViewModel ViewModel { get; set; }
        ToolTip toolTip;
        bool tooltipShown = false;
                
        public ClockWindow(TaskbarInfo targetTaskbar, ClockViewModel viewModel)
            // currently we always use the Windows 10 renderer
            : base(targetTaskbar, new Win10TaskbarClockRenderer(viewModel))
        {
            ViewModel = viewModel;

            // redraw the window, when the current time changes
            ViewModel.PropertyChanged += (s, e) => Invalidate();
            
            // when clicked, open teh calendar flyout
            MouseClick += ClockWindow_MouseClick;

            // the tool tip which contains a long version of the day including Weekday
            toolTip = new ToolTip();
            toolTip.Active = true;            
            toolTip.UseFading = false;

            // we have to show the tooltip manually for correct positioning
            MouseEnter += ClockWindow_MouseEnter;
            MouseLeave += ClockWindow_MouseLeave;            
        }

        private void ClockWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Task.Run(() =>
                {
                    TaskbarUtils.ShowCalendarFlyOut(this.Bounds, Taskbar.DockPosition.GetCorrespondingFlyoutPosition());
                });
            }
        }

        private void ClockWindow_MouseEnter(object sender, EventArgs e)
        {
            // we need the focus to show a tooltip
            this.Focus();

            if (!tooltipShown)
            {
                tooltipShown = true;                
                // we have to use the Show() method of the tooltip, since otherwise
                // it will always be positioned at the exact mouse position and not
                // next to the window
                toolTip.Show(ViewModel.CurrentDateTime.ToLongDateString(), this, 5000);
            }
        }

        private void ClockWindow_MouseLeave(object sender, EventArgs e)
        {            
            tooltipShown = false;
        }
    }
}
