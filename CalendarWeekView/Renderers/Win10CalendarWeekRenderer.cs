using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Renderers;

namespace CalendarWeekView.Renderers
{
    /// <summary>
    /// Draws a clock which looks like the system tray clock of Windows 10
    /// </summary>
    public class Win10CalendarWeekRenderer : ITaskbarComponentRenderer
    {
        static Color BgHighlightColor = Color.FromArgb(25, Color.White);
        static Font font = new Font(new FontFamily("Segoe UI Symbol"), 9f, FontStyle.Regular, GraphicsUnit.Point);

        public Win10CalendarWeekRenderer()
        {
            // --
        }

        public void Render(Graphics g, RendererParameters parameters)
        {
            if (parameters.IsMouseOver && !parameters.IsPressed)
                // mouse over highlight
                g.Clear(BgHighlightColor);

            // allow text smoothing
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var cWeek = CalendarWeek.GetGermanCalendarWeek(DateTime.Today);

            // depending on the amount of space, we either print
            // one, two or three lines            
            String[] lineVariants = new string[]
            {
                // three lines
                //$"Kalenderwoche\n{cWeek.Week:00}, {cWeek.Year}",                

                // two lines
                $"KW {cWeek.Week:00}\n{cWeek.Year}",                

                // one line
                $"KW {cWeek.Week:00}"
            };

            // select the largest variant for which there is enough space
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;            

            int variant = 0;
            while (variant < lineVariants.Length - 1
                  && g.MeasureString(lineVariants[variant], font, int.MaxValue, sf).Height > parameters.WindowSize.Height - 5)
            {
                variant++;
            }

            Rectangle windowRect = new Rectangle(0, 0, parameters.WindowSize.Width, parameters.WindowSize.Height);
            g.DrawString(lineVariants[variant], font, Brushes.White, windowRect, sf);
        }
    }
}
