using System;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;

namespace WareMaster
{
    public static class WMLogger
    {
        public static void WriteLog(string message)
        {
            try
            {
                string logPath = ConfigurationManager.AppSettings["logPath"];
                string logfile = Path.Combine(logPath, "log.txt"); // Combine with file name
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                if (!File.Exists(logfile))
                {
                    File.WriteAllText(logfile, $"{DateTime.Now} : {message}{Environment.NewLine}");
                }
                else
                {
                    File.AppendAllText(logfile, $"{DateTime.Now} : {message}{Environment.NewLine}");
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
