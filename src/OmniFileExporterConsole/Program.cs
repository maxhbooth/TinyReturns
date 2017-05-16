using System;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.DependencyManagement;

namespace Dimensional.TinyReturns.OmniFileExporterConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DependencyManager.BootstrapForSystem("OmniFileExporterConsole", new DatabaseSettings());

            var omniDataFileCreator = MasterFactory.GetOmniDataFileCreator();

            omniDataFileCreator.CreateFile(new MonthYear(2012, 6), GetUniqueFileName());
        }

        private static string GetUniqueFileName()
        {
            return "c:\\temp\\OminData_" + Guid.NewGuid() + ".txt";
        }
    }
}
