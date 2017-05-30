using System;
using System.IO;
using System.Reflection;
using log4net.Config;

namespace Dimensional.TinyReturns.Logging
{
    public static class LoggingBootstrapper
    {
        public static void StartupLog(
            string logName)
        {
            var configFile = new FileInfo(GetLogConfigFile());

            log4net.GlobalContext.Properties["LogName"] = logName;
            XmlConfigurator.Configure(configFile);
        }

        private static string GetLogConfigFile()
        {
            var targetFile = Path.Combine(GetDirectoryOfExecutingAssembly(), "Log4NetConfig.xml");

            return targetFile;
        }

        private static string GetDirectoryOfExecutingAssembly()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

    }
}