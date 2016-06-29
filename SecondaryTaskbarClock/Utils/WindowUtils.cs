using SecondaryTaskbarClock.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondaryTaskbarClock.Utils
{
    public static class WindowUtils
    {
        /// <summary>
        /// Get the bounds of the window with the given handle
        /// ((Wrapper around Win32 GetWindowRect)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            NativeImports.RECT rect;
            if (NativeImports.GetWindowRect(handle, out rect))
                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            else
                throw new InvalidOperationException("Could not get window bounds.");
        }

        /// <summary>
        /// Returns the handles of all top-level windows of the given class
        /// </summary>
        /// <param name="className">The class name to search fore</param>        
        public static ISet<IntPtr> ListWindowsWithClass(string className)
        {
            HashSet<IntPtr> resultHandles = new HashSet<IntPtr>();

            IntPtr currChild = NativeImports.FindWindowEx(IntPtr.Zero, IntPtr.Zero, className, null);

            while(currChild != IntPtr.Zero)
            {
                resultHandles.Add(currChild);
                // next window (if any)
                currChild = NativeImports.FindWindowEx(IntPtr.Zero, currChild, className, null);
            }

            return resultHandles;
        }
    }
}
