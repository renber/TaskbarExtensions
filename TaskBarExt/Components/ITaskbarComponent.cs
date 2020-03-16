using TaskBarExt.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBarExt.Components
{
    public interface ITaskbarComponent
    {
        /// <summary>
        /// The preferred size of this component
        /// </summary>
        Size PreferredSize { get; }

        /// <summary>
        /// The renderer to use for this component
        /// </summary>
        ITaskbarComponentRenderer Renderer { get; }

        /// <summary>
        /// Defines when the component wants to be redrawn automatically
        /// </summary>
        RefreshBehavior RefreshBehavior { get; }

        /// <summary>
        /// Fired when the component wants itself to be rerendered
        /// </summary>
        event EventHandler RefreshRequested;
    }

    /// <summary>
    /// Indicates when a taskbar component wants to be redrawn
    /// </summary>
    [Flags]
    public enum RefreshBehavior
    {
        Explicit = 0,
        MouseEnter = 1,
        MouseLeave = 2,
        MouseMove = 4,
        MouseDown = 8,
        MouseUp = 16
    }
}
