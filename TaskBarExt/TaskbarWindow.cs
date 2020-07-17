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
        public ITaskbarComponent TaskbarComponent { get; private set; }
        protected TaskbarWindowPlacement Placement { get; private set; }

        protected bool IsAttaching { get; set; } = false;

        /// <summary>
        /// Check if the window is still correctly attached to the task bar or needs reattachment
        /// (e.g. sicne the taskbar has revalidated itself)
        /// </summary>
        bool IsCorrectlyAttached { get; } = false;

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
        public TaskbarWindow(TaskbarRef targetTaskbar, TaskbarWindowPlacement placement, ITaskbarComponent component)
        {
            if (targetTaskbar == null)
                throw new ArgumentNullException("targetTaskbar");
            if (component == null)
                throw new ArgumentNullException("component");

            Taskbar = targetTaskbar;
            TaskbarComponent = component;
            Placement = placement;

            TargetSize = TaskbarComponent.PreferredSize;

            FormBorderStyle = FormBorderStyle.None;

            // fix flickering
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);            
            
            // since the window is a child of the taskbar, its
            // background becomes transparent automatically             
            BackColor = Color.Black;

            // when the taskbar position or size changes
            // update this window's position/size accordingly
            Taskbar.PositionOrSizeChanged += Taskbar_PositionOrSizeChanged;

            // wire up component events
            component.RefreshRequested += Component_RefreshRequested;
        }

        private void Taskbar_PositionOrSizeChanged(object sender, TaskbarChangedEventArgs e)
        {
            if (!IsDisposed)
            {
                AttachToTaskbar();
            }
        }

        private void Component_RefreshRequested(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => this.Refresh()));
                else
                    this.Refresh();
            }
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

        void Place_RightOfTaskButtons(Rectangle taskbarRect)
        {
            // place into the button bar
            // Set this window as child of the taskbar's button bar            
            IntPtr btnsHwnd;

            if (Taskbar.IsPrimary)
                btnsHwnd = TaskbarUtils.GetPrimaryTaskButtonsHwnd(Taskbar.Handle);
            else
                btnsHwnd = TaskbarUtils.GetSecondaryTaskButtonsHwnd(Taskbar.Handle);

            NativeImports.SetWindowLong(Handle, NativeImports.GWL_STYLE, NativeImports.GetWindowLong(Handle, NativeImports.GWL_STYLE) | NativeImports.WS_CHILD);
            NativeImports.SetParent(Handle, btnsHwnd);

            // get the size of the button bar to place the window
            var taskBtnRect = WindowUtils.GetWindowBounds(btnsHwnd);

            switch (Taskbar.DockPosition)
            {
                case TaskbarDockPosition.Top:
                case TaskbarDockPosition.Bottom:
                    ActualSize = new Size(TargetSize.Width, taskbarRect.Height);
                    // place the component at the far right                       
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, taskBtnRect.Width - TargetSize.Width, 0, ActualSize.Width, ActualSize.Height, 0);
                    break;

                case TaskbarDockPosition.Left:
                case TaskbarDockPosition.Right:
                    ActualSize = new Size(taskbarRect.Width, TargetSize.Height);

                    // place the component at the bottom                                        
                    // we use SetWindowPos since setting Left and Top does not seem to work correctly
                    NativeImports.SetWindowPos(Handle, IntPtr.Zero, 0, taskBtnRect.Height - TargetSize.Height, ActualSize.Width, ActualSize.Height, 0);
                    break;
            }

            // listen for size changes of the button area
            Taskbar.RegisterObservableChild(btnsHwnd);
        }

        void Place_BetweenTrayAndClock(Rectangle taskbarRect)
        {
            if (!Taskbar.IsPrimary)
            {
                // there is no tray area on secondary taskbars
                // place normally
                Place_RightOfTaskButtons(taskbarRect);
                return;
            }

            IntPtr btnsHwnd = TaskbarUtils.GetPrimaryTaskButtonsHwnd(Taskbar.Handle);
            IntPtr trayHwnd = TaskbarUtils.GetPrimaryTaskTrayNotifyHwnd(Taskbar.Handle);

            int w = TargetSize.Width;

            // shrink the task bar buttons area
            if (NativeImports.GetWindowRect(btnsHwnd, out NativeImports.RECT btnsRect))
            {
                NativeImports.SetWindowPos(btnsHwnd, IntPtr.Zero, 0, 0, (btnsRect.Right - btnsRect.Left) - w, btnsRect.Bottom - btnsRect.Top, NativeImports.SetWindowPosFlags.SWP_NOMOVE | NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);
            }        
           
            // enlarge and move the tray area to the left
            if (NativeImports.GetWindowRect(trayHwnd, out NativeImports.RECT trayRect))
            {
                Rectangle clientRect = new Rectangle(trayRect.Left - taskbarRect.X, trayRect.Top - taskbarRect.Top, trayRect.Right - trayRect.Left, trayRect.Bottom - trayRect.Top);
                NativeImports.SetWindowPos(trayHwnd, IntPtr.Zero, clientRect.Left - w, clientRect.Top, clientRect.Width + w, clientRect.Height, NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);
                // adjust
                trayRect.Left -= w;

                // move clock and other elements to the right
                var elements = TaskbarUtils.GetPrimaryTaskTrayElementsRightOfIcons(Taskbar.Handle);
                int initialX = -1;
                foreach (var elWnd in elements)
                {
                    if (NativeImports.GetWindowRect(elWnd, out NativeImports.RECT elementRect))
                    {
                        Rectangle elementClientRect = new Rectangle(elementRect.Left - trayRect.Left, elementRect.Top - trayRect.Top, elementRect.Right - elementRect.Left, elementRect.Bottom - elementRect.Top);
                        if (initialX == -1)
                            initialX = elementClientRect.X;

                        NativeImports.SetWindowPos(elWnd, IntPtr.Zero, elementClientRect.X + w, elementClientRect.Y, elementClientRect.Width, elementClientRect.Height, 0);
                    }
                }

                // place TaskbarWindow between icons and clock
                ActualSize = new Size(TargetSize.Width, clientRect.Height);
                NativeImports.SetWindowLong(Handle, NativeImports.GWL_STYLE, NativeImports.GetWindowLong(Handle, NativeImports.GWL_STYLE) | NativeImports.WS_CHILD);
                NativeImports.SetParent(Handle, trayHwnd);
                NativeImports.SetWindowPos(Handle, IntPtr.Zero, initialX, clientRect.Y, ActualSize.Width, ActualSize.Height, NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);

                // listen for size changes of the tray area
                Taskbar.RegisterObservableChild(trayHwnd);
            }
        }

        void Place_EndOfTaskbar(Rectangle taskbarRect)
        {
            // TODO: Implement Secondary Taskbar
            if (!Taskbar.IsPrimary)
            {                
                Place_RightOfTaskButtons(taskbarRect);
                return;
            }            

            IntPtr btnsHwnd = TaskbarUtils.GetPrimaryTaskButtonsHwnd(Taskbar.Handle);
            IntPtr trayHwnd = TaskbarUtils.GetPrimaryTaskTrayNotifyHwnd(Taskbar.Handle);

            int w = TargetSize.Width;

            // shrink the task bar buttons area
            if (NativeImports.GetWindowRect(btnsHwnd, out NativeImports.RECT btnsRect))
            {
                NativeImports.SetWindowPos(btnsHwnd, IntPtr.Zero, 0, 0, (btnsRect.Right - btnsRect.Left) - w, btnsRect.Bottom - btnsRect.Top, NativeImports.SetWindowPosFlags.SWP_NOMOVE | NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);
            }

            // enlarge and move the tray area to the left
            if (NativeImports.GetWindowRect(trayHwnd, out NativeImports.RECT trayRect))
            {
                Rectangle clientRect = new Rectangle(trayRect.Left - taskbarRect.X, trayRect.Top - taskbarRect.Top, trayRect.Right - trayRect.Left, trayRect.Bottom - trayRect.Top);
                NativeImports.SetWindowPos(trayHwnd, IntPtr.Zero, clientRect.Left - w, clientRect.Top, clientRect.Width + w, clientRect.Height, NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);

                ActualSize = new Size(TargetSize.Width, taskbarRect.Height);
                NativeImports.SetWindowLong(Handle, NativeImports.GWL_STYLE, NativeImports.GetWindowLong(Handle, NativeImports.GWL_STYLE) | NativeImports.WS_CHILD);
                NativeImports.SetParent(Handle, trayHwnd);
                NativeImports.SetWindowPos(Handle, IntPtr.Zero, clientRect.Width, 0, ActualSize.Width, ActualSize.Height, NativeImports.SetWindowPosFlags.SWP_NOREPOSITION);
            }

            // Listen for changes of the tray area
            Taskbar.RegisterObservableChild(trayHwnd);
        }

        /// <summary>
        /// Attach this window to the connected taskbar or update the positioning/sizing
        /// after the taskbar size/position has changed
        /// </summary>
        void AttachToTaskbar()
        {
            if (IsAttaching) return;

            try
            {
                IsAttaching = true;

                var taskbarRect = WindowUtils.GetWindowBounds(Taskbar.Handle);                

                switch (Placement)
                {
                    case TaskbarWindowPlacement.RightOfTaskButtons:
                        Place_RightOfTaskButtons(taskbarRect);
                        break;
                    case TaskbarWindowPlacement.BetweenTrayAndClock:
                        Place_BetweenTrayAndClock(taskbarRect);
                        break;
                    case TaskbarWindowPlacement.EndOfTaskbar:
                        Place_EndOfTaskbar(taskbarRect);
                        break;
                }
            }
            finally
            {
                IsAttaching = false;
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

        /// <summary>
        /// Undo any changes done to the taskbar by this component
        /// </summary>
        public virtual void RestoreTaskbar()
        {
            Taskbar.PositionOrSizeChanged -= Taskbar_PositionOrSizeChanged;
            TaskbarComponent.RefreshRequested -= Component_RefreshRequested;
        }
    }
}
