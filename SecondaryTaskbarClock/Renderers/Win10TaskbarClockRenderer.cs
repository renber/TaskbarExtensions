using SecondaryTaskbarClock.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondaryTaskbarClock.Renderers
{
    /// <summary>
    /// Draws a clock which looks like the system tray clock of Windows 10
    /// </summary>
    public class Win10TaskbarClockRenderer : IWindowContentRenderer
    {                      
        static Color BgHighlightColor = Color.FromArgb(25, Color.White);
        static Font font = new Font("Segoe UI", 9);

        ClockViewModel ViewModel { get; set; }

        public Win10TaskbarClockRenderer(ClockViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void Render(Graphics g, RendererParameters parameters)
        {
            if (parameters.IsMouseOver && !parameters.IsPressed)
                // mouse over highlight
                g.Clear(BgHighlightColor);            

            // allow text smoothing
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;            

            // top line
            Rectangle topRect = new Rectangle(0, 0, parameters.WindowSize.Width, parameters.WindowSize.Height / 2);            
            g.DrawString(ViewModel.CurrentDateTime.ToShortTimeString(), font, Brushes.White, topRect, sf);

            // bottom line
            Rectangle bottomRect = new Rectangle(0, parameters.WindowSize.Height / 2, parameters.WindowSize.Width, parameters.WindowSize.Height / 2);            
            g.DrawString(ViewModel.CurrentDateTime.ToShortDateString(), font, Brushes.White, bottomRect, sf);
        }
    }
}
