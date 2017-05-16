using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;

namespace Dimensional.TinyReturns.Core
{
    public class CalculateReturnRequest
    {
        public CalculateReturnRequest()
        {
            AnnualizeAction = AnnualizeActionEnum.Annualize;
        }

        public CalculateReturnRequest(
            MonthYear endMonth,
            int numberOfMonths): this()
        {
            EndMonth = endMonth;
            NumberOfMonths = numberOfMonths;
        }

        public MonthYear EndMonth { get; set; }
        public int NumberOfMonths { get; set; }
        public AnnualizeActionEnum AnnualizeAction { get; set; }

        public bool MonthsIsMoreThanYearAndAnnualizeActionSet()
        {
            return (NumberOfMonths > 12) && (AnnualizeAction == AnnualizeActionEnum.Annualize);
        }
    }
}