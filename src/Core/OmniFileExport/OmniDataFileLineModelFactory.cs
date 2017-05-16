using System;
using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.DateExtend;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PublicWebReport;

namespace Dimensional.TinyReturns.Core.OmniFileExport
{
    public class OmniDataFileLineModelFactory
    {
        private PortfolioWithPerformance _portfolio;
        private readonly MonthYearRange _januaryToGivenMonth;
        private readonly MonthYear _endMonth;

        public OmniDataFileLineModelFactory(
            MonthYear endEndMonth)
        {
            _endMonth = endEndMonth;
            _januaryToGivenMonth = CreateRangeFromJanuaryToGivenMonth(endEndMonth);
        }

        public void SetCurrentPortfolio(
            PortfolioWithPerformance portfolio)
        {
            _portfolio = portfolio;
        }

        public IEnumerable<OmniDataFileLineModel> CreateNetMonthLineModels()
        {
            var netMonthlyReturns = _portfolio.GetNetReturnsInRange(_januaryToGivenMonth);

            return netMonthlyReturns.Select(r => CreateMonthModel(r, "N"));
        }

        public IEnumerable<OmniDataFileLineModel> CreateGrossMonthLineModels()
        {
            var netMonthlyReturns = _portfolio.GetGrossReturnsInRange(_januaryToGivenMonth);

            return netMonthlyReturns.Select(r => CreateMonthModel(r, "G"));
        }

        public IEnumerable<OmniDataFileLineModel> CreateNetQuarterLineModels()
        {
            var models = new List<OmniDataFileLineModel>();

            _januaryToGivenMonth.ForEachMonthInRange(
                m =>
                {
                    if (m.IsLastMonthOfQuarter)
                    {
                        var calculateQuarterRequest = CalculateReturnRequestFactory.ThreeMonth(m);

                        var netQuarterResult = _portfolio.CalculateNetReturn(calculateQuarterRequest);

                        if (!netQuarterResult.HasError)
                            models.Add(CreateQuarterModel(m, netQuarterResult, "N"));
                    }
                });

            return models;
        }

        public IEnumerable<OmniDataFileLineModel> CreateGrossQuarterLineModels()
        {
            var models = new List<OmniDataFileLineModel>();

            _januaryToGivenMonth.ForEachMonthInRange(
                m =>
                {
                    if (m.IsLastMonthOfQuarter)
                    {
                        var calculateQuarterRequest = CalculateReturnRequestFactory.ThreeMonth(m);

                        var netQuarterResult = _portfolio.CalculateGrossReturn(calculateQuarterRequest);

                        if (!netQuarterResult.HasError)
                            models.Add(CreateQuarterModel(m, netQuarterResult, "G"));
                    }
                });

            return models;
        }

        public OmniDataFileLineModel CreateNetYearToDateLineModel()
        {
            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(_endMonth);

            var yearToDateResult = _portfolio.CalculateNetReturn(yearToDateRequest);

            if (yearToDateResult.HasError)
                return null;

            return CreateYearToDateModel(_endMonth, yearToDateResult, "N");
        }
        public OmniDataFileLineModel CreateGrossYearToDateLineModel()
        {
            var yearToDateRequest = CalculateReturnRequestFactory.YearToDate(_endMonth);

            var yearToDateResult = _portfolio.CalculateGrossReturn(yearToDateRequest);

            if (yearToDateResult.HasError)
                return null;

            return CreateYearToDateModel(_endMonth, yearToDateResult, "G");
        }


        private OmniDataFileLineModel CreateMonthModel(
            ReturnSeries.MonthlyReturn r,
            string feeTypeCode)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = feeTypeCode;
            m.Duration = "M";
            m.EndDate = FormatDate(r.MonthYear.LastDayOfMonth);
            m.ReturnValue = r.Value.ToString();

            return m;
        }

        private OmniDataFileLineModel CreateQuarterModel(
            MonthYear monthYear,
            ReturnResult result,
            string feeTypeCode)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = feeTypeCode;
            m.Duration = "Q";
            m.EndDate = FormatDate(monthYear.LastDayOfMonth);
            m.ReturnValue = result.Value.ToString();

            return m;
        }

        private OmniDataFileLineModel CreateYearToDateModel(
            MonthYear monthYear,
            ReturnResult result,
            string typeCode)
        {
            var m = CreateModelWithPortfolioPopulated();

            m.FeeType = typeCode;
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