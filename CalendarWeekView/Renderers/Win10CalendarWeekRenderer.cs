using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBarExt.Renderers;
using TaskBarExt.Utils;

namespace CalendarWeekView.Renderers
{
    /// <summary>
    /// Draws a clock which looks like the system tray clock of Windows 10
    /// </summary>
    public class Win10CalendarWeekRenderer : ITaskbarComponentRenderer
    {
        static Color BgHighlightColor = Color.FromArgb(25, Color.White);
        Font Font { get; }
        Color FontColor { get; }

        string DisplayFormatString { get; }
        CalendarWeekCalculationRule WeekRule { get; }

        public Win10CalendarWeekRenderer(Font font, Color fontColor, String displayFormatString, CalendarWeekCalculationRule weekRule)
        {
            Font = font;
            FontColor = fontColor;
            DisplayFormatString = displayFormatString;
            WeekRule = weekRule;
        }

        public void Render(Graphics g, RendererParameters parameters)
        {
            if (parameters.IsMouseOver && !parameters.IsPressed)
                // mouse over highlight
                g.Clear(BgHighlightColor);

            // allow text smoothing
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var cWeek = CalendarWeek.GetCalendarWeek(DateTime.Today, WeekRule);            

            String text = DisplayFormatString.Replace("%week%", $"{cWeek.Week}").Replace("%year%", $"{cWeek.Year}");

            // select the largest variant for which there is enough space
            using (StringFormat sf = new StringFormat())
            using (var textBrush = new SolidBrush(FontColor))
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                Rectangle windowRect = new Rectangle(0, 0, parameters.WindowSize.Width, parameters.WindowSize.Height);
                g.DrawString(text, Font, textBrush, windowRect, sf);                
            }
        }
    }
}
