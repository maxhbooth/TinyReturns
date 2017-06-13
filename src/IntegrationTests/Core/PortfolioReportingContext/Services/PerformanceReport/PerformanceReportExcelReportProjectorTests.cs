using System;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PerformanceReport;
using Dimensional.TinyReturns.Core.SharedContext.Services.DateExtend;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Database.TinyReturnsDatabase.Portfolio;
using FluentAssertions;
using Xunit;
using System.Diagnostics;
using System.Linq;

namespace Dimensional.TinyReturns.IntegrationTests.Core.PortfolioReportingContext.Services.PerformanceReport
{
    public class PerformanceReportExcelReportProjectorTests
    {
        public class TestHelper
        {
            private readonly AllTablesDeleter _allTablesDeleter;
            private readonly PortfolioDataTableGateway _portfolioDataTableGateway;
            private readonly ReturnSeriesDataTableGateway _returnSeriesDataTableGateway;
            private readonly MonthlyReturnDataTableGateway _monthlyReturnDataTableGateway;
            private readonly PortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
            private readonly PerformanceReportExcelReportViewSpy _performanceReportExcelReportViewSpy;
            private readonly PortfolioToBenchmarkDataTableGateway _portfolioToBenchmarkDataTableGateway;
            private readonly CountriesDataTableGateway _countriesDataTableGateway;
            private readonly BenchmarkDataTableGateway _benchmarkDataTableGateway;
            private readonly BenchmarkToReturnSeriesDataTableGateway _benchmarkToReturnSeriesDataTableGateway;

            public TestHelper()
            {
                var databaseSettings = new DatabaseSettings();
                var systemLogForIntegrationTests = new SystemLogForIntegrationTests();

                _allTablesDeleter = new AllTablesDeleter();

                _portfolioDataTableGateway = new PortfolioDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _returnSeriesDataTableGateway = new ReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _monthlyReturnDataTableGateway = new MonthlyReturnDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioToReturnSeriesDataTableGateway = new PortfolioToReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _countriesDataTableGateway = new CountriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _benchmarkDataTableGateway = new BenchmarkDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _benchmarkToReturnSeriesDataTableGateway = new BenchmarkToReturnSeriesDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _portfolioToBenchmarkDataTableGateway = new PortfolioToBenchmarkDataTableGateway(
                    databaseSettings,
                    systemLogForIntegrationTests);

                _performanceReportExcelReportViewSpy = new PerformanceReportExcelReportViewSpy();
            }

            public PerformanceReportExcelReportViewSpy PerformanceReportExcelReportViewSpy
            {
                get { return _performanceReportExcelReportViewSpy; }
            }

            public void InsertPortfolioDto(
                PortfolioDto dto)
            {
                _portfolioDataTableGateway.Insert(dto);
            }

            public int InsertReturnSeriesDto(ReturnSeriesDto dto)
            {
                return _returnSeriesDataTableGateway.Insert(dto);
            }

            public void InsertMonthlyReturnDtos(MonthlyReturnDto[] dtos)
            {
                _monthlyReturnDataTableGateway.Insert(dtos);
            }

            public void InsertPortfolioToReturnSeriesDto(PortfolioToReturnSeriesDto dto)
            {
                _portfolioToReturnSeriesDataTableGateway.Insert(new[] { dto });
            }

            public void DatabaseDataDeleter(
                Action act)
            {
                var databaseSettings = new DatabaseSettings();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);

                act();

                _allTablesDeleter.DeleteAllDataFromTables(
                    databaseSettings.TinyReturnsDatabaseConnectionString,
                    new AllTablesDeleter.TableInfoDto[0]);
            }

