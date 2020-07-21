using CalendarWeekView.Renderers;
using CalendarWeekView.Services;
using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Components;
using TaskBarExt.Renderers;

namespace CalendarWeekView.Components
{    
    class CalendarWeekComponent : ITaskbarComponent
    {
        IAppSettings Settings { get; }

        Size preferredSize = new Size(60, 40);

        public Size PreferredSize => preferredSize;

        public ITaskbarComponentRenderer Renderer { get; private set; }

        public RefreshBehavior RefreshBehavior
        {
            get
            {
                return RefreshBehavior.MouseEnter | RefreshBehavior.MouseLeave | RefreshBehavior.MouseDown | RefreshBehavior.MouseUp;
            }
        }

        public event EventHandler RefreshRequested;

        public CalendarWeekComponent(IAppSettings settings)
        {
            Settings = settings;
            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            Renderer = new Win10CalendarWeekRenderer(Settings.DisplayFont, Settings.FontColor, Settings.DisplayFormatString, CalendarWeekCalculationRule.ISO8601);
            OnRefreshRequested();
        }

        protected void OnRefreshRequested()
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
