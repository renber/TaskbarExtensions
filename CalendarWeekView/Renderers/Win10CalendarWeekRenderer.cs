using CalendarWeekView.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

            using (var bmp = new Bitmap(parameters.WindowSize.Width, parameters.WindowSize.Height, PixelFormat.Format32bppArgb))
            {
                // CLeartype does not work with transparent backgrounds
                // -> draw it on solid black and convert it to transparent afterwards
                // decide on Black/WHite depending on brightness of FontColor?
                Color chromaColor = FontColor == Color.Black ? Color.White : Color.Black;
                Color fontdrawColor = FontColor == Color.Black ? Color.Black : Color.White;

                using (var gbmp = Graphics.FromImage(bmp))
                {                   
                    gbmp.Clear(chromaColor);
                             
                    gbmp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gbmp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gbmp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;                    

                    var cWeek = CalendarWeek.GetCalendarWeek(DateTime.Today, WeekRule);

                    String text = DisplayFormatString.Replace("%week%", $"{cWeek.Week}").Replace("%year%", $"{cWeek.Year}");

                    int padding = 3;
                    int linePadding = 2;
                    int availableHeight = parameters.WindowSize.Height - 2 * padding;

                    String[] lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    // get maximum number of lines which can be displayed
                    if (lines.Length > 0)
                    {
                        int lineHeight = (int)gbmp.MeasureString("W", Font).Height;
                        int maxLines = availableHeight / lineHeight;
                        int linesToDraw = Math.Min(maxLines, lines.Length);
                        int heightPerLine = availableHeight / linesToDraw;

                        using (StringFormat sf = StringFormat.GenericTypographic)
                        using (var textBrush = new SolidBrush(fontdrawColor))
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;

                            for (int i = 0; i < linesToDraw; i++)
                            {
                                var windowRect = new Rectangle(0, padding + i * (heightPerLine + linePadding), parameters.WindowSize.Width, heightPerLine);
                                gbmp.DrawString(lines[i], Font, textBrush, windowRect, sf);
                            }
                        }                        
                    }
                }
                
                RemoveChroma(bmp, FontColor, chromaColor);
                g.DrawImage(bmp, 0, 0);
            }                                
        }

        /// <summary>
        /// Adapted from https://stackoverflow.com/questions/2991490/bad-text-rendering-using-drawstring-on-top-of-transparent-pixels
        /// (Only works for White atm)
        /// </summary>        
        static unsafe void RemoveChroma(Bitmap image, Color foregroundColor, Color chromaColor)
        {
            if (image == null) throw new ArgumentNullException("image");
            BitmapData data = null;

            try
            {
                data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                for (int y = data.Height - 1; y >= 0; --y)
                {
                    int* row = (int*)(data.Scan0 + (y * data.Stride));
                    for (int x = data.Width - 1; x >= 0; --x)
                    {
                        if (row[x] == 0) continue;
                        Color pixel = Color.FromArgb(row[x]);

                        if (pixel.R == chromaColor.R && pixel.G == chromaColor.G && pixel.B == chromaColor.B && pixel.A == 255)
                        {
                            row[x] = 0;
                        }
                        else
                        {
                            row[x] = Color.FromArgb(
                              255 - ((int)
                                ((Math.Abs(pixel.B - foregroundColor.B) +
                                  Math.Abs(pixel.G - foregroundColor.G) +
                                  Math.Abs(pixel.R - foregroundColor.R)) / 3)),
                              foregroundColor).ToArgb();
                        }
                    }
                }
            }
            finally
            {
                if (data != null) image.UnlockBits(data);
            }
        }
    }
}
