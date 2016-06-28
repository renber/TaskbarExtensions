using SecondaryTaskbarClock.Native;
using SecondaryTaskbarClock.Renderers;
using SecondaryTaskbarClock.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondaryTaskbarClock.Views
{
    /// <summary>
    /// A window which gets added to a taskbar as a component
    /// </summary>
    public class TaskbarWindow : Form
    {
        protected TaskbarInfo Taskbar { get; private set; }
        protected IWindowContentRenderer ContentRenderer { get; private set; }

        Size TargetSize = new Size(80, 40);
        bool isMouseOver = false;
        bool isMouseDown = false;

        /// <summary>
        /// Creates a new taskbar window and adds it to the given taskbar
        /// </summary>
        /// <param name="targetTaskbar">The taskbar to add this window to</param>
        public TaskbarWindow(TaskbarInfo targetTaskbar, IWindowContentRenderer contentRenderer)
        {
            if (targetTaskbar == null)
                throw new ArgumentNullException("targetTaskbar");
            if (contentRenderer == null)
                throw new ArgumentNullException("contentRenderer");

            Taskbar = targetTaskbar;
            ContentRenderer = contentRenderer;

            FormBorderStyle = FormBorderStyle.None;
            // since the window is a child of the taskbar, its
            //background becomes transparent automatically             
            BackColor = Color.Black;            
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