            public PerformanceReportExcelReportPresenter CreatePresenter()
            {
                var returnSeriesRepository = new ReturnSeriesRepository(
                    _returnSeriesDataTableGateway,
                    _monthlyReturnDataTableGateway);

                var benchmarkWithPerformanceRepository = new BenchmarkWithPerformanceRepository(
                    _benchmarkDataTableGateway,
                    _benchmarkToReturnSeriesDataTableGateway,
                    returnSeriesRepository);

                var portfolioWithPerformanceRepository = new PortfolioWithPerformanceRepository(
                    _portfolioDataTableGateway,
                    _portfolioToReturnSeriesDataTableGateway,
                    _portfolioToBenchmarkDataTableGateway,
                    _countriesDataTableGateway,
                    returnSeriesRepository,
                    benchmarkWithPerformanceRepository);

                return new PerformanceReportExcelReportPresenter(
                    portfolioWithPerformanceRepository,
                    _performanceReportExcelReportViewSpy);
            }
        }

        [Fact]
        public void ShouldPrintNoModelsWhenNoPortfoliosExists()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var presenter = testHelper.CreatePresenter();

                presenter.CreateReport(
                    new MonthYear(2010, 1),
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();
                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(0);
                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 1/2010");
            });
        }

        [Fact]
        public void ShouldPrintNetOnlyRecordForPortrfolio()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var presenter = testHelper.CreatePresenter();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var returnSeriesId = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesId,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var monthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesId,
                    monthYearRange);

                testHelper.InsertMonthlyReturnDtos(monthlyReturnDtos);

                presenter.CreateReport(
                    monthYear,
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();

                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(1);

                var recordModel = viewSpy.PerformanceReportExcelReportModel.Records[0];

                recordModel.EntityNumber.Should().Be(portfolioNumber);
                recordModel.Name.Should().Be(portfolioName);
                recordModel.Type.Should().Be("Portfolio");
                recordModel.FeeType.Should().Be("Net");

                recordModel.OneMonth.Should().BeApproximately(-0.4162m, 0.00000001m);
                recordModel.ThreeMonths.Should().BeApproximately(-0.375236505m, 0.00000001m);
                recordModel.TwelveMonths.Should().NotHaveValue();
                recordModel.YearToDate.Should().BeApproximately(0.0010907851939m, 0.00000001m);
                recordModel.FirstFullMonth.Should().NotHaveValue();
                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 5/2016");
            });
        }

        [Fact]
        public void ShouldPrintNetAndGrossRecordsForPortrfolio()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var presenter = testHelper.CreatePresenter();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2016, 1, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                var returnSeriesIdGross = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-10),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                foreach (var monthlyReturn in netMonthlyReturnDtos)
                {
                    Debug.WriteLine("Net Returns:");
                    Debug.WriteLine(monthlyReturn.ReturnValue);
                }

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdGross,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.GrossSeriesTypeCode
                });

                var grossMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdGross,
                    monthYearRange,
                    seed: 2);

                foreach (var monthlyReturn in grossMonthlyReturnDtos)
                {
                    Debug.WriteLine("Gross Returns:");
                    Debug.WriteLine(monthlyReturn.ReturnValue);
                }

                
                testHelper.InsertMonthlyReturnDtos(grossMonthlyReturnDtos);

                presenter.CreateReport(
                    monthYear,
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();

                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(2);

                var netRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[0];

                netRecordModel.EntityNumber.Should().Be(portfolioNumber);
                netRecordModel.Name.Should().Be(portfolioName);
                netRecordModel.Type.Should().Be("Portfolio");
                netRecordModel.FeeType.Should().Be("Net");

                var expectedNetSixMonth = (1 - 0.4162m) * (1 - 0.4526m) * (1 + 0.955m)
                                            * (1 - 0.1157m) * (1 + 0.812m) * (1 + 0.1177m) - 1;
                var expectedNetQuarterToDate = (1-0.4162m)* (1 - 0.4526m) - 1;
                var expectedNetFirstFullMonth= (1 - 0.4162m) * (1 - 0.4526m) * (1 + 0.955m)
                                          * (1 - 0.1157m) - 1;

                netRecordModel.OneMonth.Should().BeApproximately(-0.4162m, 0.00000001m);
                netRecordModel.ThreeMonths.Should().BeApproximately(-0.375236505m, 0.00000001m);
                netRecordModel.SixMonths.Should().BeApproximately(expectedNetSixMonth, 0.00000001m);
                netRecordModel.TwelveMonths.Should().NotHaveValue();
                netRecordModel.StandardDeviation.Should().NotHaveValue();
                netRecordModel.YearToDate.Should().BeApproximately(0.0010907851939m, 0.00000001m);
                netRecordModel.QuarterToDate.Should().BeApproximately(expectedNetQuarterToDate, 0.00000001m);
                netRecordModel.FirstFullMonth.Should().BeApproximately(expectedNetFirstFullMonth, 0.00000001m);


                var grossRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[1];

                grossRecordModel.EntityNumber.Should().Be(portfolioNumber);
                grossRecordModel.Name.Should().Be(portfolioName);
                grossRecordModel.Type.Should().Be("Portfolio");
                grossRecordModel.FeeType.Should().Be("Gross");

                var expectedGrossSixMonth = (1 + 0.5307m) * (1 - 0.9776m) * (1 - 0.5501m)
                                            * (1 - 0.109m) * (1 + 0.6042m) * (1 - 0.3867m) - 1;
                var expectedGrossQuarterToDate = (1+0.5307m) * (1 - 0.9776m) -1;
                var expectedGrossFirstFullMonth = (1 + 0.5307m) * (1 - 0.9776m) * (1 - 0.5501m)
                                            * (1 - 0.109m) - 1;

                grossRecordModel.OneMonth.Should().BeApproximately(0.5307m, 0.00000001m);
                grossRecordModel.ThreeMonths.Should().BeApproximately(-0.9845739727m, 0.00000001m);
                grossRecordModel.SixMonths.Should().BeApproximately(expectedGrossSixMonth, 0.00000001m);
                grossRecordModel.TwelveMonths.Should().NotHaveValue();
                grossRecordModel.StandardDeviation.Should().NotHaveValue();
                grossRecordModel.YearToDate.Should().BeApproximately(-0.977950928298953m, 0.00000001m);
                grossRecordModel.QuarterToDate.Should().BeApproximately(expectedGrossQuarterToDate, 0.00000001m);
                grossRecordModel.FirstFullMonth.Should().BeApproximately(expectedGrossFirstFullMonth, 0.00000001m);

                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 5/2016");
            });
        }


        [Fact]
        public void ShouldPrintNetAndGrossRecordsForPortrfolioForAYear()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var presenter = testHelper.CreatePresenter();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2010, 1, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                var returnSeriesIdGross = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-11),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                Debug.WriteLine("Net Returns:");
                foreach (var monthlyReturn in netMonthlyReturnDtos)
                {
                    Debug.Write(monthlyReturn.ReturnValue + "m, ");
                }

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdGross,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.GrossSeriesTypeCode
                });

                var grossMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdGross,
                    monthYearRange,
                    seed: 2);

                Debug.WriteLine("Gross Returns:");
                foreach (var monthlyReturn in grossMonthlyReturnDtos)
                {

                    Debug.Write(monthlyReturn.ReturnValue + "m, ");
                }


                testHelper.InsertMonthlyReturnDtos(grossMonthlyReturnDtos);

                presenter.CreateReport(
                    monthYear,
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();

                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(2);

                var netRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[0];

                netRecordModel.EntityNumber.Should().Be(portfolioNumber);
                netRecordModel.Name.Should().Be(portfolioName);
                netRecordModel.Type.Should().Be("Portfolio");
                netRecordModel.FeeType.Should().Be("Net");

                

                var netValues = new decimal[]
                {
                    0.4524m, 0.6346m, 0.536m, 0.1163m, -0.588m, 0.1177m,
                    0.812m, -0.1157m, 0.955m, -0.4526m, -0.4162m, -0.0654m
                };

                var netMean = netValues.Sum() / netValues.Length;

                for (int i = 0; i < netValues.Length; i++)
                {
                    netValues[i] = (netMean - netValues[i]) * (netMean - netValues[i]);
                }

                var expectedNetStandardDeviation = (Decimal)Math.Sqrt((Double)netValues.Sum() / netValues.Length);


                netRecordModel.StandardDeviation.Should().BeApproximately(
                    expectedNetStandardDeviation, 0.00000001m);


                var grossRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[1];

                grossRecordModel.EntityNumber.Should().Be(portfolioNumber);
                grossRecordModel.Name.Should().Be(portfolioName);
                grossRecordModel.Type.Should().Be("Portfolio");
                grossRecordModel.FeeType.Should().Be("Gross");

                var grossValues = new decimal[]
                {0.5421m, -0.1917m, -0.6681m, 0.97m, -0.782m,
                    -0.3867m, 0.6042m, -0.109m, -0.5501m, -0.9776m,
                    0.5307m, -0.9426m

				};

                var grossMean = grossValues.Sum() / grossValues.Length;

                for (int i = 0; i < grossValues.Length; i++)
                {
                    grossValues[i] = (grossMean - grossValues[i]) * (grossMean - grossValues[i]);
                }
                decimal expectedGrossStandardDeviation= (Decimal)Math.Sqrt((Double)grossValues.Sum() / grossValues.Length); ;


                grossRecordModel.StandardDeviation.Should().BeApproximately(expectedGrossStandardDeviation, 0.00000001m);

                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 5/2016");
            });
        }

        [Fact]
        public void ShouldOnlyRecordStandardDeviationOfMonthlyReturnsAfterInception()
        {
            var testHelper = new TestHelper();

            testHelper.DatabaseDataDeleter(() =>
            {
                var presenter = testHelper.CreatePresenter();

                var portfolioNumber = 100;
                var portfolioName = "Portfolio 100";

                var monthYear = new MonthYear(2016, 5);

                testHelper.InsertPortfolioDto(new PortfolioDto()
                {
                    Number = portfolioNumber,
                    Name = portfolioName,
                    InceptionDate = new DateTime(2015, 6, 1)
                });

                var returnSeriesIdNet = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                var returnSeriesIdGross = testHelper.InsertReturnSeriesDto(new ReturnSeriesDto()
                {
                    Name = "Net Return Series for Portfolio 100"
                });

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdNet,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.NetSeriesTypeCode
                });

                var monthYearRange = new MonthYearRange(
                    monthYear.AddMonths(-56),
                    monthYear);

                var netMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdNet,
                    monthYearRange);

                Debug.WriteLine("Net Returns:");
                foreach (var monthlyReturn in netMonthlyReturnDtos)
                {
                    Debug.Write(monthlyReturn.ReturnValue + "m, ");
                }

                testHelper.InsertMonthlyReturnDtos(netMonthlyReturnDtos);

                testHelper.InsertPortfolioToReturnSeriesDto(new PortfolioToReturnSeriesDto()
                {
                    PortfolioNumber = portfolioNumber,
                    ReturnSeriesId = returnSeriesIdGross,
                    SeriesTypeCode = PortfolioToReturnSeriesDto.GrossSeriesTypeCode
                });

                var grossMonthlyReturnDtos = MonthlyReturnDtoDataBuilder.CreateMonthlyReturns(
                    returnSeriesIdGross,
                    monthYearRange,
                    seed: 2);

                Debug.WriteLine("Gross Returns:");
                foreach (var monthlyReturn in grossMonthlyReturnDtos)
                {

                    Debug.Write(monthlyReturn.ReturnValue + "m, ");
                }
                foreach (var portfolioMonthlyReturnDto in grossMonthlyReturnDtos)
                {
                    Debug.WriteLine(portfolioMonthlyReturnDto.ReturnValue + " and Montheyar " + portfolioMonthlyReturnDto.Month + "/" + portfolioMonthlyReturnDto.Year);
                }


                testHelper.InsertMonthlyReturnDtos(grossMonthlyReturnDtos);

                presenter.CreateReport(
                    monthYear,
                    "c:\\temp\\excel.xlsx");

                var viewSpy = testHelper.PerformanceReportExcelReportViewSpy;

                viewSpy.RenderReportWasCalled.Should().BeTrue();

                viewSpy.PerformanceReportExcelReportModel.Records.Length.Should().Be(2);

                var netRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[0];

                netRecordModel.EntityNumber.Should().Be(portfolioNumber);
                netRecordModel.Name.Should().Be(portfolioName);
                netRecordModel.Type.Should().Be("Portfolio");
                netRecordModel.FeeType.Should().Be("Net");



