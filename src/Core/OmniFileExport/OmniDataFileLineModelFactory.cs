using System;
using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;

namespace Dimensional.TinyReturns.Core.OmniFileExport
{
    public class OmniDataFileLineModelFactory
    {
        private InvestmentVehicle _portfolio;
        private readonly MonthYearRange _januaryToGivenMonth;
        private readonly MonthYear _endMonth;

        public OmniDataFileLineModelFactory(
            MonthYear endEndMonth)
        {
            _endMonth = endEndMonth;
            _januaryToGivenMonth = CreateRangeFromJanuaryToGivenMonth(endEndMonth);
        }

        public void SetCurrentPortfolio(
            InvestmentVehicle portfolio)
        {
            _portfolio = portfolio;
        }

        public IEnumerable<OmniDataFileLineModel> CreateMonthLineModels(
            FeeType feeType)
        {
            var netMonthlyReturns = _portfolio.GetReturnsInRange(_januaryToGivenMonth, feeType);

            return netMonthlyReturns.Select(r => CreateMonthModel(r, feeType));
        }

        public IEnumerable<OmniDataFileLineModel> CreateQuarterLineModels(FeeType feeType)
        {
            var models = new List<OmniDataFileLineModel>();

            _januaryToGivenMonth.ForEachMonthInRange(
                m =>
                {
                    if (m.IsLastMonthOfQuarter)
                    {
                        var calculateQuarterRequest = CalculateReturnRequestFactory.ThreeMonth(m);

                        var netQuarterResult = _portfolio.CalculateReturn(calculateQuarterRequest, feeType);

                        if (!netQuarterResult.HasError)
                            models.Add(CreateQuarterModel(m, netQuarterResult, feeType));
                    }
                });

            return models;
        }

        public OmniDataFileLineModel CreateYearToDateLineModel(
            FeeType feeType)
        {
            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(_endMonth);

            var yearToDateResult = _portfolio.CalculateReturn(yearToDateRequest, feeType);

            if (yearToDateResult.HasError)
                return null;

            return CreateYearToDateModel(_endMonth, yearToDateResult, feeType);
        }

        private OmniDataFileLineModel CreateMonthModel(
            MonthlyReturn r,
            FeeType feeType)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = feeType.Code.ToString();
            m.Duration = "M";
            m.EndDate = FormatDate(r.MonthYear.LastDayOfMonth);
            m.ReturnValue = r.ReturnValue.ToString();

            return m;
        }

        private OmniDataFileLineModel CreateQuarterModel(
            MonthYear monthYear,
            ReturnResult result,
            FeeType feeType)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = feeType.Code.ToString();
            m.Duration = "Q";
            m.EndDate = FormatDate(monthYear.LastDayOfMonth);
            m.ReturnValue = result.Value.ToString();

            return m;
        }

        private OmniDataFileLineModel CreateYearToDateModel(
            MonthYear monthYear,
            ReturnResult result,
            FeeType feeType)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = feeType.Code.ToString();
            m.Duration = "Y";
            m.EndDate = FormatDate(monthYear.LastDayOfMonth);
            m.ReturnValue = result.Value.ToString();

            return m;
        }

        private OmniDataFileLineModel CreateModelWithPortfolioPopulated()
        {
            return new OmniDataFileLineModel()
            {
                InvestmentVehicleId = _portfolio.Number.ToString(),
                Type = "Portfolio",
                Name = _portfolio.Name
            };
        }

        private string FormatDate(
            DateTime date)
        {
            return date.ToString("yyyy-M-d");
        }

        private MonthYearRange CreateRangeFromJanuaryToGivenMonth(
            MonthYear monthYear)
        {
            return new MonthYearRange(new MonthYear(monthYear.Year, 1), monthYear);
        }
    }
}