using TaskBarExt.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskBarExt.Utils
{
    public static class TaskbarUtils
    {
        const string PrimaryTaskbarClassName = "shell_traywnd";
        const string PrimaryTaskbarTrayClassName = "traynotifywnd";
        const string PrimaryButtonBarClassName = "rebarwindow32";

        const string SecondaryTaskbarClassName = "shell_secondarytraywnd";
        const string SecondaryButtonBarParentClassName = "workerw";
        const string SecondaryButtonBarClassName = "mstasklistwclass";

        const string CalendarFlyoutClassName = "Windows.UI.Core.CoreWindow";

        /// <summary>
        /// Return all visible taskbars
        /// </summary>        
        public static IList<TaskbarRef> ListTaskbars()
        {
            List<TaskbarRef> taskbars = new List<TaskbarRef>();

            // get the primary taskbar
            var primaryHwnd = GetPrimaryTaskbarHwnd();
            taskbars.Add(new TaskbarRef(true, primaryHwnd, WindowUtils.GetWindowBounds(primaryHwnd), GetTaskbarDockPosition(primaryHwnd)));

            // find all secondary taskbars
            IntPtr secondaryHwnd = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, SecondaryTaskbarClassName, null);
            while (secondaryHwnd != IntPtr.Zero)
            {
                taskbars.Add(new TaskbarRef(false, secondaryHwnd, WindowUtils.GetWindowBounds(secondaryHwnd), GetTaskbarDockPosition(secondaryHwnd)));

                // get the next secondary taskbar (if any)
                secondaryHwnd = NativeImports.FindWindowEx(IntPtr.Zero, secondaryHwnd, SecondaryTaskbarClassName, null);
            }

            return taskbars;
        }

        /// <summary>
        /// Return the screen on which the taskbar with the given handle is displayed
        /// </summary>
        /// <param name="taskbarHwnd"></param>
        /// <returns></returns>
        private static Screen FindTaskbarScreen(IntPtr taskbarHwnd)
        {
            Rectangle taskbarRect = WindowUtils.GetWindowBounds(taskbarHwnd);
            var screens = Screen.AllScreens.ToList();
            return screens.FirstOrDefault(x => x.Bounds.IntersectsWith(taskbarRect));
        }

        /// <summary>
        /// Get the docking position of the taskbar with the given handle
        /// </summary>
        /// <param name="taskbarHwnd"></param>
        /// <returns></returns>
        internal static TaskbarDockPosition GetTaskbarDockPosition(IntPtr taskbarHwnd)
        {
            Screen screen = FindTaskbarScreen(taskbarHwnd);
            var taskbarRect = WindowUtils.GetWindowBounds(taskbarHwnd);
            
            if (screen.Bounds.Left == taskbarRect.Left && screen.Bounds.Top == taskbarRect.Top)
            {                
                // taskbar touches the top left corner
                if (taskbarRect.Right == screen.Bounds.Right)
                    // ... and the top right corner as well                    
                    return TaskbarDockPosition.Top;
                else
                    return TaskbarDockPosition.Left;
            } else
            {
                // taskbar touches the bottom right corner
                if (taskbarRect.Left == screen.Bounds.Left)
                    // and the bottom left corner as welt
                    return TaskbarDockPosition.Bottom;
                else
                    return TaskbarDockPosition.Right;
            }
        }

        /// <summary>
        /// Return the handle of the primary taskbar
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetPrimaryTaskbarHwnd()
        {
            return NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, PrimaryTaskbarClassName, null);
        }

        /// <summary>
        /// Return the handle of the button bar of the primary taskbar with the given handle
        /// </summary>
        /// <param name="taskbarHwnd"></param>
        /// <returns></returns>
        public static IntPtr GetPrimaryTaskButtonsHwnd(IntPtr taskbarHwnd)
        {            
            return NativeImports.FindWindowEx(taskbarHwnd, IntPtr.Zero, PrimaryButtonBarClassName, null);
        }

        /// <summary>
        /// Return the handle of the button bar of the secondary taskbar with the given handle
        /// </summary>
        /// <param name="taskbarHwnd"></param>
        /// <returns></returns>
        public static IntPtr GetSecondaryTaskButtonsHwnd(IntPtr taskbarHwnd)
        {
            IntPtr workerW = NativeImports.FindWindowEx(taskbarHwnd, IntPtr.Zero, SecondaryButtonBarParentClassName, null);
            return NativeImports.FindWindowEx(workerW, IntPtr.Zero, SecondaryButtonBarClassName, null);
        }

        /// <summary>
        /// Return the handle of the tray area of the primary taskbar
        /// </summary>
        /// <param name="primaryTaskbarHwnd">Handle of the primary taskbar</param>
        /// <returns></returns>
        private static IntPtr GetPrimaryTaskbarTrayHwnd(IntPtr primaryTaskbarHwnd)
        {
            return NativeImports.FindWindowEx(primaryTaskbarHwnd, IntPtr.Zero, PrimaryTaskbarTrayClassName, null);
        }

        /// <summary>
        /// Return the handle of the clock of the primary taskbar
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetPrimaryTaskbarClockHwnd()
        {
            // todo: error handling            
            IntPtr trayclockwclass = NativeImports.FindWindowEx(GetPrimaryTaskbarTrayHwnd(GetPrimaryTaskbarHwnd()), IntPtr.Zero, "trayclockwclass", null);
            return trayclockwclass;
        }        

        /// <summary>
        /// Return the handle of the taskbar's calendar flyout window
        /// </summary>
        /// <param name="exceptHandles">Only a potenial flyout window with a handle not in this set is returned</param>
        /// <returns></returns>
        private static IntPtr GetCalendarFlyoutHwnd(ISet<IntPtr> exceptHandles)
        {
            // since the text of the calendar flyout is language-dependent and the
            // class name is not unique, we search for a window of class
            // Windows.UI.Core.CoreWindow which touches the primary taskbar's clock
            // and is not contained in the passed set exceptHandles 
            IntPtr taskbarClockHwnd = GetPrimaryTaskbarClockHwnd();
            Rectangle taskbarClockRect = WindowUtils.GetWindowBounds(taskbarClockHwnd);

            IntPtr candidateHwnd = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, CalendarFlyoutClassName, null);            
            
            while (candidateHwnd != IntPtr.Zero)
            {
                // is this window a potential candidate to be the calendar flyout?
                if (!exceptHandles.Contains(candidateHwnd))
                {                    
                    // does the current window touch the primary taskbar's clock?                
                    Rectangle candidateRect = WindowUtils.GetWindowBounds(candidateHwnd);
                    candidateRect.Intersect(taskbarClockRect);

                    // if the two rectangles touch, intersect results in a rectangle
                    // where exactly one dimension is zero
                    if ((candidateRect.Width == 0 && candidateRect.Height > 0) ||
                        (candidateRect.Width > 0 && candidateRect.Height == 0))
                    {
                        // with a very high probability, this is the calendar flyout
                        return candidateHwnd;
                    }
                }

                // find the next window with the calendar class
                candidateHwnd = NativeImports.FindWindowEx(IntPtr.Zero, candidateHwnd, CalendarFlyoutClassName, null);
            }

            return IntPtr.Zero;

            /*
            // if we have the localized window text, we may use the following
            const string flyoutClassname = "Windows.UI.Core.CoreWindow";
            const string flyoutText = "Datums- und Uhrzeitinformationen";

            IntPtr hwnd = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, flyoutClassname, flyoutText);
            return hwnd;*/
        }

        /// <summary>
        /// Invoke the primary taskbar's calendar flyout
        /// </summary>        
        private static bool InvokeCalendarFlyOut()
        {
            // in order to show the calendar flyout
            // we simulate a click on the primary taskbars clock
            var clockHwnd = GetPrimaryTaskbarClockHwnd();
            var taskbarHwnd = GetPrimaryTaskbarHwnd();

            if (clockHwnd == IntPtr.Zero || taskbarHwnd == IntPtr.Zero)
                return false;

            // get clock window bounds            
            NativeImports.RECT clockRect;
            if (!NativeImports.GetWindowRect(clockHwnd, out clockRect))
                return false;

            // the click has to be sent to the taskbar (not to the clock hwnd)
            // but at the clocks location
            IntPtr wParam = new IntPtr(NativeImports.HTCAPTION);
            // simulate the click, some pixels inside the tray clock
            IntPtr lParam = new IntPtr(((clockRect.Top + 10) << 16) | clockRect.Left + 10);

            NativeImports.PostMessage(taskbarHwnd, NativeImports.WM_NCLBUTTONDOWN, wParam.ToInt32(), lParam.ToInt32());
            NativeImports.PostMessage(taskbarHwnd, NativeImports.WM_NCLBUTTONUP, wParam.ToInt32(), lParam.ToInt32());

            // remove the focus highlight
            NativeImports.PostMessage(taskbarHwnd, NativeImports.WM_MOUSELEAVE, 0, 0);

            return true;
        }

        /// <summary>
        /// Show the primary taskbar's calendar flyout at the given location with the given alignment
        /// </summary>    
        public static void ShowCalendarFlyOut(Rectangle alignTo, FlyoutAlignment alignment)
        {
            // get all window handles which have the same class as the
            // flyout, so that we know these are not the flyout
            var nonFlyoutHandles = WindowUtils.ListWindowsWithClass(CalendarFlyoutClassName);

            // invoke the primary taskbar's calendar flyout
            if (TaskbarUtils.InvokeCalendarFlyOut())
            {
                // wait for it to open (max. 2 seconds)
                IntPtr flyoutHwnd = IntPtr.Zero;
                int start = Environment.TickCount;
                while (flyoutHwnd == IntPtr.Zero && (Environment.TickCount - start <= 2000))
                    flyoutHwnd = TaskbarUtils.GetCalendarFlyoutHwnd(nonFlyoutHandles);

                if (flyoutHwnd != IntPtr.Zero)
                {                    
                    // and move it to this clock's location                   
                    NativeImports.RECT flyoutRect;
                    if (NativeImports.GetWindowRect(flyoutHwnd, out flyoutRect))
                    {
                        int flyoutWidth = flyoutRect.Right - flyoutRect.Left;
                        int flyoutHeight = flyoutRect.Bottom - flyoutRect.Top;

                        switch (alignment)
                        {
                            case FlyoutAlignment.Below:
                                // place the calendar flyout below the clock
                                NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, alignTo.Right - flyoutWidth, alignTo.Bottom, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                                break;

                            case FlyoutAlignment.Above:
                                // place the calendar flyout above the clock
                                NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, alignTo.Right - flyoutWidth, alignTo.Top - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                                break;

                            case FlyoutAlignment.ToTheRight:
                                // place the calendar flyout to the right of the clock
                                NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, alignTo.Right, alignTo.Bottom - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                                break;

                            case FlyoutAlignment.ToTheLeft:
                                // place the calendar flyout to the left of the clock
                                NativeImports.SetWindowPos(flyoutHwnd, IntPtr.Zero, alignTo.Left - flyoutWidth, alignTo.Bottom - flyoutHeight, 0, 0, NativeImports.SetWindowPosFlags.SWP_NOSIZE);
                                break;
                        }
                    }
                }
            }
        }
    }

    public class TaskbarRef
    {
        public bool IsPrimary { get; private set; }
        public TaskbarDockPosition DockPosition { get; private set; }
        public IntPtr Handle { get; private set; }
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Raised when the position or the size of this taskbar has changed
        /// </summary>
        public event EventHandler PositionOrSizeChanged;
        
        public TaskbarRef(bool isPrimary, IntPtr hwnd, Rectangle bounds, TaskbarDockPosition dockPosition)
        {
            IsPrimary = isPrimary;
            Handle = hwnd;
            Bounds = bounds;
            DockPosition = dockPosition;
        }

        /// <summary>
        /// Raises the PositionOrSizeChanged event
        /// 
        /// </summary>
        protected void RaisePositionOrSizeChanged()
        {            
            PositionOrSizeChanged?.Invoke(this, EventArgs.Empty);            
        }

        /// <summary>
        /// Reevaluates the DockPosition and Bounds
        /// and raises the PositionOrSizeChanged event if necessary
        /// </summary>
        public void Update()
        {
            var oldDockPosition = DockPosition;
            var oldBounds = Bounds;

            DockPosition = TaskbarUtils.GetTaskbarDockPosition(Handle);
            Bounds = WindowUtils.GetWindowBounds(Handle);

            if (oldDockPosition != DockPosition ||
                oldBounds != Bounds)
                RaisePositionOrSizeChanged();
        }
    }

    public enum TaskbarDockPosition
    {
        /// <summary>
        // default location
        /// </summary>
        Bottom = 0,
        Top,
        Left,
        Right
    }

    /// <summary>
    /// Alignment of the calendar flyout
    /// (Ordinals match the corresponding TaskbarDockPosition)
    /// </summary>
    public enum FlyoutAlignment
    {
        Above = 0,
        Below,
        ToTheRight,
        ToTheLeft
    }

    public static class TaskbarDockPositionExtensions
    {
        /// <summary>
        /// Return the position for the calendar flyout for the given taskbar dock position
        /// </summary>
        /// <param name="dockPosition"></param>
        /// <returns></returns>
        public static FlyoutAlignment GetCorrespondingFlyoutPosition(this TaskbarDockPosition dockPosition)
        {
            return (FlyoutAlignment)dockPosition;
        }
    }
}
