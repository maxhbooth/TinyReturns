using System.Collections.Generic;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport
{
    public class GrowthofWealthSeries
    {
        //need monthlyseries and growthofwealth for each month
        public MonthlyGrowthOfWealth[] MonthlyGrowthOfWealthReturn;

        private readonly FinancialMath _financialMath;
        
        public GrowthofWealthSeries(
            ReturnSeries returnSeries )
        {
            _financialMath = new FinancialMath();
            MonthlyGrowthOfWealthReturn = TransformIntoGrowthOfWealthReturns(returnSeries.GetMonthlyReturns());
        }

        private MonthlyGrowthOfWealth[] TransformIntoGrowthOfWealthReturns(ReturnSeries.MonthlyReturn[] monthlyReturns)
        {
            var monthlyGrowthOfWealthReturns = new List<MonthlyGrowthOfWealth>();


            decimal? calculatedChangeOfWealth = null;

            foreach (var monthlyReturn in monthlyReturns)
            {

                if (calculatedChangeOfWealth == null)
                {
                    calculatedChangeOfWealth = monthlyReturn.Value;
                }
                else
                {
                    var changeOfWealthOverLastMonth = new decimal[]
                    {
                        calculatedChangeOfWealth.Value, monthlyReturn.Value
                    };

                    calculatedChangeOfWealth = _financialMath.PerformGeometricLinking(changeOfWealthOverLastMonth).Value;

                }

                monthlyGrowthOfWealthReturns.Add(new MonthlyGrowthOfWealth(monthlyReturn.MonthYear, calculatedChangeOfWealth.Value));

            }

            return monthlyGrowthOfWealthReturns.ToArray();
        }


        public class MonthlyGrowthOfWealth
        {
            public MonthlyGrowthOfWealth(
                MonthYear monthYear,
                decimal value)
            {
                Value = value;
                MonthYear = monthYear;
            }

            public MonthYear MonthYear { get; private set; }
            public decimal Value { get; private set; }
        }
    }
}