using System;
using System.Configuration;
using System.Windows.Forms;
using NLog;

namespace StreamNotifier
{
    internal static class Program
    {
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {

            AppDomain.CurrentDomain.UnhandledException += (s, a) => _logger.Fatal(a.ExceptionObject);
            Application.ThreadException += (s, a) => _logger.Fatal(a.Exception);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainHandler.Run();
            Application.Run();
        }


    }
}