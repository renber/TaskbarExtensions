using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarWeekView.Types
{
    /// <summary>
    /// Verwaltet die Daten einer Kalenderwoche
    /// </summary>
    public struct CalendarWeek : IComparable, IComparable<CalendarWeek>
    {
        /// <summary>
        /// Das Jahr
        /// </summary>
        public int Year;

        /// <summary>
        /// Die Kalenderwoche
        /// </summary>
        public int Week;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="year">Das Jahr</param>
        /// <param name="week">Die Kalenderwoche</param>
        public CalendarWeek(int year, int week)
        {
            this.Year = year;
            this.Week = week;
        }

        public override string ToString()
        {
            return String.Format("{0:d}", this.Week);
        }

        public int CompareTo(CalendarWeek obj)
        {
            if (obj.Year > this.Year)
                return -1;

            if (obj.Year == this.Year)
                return Week.CompareTo(obj.Week);

            return 1;
        }

        public int CompareTo(Object obj)
        {
            if (obj is CalendarWeek)
                return CompareTo((CalendarWeek)obj);
            else
                return 0;
        }

        public string GetDescription()
        {
            return String.Format("KW {0:d}", this.Week);
        }


        public int GetSingleValue()
        {
            return Week;
        }

        public static CalendarWeek GetCalendarWeek(DateTime date, CalendarWeekCalculationRule rule)
        {
            switch(rule)
            {
                case CalendarWeekCalculationRule.ISO8601: return GetISO8601CalendarWeek(date);
                case CalendarWeekCalculationRule.US: return GetUSCalendarWeek(date);
                default:
                    throw new ArgumentException($"Unsupported CalendarWeekRule: {rule}");
            }
        }

        /// <summary>
        /// Berechnet die deutsche Kalenderwoche für das angegebene Datum
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static CalendarWeek GetISO8601CalendarWeek(DateTime date)
        {
            double a = Math.Floor((14 - (date.Month)) / 12D);
            double y = date.Year + 4800 - a;
            double m = (date.Month) + (12 * a) - 3;

            double jd = date.Day + Math.Floor(((153 * m) + 2) / 5) +
               (365 * y) + Math.Floor(y / 4) - Math.Floor(y / 100) +
               Math.Floor(y / 400) - 32045;

            double d4 = (jd + 31741 - (jd % 7)) % 146097 % 36524 %
               1461;
            double L = Math.Floor(d4 / 1460);
            double d1 = ((d4 - L) % 365) + L;

            // Kalenderwoche ermitteln
            int calendarWeek = (int)Math.Floor(d1 / 7) + 1;

            // Das Jahr der Kalenderwoche ermitteln
            int year = date.Year;
            if (calendarWeek == 1 && date.Month == 12)
                year++;
            if (calendarWeek >= 52 && date.Month == 1)
                year--;

            // Die ermittelte Kalenderwoche zurückgeben
            return new CalendarWeek(year, calendarWeek);
        }

        private static CalendarWeek GetUSCalendarWeek(DateTime date)
        {
            CultureInfo usCulture = new CultureInfo("en-US");
            Calendar usCal = usCulture.Calendar;
            CalendarWeekRule cwr = usCulture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dow = usCulture.DateTimeFormat.FirstDayOfWeek;

            return new CalendarWeek(date.Year, usCal.GetWeekOfYear(DateTime.Now, cwr, dow));
        }
    }

    public enum CalendarWeekCalculationRule
    {
        US,
        ISO8601
    }
}