//                var netValues = new decimal[]
//                {
//                    0.4524m, 0.6346m, 0.536m, 0.1163m, -0.588m, 0.1177m,
//                    0.812m, -0.1157m, 0.955m, -0.4526m, -0.4162m, -0.0654m
//                };


                var netValues = new decimal[]
                {
                    0.58950000m, -0.32570000m, -0.08560000m, -0.70640000m, -0.55740000m, -0.17990000m,
                    0.43740000m, 0.23960000m, -0.02410000m, -0.61020000m, 0.75630000m, 0.65080000m
                };
                var netMean = netValues.Sum() / netValues.Length;

                for (int i = 0; i < netValues.Length; i++)
                {
                    netValues[i] = (netMean - netValues[i]) * (netMean - netValues[i]);
                }

                var expectedNetStandardDeviation = (Decimal)Math.Sqrt((Double)netValues.Sum() / netValues.Length);


                netRecordModel.StandardDeviation.Should().BeApproximately(
                    expectedNetStandardDeviation, 0.00000001m);


                var grossRecordModel = viewSpy.PerformanceReportExcelReportModel.Records[1];

                grossRecordModel.EntityNumber.Should().Be(portfolioNumber);
                grossRecordModel.Name.Should().Be(portfolioName);
                grossRecordModel.Type.Should().Be("Portfolio");
                grossRecordModel.FeeType.Should().Be("Gross");

