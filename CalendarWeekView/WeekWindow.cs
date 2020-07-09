using CalendarWeekView.Components;
using CalendarWeekView.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskBarExt;
using TaskBarExt.Utils;

namespace CalendarWeekView
{
    public partial class WeekWindow : TaskbarWindow
    {
        public WeekWindow(TaskbarRef targetTaskbar)
            : base(targetTaskbar, TaskbarWindowPlacement.BetweenTrayAndClock, new CalendarWeekComponent())
        {
            InitializeComponent();
        }

        private void WeekWindow_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
