using System.IO;
using log4net.Config;

namespace Dimensional.TinyReturns.Logging
{
    public static class LoggingBootstrapper
    {
        public static void StartupLog()
        {
            var configFile = new FileInfo(GetLogConfigFile());

            XmlConfigurator.Configure(configFile);
        }

        private static string GetLogConfigFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\Log4NetConfig.xml";

            return targetFile;
        }

    }
}