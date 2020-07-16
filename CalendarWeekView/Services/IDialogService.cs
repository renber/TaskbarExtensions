using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWeekView.Services
{
    public interface IDialogService
    {
        void ShowSettingsDialog();

        bool ShowFontDialog(Font initialFont, Color initialColor, out Tuple<Font, Color> selection);

    }
}
