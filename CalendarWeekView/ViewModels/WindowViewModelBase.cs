using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWeekView.ViewModels
{
    class WindowViewModelBase : ViewModelBase
    {

        public event EventHandler ViewCloseRequested;

        protected void RequestViewClose()
        {
            ViewCloseRequested?.Invoke(this, EventArgs.Empty);
        }

    }
}
