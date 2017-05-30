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

            var previousMonthYear = new MonthYear(2016, 12);

            omniDataFileCreator.CreateFile(previousMonthYear, GetUniqueFileName());
        }

        private static MonthYear GetPreviousMonth()
        {
            var currentMonthYear = new MonthYear(DateTime.Now);
            var previousMonthYear = currentMonthYear.AddMonths(-1);
            return previousMonthYear;
        }

        private static string GetUniqueFileName()
        {
            return "c:\\temp\\OminData_" + Guid.NewGuid() + ".txt";
        }
    }
}
