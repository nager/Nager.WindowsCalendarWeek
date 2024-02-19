using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Nager.WindowsCalendarWeek
{
    internal static class Program
    {
        private static Timer RefreshIconTimer;
        private static NotifyIcon CalendarWeekNotifyIcon;
        private static int OneDayMilliseconds = 86400000;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var firstRefreshInterval = (int)(OneDayMilliseconds - Math.Floor(DateTime.Now.TimeOfDay.TotalMilliseconds));

            RefreshIconTimer = new Timer();
            RefreshIconTimer.Interval = firstRefreshInterval;
            RefreshIconTimer.Tick += new EventHandler(RefreshIconTimer_Tick);
            RefreshIconTimer.Start();

            var bmp = GetBitmapOfCurrentCalendarWeek();

            CalendarWeekNotifyIcon = new NotifyIcon
            {
                Icon = Icon.FromHandle(bmp.GetHicon()),
                ContextMenuStrip = new ContextMenuStrip
                {
                    Items =
                    {
                        new ToolStripMenuItem("Online Kalender", null, new EventHandler(OnlineCalendarItemClickEvent), "ToolStripMenuItem_OnlineCalendar"),
                        new ToolStripMenuItem("Feiertage", null, new EventHandler(HolidayItemClickEvent), "ToolStripMenuItem_Holiday"),
                        new ToolStripMenuItem("Quit", null, new EventHandler(QuitItemClickEvent), "ToolStripMenuItem_Quit")
                    }
                },
                Visible = true
            };

            CalendarWeekNotifyIcon.MouseClick += TrayIcon_Click;

            Application.Run();
        }

        private static void RefreshIconTimer_Tick(object sender, EventArgs e)
        {
            RefreshIconTimer.Interval = OneDayMilliseconds;

            var oldIcon = CalendarWeekNotifyIcon.Icon;
            if (oldIcon != null)
            {
                oldIcon.Dispose();
            }

            var bmp = GetBitmapOfCurrentCalendarWeek();
            CalendarWeekNotifyIcon.Icon = Icon.FromHandle(bmp.GetHicon());
        }

        private static void TrayIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OpenOnlineCalendar();
            }
        }

        private static void OnlineCalendarItemClickEvent(object sender, EventArgs e)
        {
            OpenOnlineCalendar();
        }

        private static void HolidayItemClickEvent(object sender, EventArgs e)
        {
            OpenHoliday();
        }

        private static void OpenOnlineCalendar()
        {
            // show first or second half of the year
            var ext = DateTime.Today.Month > 6 ? "-2" : "";

            var target = $"https://kalenderwoche.at/kalender/{DateTime.Today.Year}{ext}";
            System.Diagnostics.Process.Start(target);
        }

        private static void OpenHoliday()
        {
            var target = $"https://date.nager.at/";
            System.Diagnostics.Process.Start(target);
        }

        private static Bitmap GetBitmapOfCurrentCalendarWeek()
        {
            var width = 16;
            var height = 16;
            var text = GetWeekNumber().ToString();

            var bitmap = new Bitmap(width, height);
            using (var font = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point))
            {
                using (var drawBrush = new SolidBrush(Color.White))
                {
                    using (var stringFormat = new StringFormat())
                    {
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            var drawingArea = new Rectangle(0, 0, width, height);

                            graphics.FillRectangle(Brushes.Black, 0, 0, width, height);
                            graphics.DrawString(text, font, drawBrush, drawingArea, stringFormat);
                        }
                    }
                }
            }

            return bitmap;
        }

        private static int GetWeekNumber()
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static void QuitItemClickEvent(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
