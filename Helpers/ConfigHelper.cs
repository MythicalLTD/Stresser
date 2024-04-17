using System;
using System.IO;
using Salaros.Configuration;
using Stresser.Helpers.HLogger;

namespace Stresser.Helpers.ConfigHelper
{
    public class ConfigHelper
    {
        public static string GetSetting(string app, string setting)
        {
            string path = Path.Combine(Program.appWorkDir, "config.ini");

            try
            {
                if (!File.Exists(path))
                {
                    CreateDefaultConfigFile(path);
                }

                var cfg = new ConfigParser(path);
                var st = cfg.GetValue(app, setting);
                return st;
            }
            catch (Exception ex)
            {
                CreateDefaultConfigFile(path);

                Program.hLogger.Log(LogType.Error, "Failed to get setting: " + ex.Message);
                Program.Stop();
                return "";
            }
        }

        public static void UpdateSetting(string app, string setting, string value)
        {
            string path = Path.Combine(Program.appWorkDir, "config.ini");
            try
            {
                if (!File.Exists(path))
                {
                    CreateDefaultConfigFile(path);
                }

                var cfg = new ConfigParser(path);
                cfg.SetValue(app, setting, value);
                cfg.Save();
                Program.hLogger.Log(LogType.Info, $"Updated: {setting}");
            }
            catch (Exception ex)
            {
                CreateDefaultConfigFile(path);
                Program.hLogger.Log(LogType.Error, "Failed to update settings: " + ex.Message);
                Program.Stop();
            }
        }

        private static void CreateDefaultConfigFile(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("[webserver]");
                    sw.WriteLine("port=1492");
                    sw.WriteLine("host=0.0.0.0");
                    sw.WriteLine("token=test");

                    sw.WriteLine("[os]");
                    sw.WriteLine("forcewinsupport=false");
                    sw.WriteLine("savelogs=false");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error creating config file: " + ex.Message);
            }
        }
    }
}
