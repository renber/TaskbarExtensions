using SecondaryTaskbarClock.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondaryTaskbarClock.Utils
{
    public static class TaskbarUtils
    {
        const string PrimaryTaskbarClassName = "shell_traywnd";
        const string PrimaryTaskbarTrayClassName = "traynotifywnd";

        const string SecondaryTaskbarClassName = "shell_secondarytraywnd";
        const string SecondaryButtonBarParentClassName = "workerw";
        const string SecondaryButtonBarClassName = "mstasklistwclass";

        /// <summary>
        /// Return all visible taskbars
        /// </summary>        
        public static IList<TaskbarInfo> ListTaskbars()
        {
            List<TaskbarInfo> taskbars = new List<TaskbarInfo>();

            // get the primary taskbar
            var primaryHwnd = GetPrimaryTaskbarHwnd();
            taskbars.Add(new TaskbarInfo(true, primaryHwnd, WindowUtils.GetWindowBounds(primaryHwnd), GetTaskbarDockPosition(primaryHwnd)));

            // find all secondary taskbars
            IntPtr secondaryHwnd = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, SecondaryTaskbarClassName, null);
            while (secondaryHwnd != IntPtr.Zero)
            {
                taskbars.Add(new TaskbarInfo(false, secondaryHwnd, WindowUtils.GetWindowBounds(secondaryHwnd), GetTaskbarDockPosition(secondaryHwnd)));

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
        private static TaskbarDockPosition GetTaskbarDockPosition(IntPtr taskbarHwnd)
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
        /// <returns></returns>
        public static IntPtr GetCalendarFlyoutHwnd()
        {
            // only works for german systems atm
            const string flyoutClassname = "Windows.UI.Core.CoreWindow";
            const string flyoutText = "Datums- und Uhrzeitinformationen";

            IntPtr hwnd = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, flyoutClassname, flyoutText);
            return hwnd;
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
            // invoke the primary taskbar's calendar flyout
            if (TaskbarUtils.InvokeCalendarFlyOut())
            {
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

    public class TaskbarInfo
    {
        public bool IsPrimary { get; private set; }
        public TaskbarDockPosition DockPosition { get; private set; }
        public IntPtr Handle { get; private set; }
        public Rectangle Bounds { get; private set; }   
        
        public TaskbarInfo(bool isPrimary, IntPtr hwnd, Rectangle bounds, TaskbarDockPosition dockPosition)
        {
            IsPrimary = isPrimary;
            Handle = hwnd;
            Bounds = bounds;
            DockPosition = dockPosition;
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
