using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondaryTaskbarClock.Renderers
{
    public interface IWindowContentRenderer
    {
        /// <summary>
        /// Render the window content to the given graphics object
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void Render(Graphics g, RendererParameters parameters);
    }

    public class RendererParameters
    {
        /// <summary>
        /// Size of the window to render
        /// </summary>
        public Size WindowSize { get; private set; }

        /// <summary>
        /// The cursor is inside the window
        /// </summary>
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// The cursor is inside the window and the left mouse button is held down
        /// </summary>
        public bool IsPressed { get; private set; }

        public RendererParameters(int width, int height, bool isMouseOver, bool isPressed)
        {
            WindowSize = new Size(width, height);
            IsMouseOver = isMouseOver;
            IsPressed = isPressed;
        }
    }
}
