using SecondaryTaskbarClock.Native;
using SecondaryTaskbarClock.Renderers;
using SecondaryTaskbarClock.Utils;
using SecondaryTaskbarClock.ViewModels;
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
    public class ClockWindow : Form
    {
        TaskbarInfo Taskbar { get; set; }
        IWindowContentRenderer ContentRenderer { get; set; }

        Size TargetSize = new Size(80, 40);

        bool isMouseOver = false;
        bool isMouseDown = false;

        public ClockWindow(TaskbarInfo targetTaskbar, ClockViewModel viewModel)            
        {            
            Taskbar = targetTaskbar;
            // currently we always use the Windows 10 renderer
            ContentRenderer = new Win10TaskbarClockRenderer(viewModel);

            FormBorderStyle = FormBorderStyle.None;

            // since the window is a child of the taskbar, its
            //background becomes transparent automatically             
            BackColor = Color.Black;

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
                   // give the flyout window some time to initialize completely
                   Thread.Sleep(150);
                   
                   NativeImports.RECT flyoutRect;
                   while (!NativeImports.GetWindowRect(flyoutHwnd, out flyoutRect))
                   {
                       // --
                   }
                   // and move it to this clock's location
                   // only works, when the taskbar is at the bottom of the screen atm               

                   Point topRight = new Point(Bounds.Right, Bounds.Top);

                   int flyoutWidth = flyoutRect.Right - flyoutRect.Left;
                   int flyoutHeight = flyoutRect.Bottom - flyoutRect.Top;
                   NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, topRight.X - flyoutWidth, topRight.Y - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
               }
           });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);            
            ContentRenderer.Render(e.Graphics, new RendererParameters(Width, Height, isMouseOver, isMouseDown));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var taskbarRect = WindowUtils.GetWindowBounds(Taskbar.Handle);

            // Set this window as child of the secondary taskbar's button bar            
            var btnsHwnd = TaskbarUtils.GetSecondaryTaskButtonsHwnd(Taskbar.Handle);
            NativeImports.SetWindowLong(Handle, NativeImports.GWL_STYLE, NativeImports.GetWindowLong(Handle, NativeImports.GWL_STYLE) | NativeImports.WS_CHILD);
            NativeImports.SetParent(Handle, btnsHwnd);
            
            // get the size of the button bar to place the clock
            var taskBtnRect = WindowUtils.GetWindowBounds(btnsHwnd);

            switch (Taskbar.DockPosition)
            {
                case TaskbarDockPosition.Top:
                case TaskbarDockPosition.Bottom:
                    TargetSize = new Size(TargetSize.Width, taskbarRect.Height);

                    // place the clock at the far right                       
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, taskBtnRect.Width - TargetSize.Width, 0, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                    break;

                case TaskbarDockPosition.Left:
                case TaskbarDockPosition.Right:
                    TargetSize = new Size(taskbarRect.Width, TargetSize.Height);

                    // place the clock at the bottom                                        
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, 0, taskBtnRect.Height - TargetSize.Height, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                    break;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // resize the window to fit the taskbar            
            var taskbarRect = WindowUtils.GetWindowBounds(Taskbar.Handle);
            
            Width = TargetSize.Width;
            Height = TargetSize.Height;                                

            this.Invalidate(); 
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            isMouseOver = true;
            this.Refresh();            
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            isMouseOver = false;
            this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            isMouseDown = true;
            this.Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            isMouseDown = false;
            this.Refresh();
        }
    }
}
