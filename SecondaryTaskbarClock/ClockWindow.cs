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

namespace SecondaryTaskbarClock
{
    public class ClockWindow : TaskbarWindow
    {
                
        public ClockWindow(TaskbarInfo targetTaskbar, ClockViewModel viewModel)
            // currently we always use the Windows 10 renderer
            : base(targetTaskbar, new Win10TaskbarClockRenderer(viewModel))
        {                                   
            // redraw the window, when the current time changes
            viewModel.PropertyChanged += (s, e) => Invalidate();
            
            // when clicked, open teh calendar flyout
            MouseClick += ClockWindow_MouseClick;   
        }

        private void ClockWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Task.Run(() =>
                {
                    TaskbarUtils.ShowCalendarFlyOut(this.Bounds, (FlyoutAlignment)Taskbar.DockPosition);
                });
            }
        }      
    }
}
