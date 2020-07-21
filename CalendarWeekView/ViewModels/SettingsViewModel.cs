using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt.Utils;
using CalendarWeekView.Types;
using System.Xml;
using CalendarWeekView.Services;

namespace CalendarWeekView.ViewModels
{
    class SettingsViewModel : WindowViewModelBase
    {
        IDialogService DialogService { get; }
        ITaskbarWindowService TaskbarService { get; }
        IAppSettings Settings { get; }
        public IList<SingleValueViewModel<TaskbarWindowPlacement>> AvailablePlacements { get; } = new List<SingleValueViewModel<TaskbarWindowPlacement>>();

        SingleValueViewModel<TaskbarWindowPlacement> selectedPlacement;
        public SingleValueViewModel<TaskbarWindowPlacement> SelectedPlacement { get => selectedPlacement; set => SetProperty(ref selectedPlacement, value); }

        public IList<SingleValueViewModel<CalendarWeekCalculationRule>> AvailableWeekRules { get; } = new List<SingleValueViewModel<CalendarWeekCalculationRule>>();

        SingleValueViewModel<CalendarWeekCalculationRule> selectedWeekRule;
        public SingleValueViewModel<CalendarWeekCalculationRule> SelectedWeekRule { get => selectedWeekRule; set => SetProperty(ref selectedWeekRule, value); }

        Font displayFont = new Font(new FontFamily("Segoe UI Symbol"), 9f, FontStyle.Regular, GraphicsUnit.Point);
        public Font DisplayFont
        {
            get => displayFont;
            set
            {
                if (SetProperty(ref displayFont, value))
                {
                    OnPropertyChanged(nameof(DisplayFontDescription));
                }
            }
        }

        Color displayFontColor = Color.Blue;
        public Color DisplayFontColor { get => displayFontColor; set => SetProperty(ref displayFontColor, value); }

        public string DisplayFontDescription => $"{DisplayFont.Name}, {DisplayFont.SizeInPoints:0.#} pt";

        string displayFormatString = "KW %week%";
        public string DisplayFormatString { get => displayFormatString; set => SetProperty(ref displayFormatString, value); }

        bool autostart = false;
        public bool Autostart { get => autostart; set => SetProperty(ref autostart, value); }

        public Action ApplyCommand { get; private set; }
        public Action OkCommand { get; private set; }
        public Action ChangeFontCommand { get; private set; }

        public Action CancelCommand { get; private set; }

        public SettingsViewModel(IAppSettings targetSettings, ITaskbarWindowService taskbarWindowService, IDialogService dialogService)
        {
            Settings = targetSettings;
            TaskbarService = taskbarWindowService;
            DialogService = dialogService;

            foreach (var p in Enum.GetValues(typeof(TaskbarWindowPlacement)))
            {
                AvailablePlacements.Add(new SingleValueViewModel<TaskbarWindowPlacement>((TaskbarWindowPlacement)p, ((TaskbarWindowPlacement)p).GetDisplayText()));
            }

            foreach (var p in Enum.GetValues(typeof(CalendarWeekCalculationRule)))
            {
                AvailableWeekRules.Add(new SingleValueViewModel<CalendarWeekCalculationRule>((CalendarWeekCalculationRule)p, ((CalendarWeekCalculationRule)p).GetDisplayText()));
            }

            ReadSettings();
            InitCommands();
        }

        private void ReadSettings()
        {
            SelectedPlacement = AvailablePlacements.FirstOrDefault(x => x.Value == Settings.Placement);
            Autostart = Settings.Autostart;

            DisplayFont = (Font)Settings.DisplayFont.Clone();
            DisplayFontColor = Settings.FontColor;
            DisplayFormatString = Settings.DisplayFormatString;
            
            SelectedWeekRule = AvailableWeekRules.FirstOrDefault(x => x.Value == Settings.CalendarWeekRule);
        }

        private void WriteSettings()
        {
            Settings.Placement = SelectedPlacement?.Value ?? TaskbarWindowPlacement.BetweenTrayAndClock;
            Settings.Autostart = Autostart;

            Settings.DisplayFont = DisplayFont;
            Settings.FontColor = DisplayFontColor;
            Settings.DisplayFormatString = DisplayFormatString;
            
            Settings.CalendarWeekRule = SelectedWeekRule?.Value ?? CalendarWeekCalculationRule.ISO8601;

            Settings.Save();
        }

        private void InitCommands()
        {
           ApplyCommand = new Action(() =>
           {
               WriteSettings();
               TaskbarService.InvalidateAll();
           });

           OkCommand = new Action(() =>
           {
               ApplyCommand.Invoke();
               RequestViewClose();
           });

            CancelCommand = new Action(() => RequestViewClose());

            ChangeFontCommand = new Action(() =>
            {            
                if (DialogService.ShowFontDialog(DisplayFont, DisplayFontColor, out Tuple<Font, Color> selection))
                {
                    DisplayFont = selection.Item1;
                    DisplayFontColor = selection.Item2;
                }                
            });
        }
    }
}
