using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Renderers;
using SecondaryTaskbarClock.ViewModels;
using TaskBarExt.Components;
using SecondaryTaskbarClock.Renderers;

namespace SecondaryTaskbarClock.Components
{
    class ClockComponent : ITaskbarComponent
    {
        ClockViewModel ViewModel;

        Size preferredSize = new Size(80, 40);

        public Size PreferredSize
        {
            get
            {
                return preferredSize;
            }
        }

        public ITaskbarComponentRenderer Renderer { get; private set; }

        public RefreshBehavior RefreshBehavior
        {
            get
            {
                return RefreshBehavior.MouseEnter | RefreshBehavior.MouseLeave | RefreshBehavior.MouseDown | RefreshBehavior.MouseUp;
            }
        }

        public event EventHandler RefreshRequested;

        public ClockComponent(ClockViewModel viewModel)
        {
            ViewModel = viewModel;
            Renderer = new Win10TaskbarClockRenderer(viewModel);

            // request this component to be redrawn , when the current time changes
            ViewModel.PropertyChanged += (s, e) => OnRefreshRequested();
        }
        
        protected void OnRefreshRequested()
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