//                var grossValues = new decimal[]
//                {0.5421m, -0.1917m, -0.6681m, 0.97m, -0.782m,
//                    -0.3867m, 0.6042m, -0.109m, -0.5501m, -0.9776m,
//                    0.5307m, -0.9426m
//                };

                var grossValues = new decimal[]
                {
                    0.11200000m, -0.96480000m, -0.02300000m, -0.31560000m, -0.02870000m, 0.28480000m,
                    0.24020000m, -0.85340000m, 0.79080000m, 0.19070000m, -0.13530000m, -0.85150000m
                };

                var grossMean = grossValues.Sum() / grossValues.Length;

                for (int i = 0; i < grossValues.Length; i++)
                {
                    grossValues[i] = (grossMean - grossValues[i]) * (grossMean - grossValues[i]);
                }
                decimal expectedGrossStandardDeviation = (Decimal)Math.Sqrt((Double)grossValues.Sum() / grossValues.Length); ;


                grossRecordModel.StandardDeviation.Should().BeApproximately(expectedGrossStandardDeviation, 0.00000001m);

                viewSpy.PerformanceReportExcelReportModel.MonthText.Should().Be("Month: 5/2016");
            });
        }

        public class PerformanceReportExcelReportViewSpy : IPerformanceReportExcelReportView
        {
            public bool RenderReportWasCalled { get; private set; }
            public PerformanceReportExcelReportModel PerformanceReportExcelReportModel { get; private set; }
            public string FullFilePath { get; private set; }

            public void RenderReport(
                PerformanceReportExcelReportModel model,
                string fullFilePath)
            {
                FullFilePath = fullFilePath;
                PerformanceReportExcelReportModel = model;
                RenderReportWasCalled = true;
            }
        }
    }
}