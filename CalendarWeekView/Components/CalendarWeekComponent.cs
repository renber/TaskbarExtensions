using CalendarWeekView.Renderers;
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
        Size preferredSize = new Size(100, 40);

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

        public CalendarWeekComponent()
        {            
            Renderer = new Win10CalendarWeekRenderer();            
        }

        protected void OnRefreshRequested()
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
