using CalendarWeekView.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt.Utils;

namespace CalendarWeekView.Types
{
    class AppSettings : IAppSettings
    {
        public Font DisplayFont
        {
            get => Properties.Settings.Default.DisplayFont;
            set => Properties.Settings.Default.DisplayFont = value;
        }       

        public Color FontColor
        {
            get => Properties.Settings.Default.FontColor;
            set => Properties.Settings.Default.FontColor = value;
        }

        public string DisplayFormatString
        {
            get => Properties.Settings.Default.DisplayFormatString;
            set => Properties.Settings.Default.DisplayFormatString = value;
        }

        public TaskbarWindowPlacement Placement
        {
            get => Properties.Settings.Default.Placement;
            set => Properties.Settings.Default.Placement = value;
        }

        public CalendarWeekCalculationRule CalendarWeekRule
        {
            get => Properties.Settings.Default.CalendarWeekRule;
            set => Properties.Settings.Default.CalendarWeekRule = value;
        }

        public bool Autostart
        {
            get => IsInAutostart();
            set => SetAutostart(value);
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        private string GetExistingAutoStartEntry()
        {
            var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            foreach(var f in Directory.GetFiles(startupFolder, "*.lnk"))
            {
                var shellLink = new ShellLink(f);
                if (PathEquals(shellLink.Target, Application.ExecutablePath))
                {
                    return shellLink.ShortCutFile;
                }               
            }

            return null;
        }

        private static bool PathEquals(string path1, string path2)
        {
            return Path.GetFullPath(path1)
                .Equals(Path.GetFullPath(path2), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check if there is a link to the current application
        /// in the user's autostart folder
        /// </summary>
        /// <returns></returns>
        private bool IsInAutostart()
        {
            return GetExistingAutoStartEntry() != null;
        }

        private void SetAutostart(bool autostartEnabled)
        {
            try
            {
                if (autostartEnabled)
                {
                    if (IsInAutostart()) return;

                    var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                    var shellLink = new ShellLink();
                    shellLink.IconPath = Application.ExecutablePath;
                    shellLink.Target = Application.ExecutablePath;
                    shellLink.ShortCutFile = Path.Combine(startupFolder, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".lnk");
                    shellLink.Save();
                }
                else
                {
                    // remove the existing link file
                    string f = GetExistingAutoStartEntry();
                    if (f != null)
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception e)
            {
                // something's wrong
            }

        }
    }
}
