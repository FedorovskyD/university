using Serilog;
using System;
using System.Data.Entity;
using System.Windows.Forms;
using University.model;

namespace University
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Конфигурируем логгер
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:w3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }
    }
}
