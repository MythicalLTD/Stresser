
using Salaros.Configuration;
using Stresser.Helpers.HLogger;

namespace Stresser.Helpers.ConfigHelper
{
    public class ConfigHelper
    {

        public static string? mysql_host;
        public static string? mysql_port;
        public static string? mysql_username;
        public static string? mysql_password;
        public static string? mysql_name;
        public static string GetSetting(string app, string setting)
        {
            try
            {
                string path = Path.Combine(Program.appWorkDir, "config.ini");
                if (File.Exists(path))
                {
                    var cfg = new ConfigParser(path);
                    var st = cfg.GetValue(app, setting);
                    return st;
                }
                else
                {
                    Program.hLogger.Log(LogType.Warning, "Config file not found!");
                    Program.Stop();
                    return "";
                }
            }
            catch (Exception ex)
            {
                Program.hLogger.Log(LogType.Error, "Failed to get setting: " + ex.Message);
                Program.Stop();
                return "";
            }
        }

        public static void UpdateSetting(string app, string setting, string value)
        {
            try
            {
                string path = Path.Combine(Program.appWorkDir, "config.ini");
                if (File.Exists(path))
                {
                    var cfg = new ConfigParser(path);
                    cfg.SetValue(app, setting, value);
                    cfg.Save();
                    Program.hLogger.Log(LogType.Info, $"Updated: {setting}");
                }
                else
                {
                    Program.hLogger.Log(LogType.Warning, "Config file not found!");
                    Program.Stop();
                }
            }
            catch (Exception ex)
            {
                Program.hLogger.Log(LogType.Error, "Failed to update settings: " + ex.Message);
                Program.Stop();
            }
        }
    }

}