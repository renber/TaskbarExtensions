using TaskBarExt.Components;
using TaskBarExt.Native;
using TaskBarExt.Renderers;
using TaskBarExt.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskBarExt
{
    // ------------
    // TODO:
    // Hook WM_WINDOWPOSCHANGED of the taskbar and
    // reposition the window if necessary
    // Hook WM_EXITSIZEMOVE to monitor size changes
    // ------------

    /// <summary>
    /// A window which gets added to a taskbar as a component
    /// </summary>
    public class TaskbarWindow : Form
    {
        protected TaskbarRef Taskbar { get; private set; }
        protected ITaskbarComponent TaskbarComponent { get; private set; }

        Size TargetSize = new Size();
        Size ActualSize = new Size();

        bool isMouseOver = false;        
        bool isMouseDown = false;

        /// <summary>
        /// Constructor for the Visual Studio Designer
        /// </summary>
        private TaskbarWindow()
        {
            // --
        }

        /// <summary>
        /// Creates a new taskbar window and adds it to the given taskbar
        /// </summary>
        /// <param name="targetTaskbar">The taskbar to add this window to</param>
        public TaskbarWindow(TaskbarRef targetTaskbar, ITaskbarComponent component)
        {
            if (targetTaskbar == null)
                throw new ArgumentNullException("targetTaskbar");
            if (component == null)
                throw new ArgumentNullException("component");

            Taskbar = targetTaskbar;
            TaskbarComponent = component;

            TargetSize = TaskbarComponent.PreferredSize;

            FormBorderStyle = FormBorderStyle.None;

            // fix flickering
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);            
            
            // since the window is a child of the taskbar, its
            //background becomes transparent automatically             
            BackColor = Color.Black;

            // when the taskbar position or size changes
            // update this window's position/size accordingly
            Taskbar.PositionOrSizeChanged += (s, e) => AttachToTaskbar();

            // wire up component events
            component.RefreshRequested += (s, e) =>
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => this.Refresh()));
                else
                    this.Refresh();
            };            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            TaskbarComponent?.Renderer?.Render(e.Graphics, new RendererParameters(Width, Height, isMouseOver, isMouseDown));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // we do not want to interact with the taskbar at designtime
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                AttachToTaskbar();
        }

        /// <summary>
        /// Attach this window to the connected taskbar or update the positioning/sizing
        /// after the taskbar size/position has changed
        /// </summary>
        void AttachToTaskbar()
        {
            var taskbarRect = WindowUtils.GetWindowBounds(Taskbar.Handle);

            // Set this window as child of the taskbar's button bar            
            IntPtr btnsHwnd;

            if (Taskbar.IsPrimary)
                btnsHwnd = TaskbarUtils.GetPrimaryTaskButtonsHwnd(Taskbar.Handle);
            else
                btnsHwnd = TaskbarUtils.GetSecondaryTaskButtonsHwnd(Taskbar.Handle);

            NativeImports.SetWindowLong(Handle, NativeImports.GWL_STYLE, NativeImports.GetWindowLong(Handle, NativeImports.GWL_STYLE) | NativeImports.WS_CHILD);
            NativeImports.SetParent(Handle, btnsHwnd);

            // get the size of the button bar to place the clock
            var taskBtnRect = WindowUtils.GetWindowBounds(btnsHwnd);

            switch (Taskbar.DockPosition)
            {
                case TaskbarDockPosition.Top:
                case TaskbarDockPosition.Bottom:
                    ActualSize = new Size(TargetSize.Width, taskbarRect.Height);

                    // place the clock at the far right                       
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, taskBtnRect.Width - TargetSize.Width, 0, ActualSize.Width, ActualSize.Height, 0);
                    break;

                case TaskbarDockPosition.Left:
                case TaskbarDockPosition.Right:
                    ActualSize = new Size(taskbarRect.Width, TargetSize.Height);

                    // place the clock at the bottom                                        
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, 0, taskBtnRect.Height - TargetSize.Height, ActualSize.Width, ActualSize.Height, 0);
                    break;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            isMouseOver = true;

            if (TaskbarComponent.RefreshBehavior.HasFlag(RefreshBehavior.MouseEnter))
                this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            isMouseOver = false;

            if (TaskbarComponent.RefreshBehavior.HasFlag(RefreshBehavior.MouseLeave))
                this.Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;

                if (TaskbarComponent.RefreshBehavior.HasFlag(RefreshBehavior.MouseDown))
                    this.Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;

                if (TaskbarComponent.RefreshBehavior.HasFlag(RefreshBehavior.MouseUp))
                    this.Refresh();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (TaskbarComponent.RefreshBehavior.HasFlag(RefreshBehavior.MouseMove))
                this.Refresh();
        }
    }
}
