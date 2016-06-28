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

            Click += ClockWindow_Click;   
        }

        private void ClockWindow_Click(object sender, EventArgs e)
        {            
            Task.Run(() =>
           {
               // invoke the primary taskbar's calendar flyout
               TaskbarUtils.InvokeCalendarFlyOut();

               // wait for it to open (max. 2 seconds)
               IntPtr flyoutHwnd = IntPtr.Zero;
               int start = Environment.TickCount;
               while (flyoutHwnd == IntPtr.Zero && (Environment.TickCount - start <= 2000))
                   flyoutHwnd = TaskbarUtils.GetCalendarFlyoutHwnd();

               if (flyoutHwnd != IntPtr.Zero)
               {
                   // give the flyout some time to initialize
                   Thread.Sleep(150);

                   // and move it to this clock's location                   
                   NativeImports.RECT flyoutRect;
                   if (NativeImports.GetWindowRect(flyoutHwnd, out flyoutRect))
                   {                       
                       int flyoutWidth = flyoutRect.Right - flyoutRect.Left;
                       int flyoutHeight = flyoutRect.Bottom - flyoutRect.Top;

                       switch (Taskbar.DockPosition)
                       {
                           case TaskbarDockPosition.Top:
                               // place the calendar flyout below the clock
                               NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, Bounds.Right - flyoutWidth, Bounds.Bottom, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                               break;

                           case TaskbarDockPosition.Bottom:
                               // place the calendar flyout above the clock
                               NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, Bounds.Right - flyoutWidth, Bounds.Top - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                               break;

                           case TaskbarDockPosition.Left:
                               // place the calendar flyout to the right of the clock
                               NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, Bounds.Right, Bounds.Bottom - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                               break;

                           case TaskbarDockPosition.Right:
                               // place the calendar flyout to the left of the clock
                               NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, Bounds.Left - flyoutWidth, Bounds.Bottom - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                               break;
                       }                       
                   }
               }
           });
        }        
    }
}
