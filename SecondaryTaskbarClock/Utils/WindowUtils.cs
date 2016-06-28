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
    }
}
