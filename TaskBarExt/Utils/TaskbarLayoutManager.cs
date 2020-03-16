using TaskBarExt.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBarExt.Utils
{
    /// <summary>
    /// Class which manages the layout of taskbar components (native and non-native)
    /// </summary>
    public class TaskbarLayoutManager
    {

        /// <summary>
        /// The taskbar this layout manager is assigned to
        /// </summary>
        public TaskbarRef Taskbar { get; private set; }

        public IList<ITaskbarComponent>  Components { get; private set; }

        private void CalculateLayout()
        {

        }
    }

    abstract class TaskbarLayoutElement
    {
        public Rectangle Bounds { get; set; }
    }

    public enum FillMode
    {
        None,
        Stretch
    }
}
