using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Nager.WindowsCalendarWeek
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TODO: Add logic to detect day change and redraw icon

            var bmp = GetBitmapOfCurrentCalendarWeek();

            var trayIcon = new NotifyIcon
            {
                Icon = Icon.FromHandle(bmp.GetHicon()),
                ContextMenuStrip = new ContextMenuStrip
                {
                    Items =
                    {
                        new ToolStripMenuItem("Online Kalender", null, new EventHandler(DetailItemClickEvent), "Online Kalendar"),
                        new ToolStripMenuItem("Quit", null, new EventHandler(QuitItemClickEvent), "Quit")
                    }
                },
                Visible = true
            };

            trayIcon.MouseClick += TrayIcon_Click;

            Application.Run();
        }

        private static void TrayIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OpenOnlineCalendar();
            }
        }

        private static void DetailItemClickEvent(object sender, EventArgs e)
        {
            OpenOnlineCalendar();
        }

        private static void OpenOnlineCalendar()
        {
            var ext = DateTime.Today.Month > 6 ? "-2" : "";

            var target = $"https://kalenderwoche.at/kalender/{DateTime.Today.Year}{ext}";
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
