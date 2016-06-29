using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SecondaryTaskbarClock.Native
{
    /// <summary>
    /// Native imports for the WinEvents API
    /// <see cref="http://stackoverflow.com/questions/9665579/setting-up-hook-on-windows-messages"/>
    /// </summary>
    public static class WinEventHook
    {
        // see https://msdn.microsoft.com/en-us/library/windows/desktop/dd318066(v=vs.85).aspx
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        const uint WINEVENT_OUTOFCONTEXT = 0;

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
       IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        public static IntPtr SetHook(uint eventId, WinEventDelegate callback)
        {
            return SetWinEventHook(eventId, eventId, IntPtr.Zero, callback, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static bool RemoveHook(IntPtr hook)
        {
            return UnhookWinEvent(hook);
        }
    }
}
