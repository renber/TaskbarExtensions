﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CalendarWeekView.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Segoe UI Symbol, 9pt")]
        public global::System.Drawing.Font DisplayFont {
            get {
                return ((global::System.Drawing.Font)(this["DisplayFont"]));
            }
            set {
                this["DisplayFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color FontColor {
            get {
                return ((global::System.Drawing.Color)(this["FontColor"]));
            }
            set {
                this["FontColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Week\r\n%week%")]
        public string DisplayFormatString {
            get {
                return ((string)(this["DisplayFormatString"]));
            }
            set {
                this["DisplayFormatString"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("BetweenTrayAndClock")]
        public global::TaskBarExt.Utils.TaskbarWindowPlacement Placement {
            get {
                return ((global::TaskBarExt.Utils.TaskbarWindowPlacement)(this["Placement"]));
            }
            set {
                this["Placement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ISO8601")]
        public global::CalendarWeekView.Types.CalendarWeekCalculationRule CalendarWeekRule {
            get {
                return ((global::CalendarWeekView.Types.CalendarWeekCalculationRule)(this["CalendarWeekRule"]));
            }
            set {
                this["CalendarWeekRule"] = value;
            }
        }
    }
}