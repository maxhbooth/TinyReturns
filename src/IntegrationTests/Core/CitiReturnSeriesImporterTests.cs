﻿using System.IO;
using System.Linq;
using Dapper;
using Dimensional.TinyReturns.Core;
using Xunit;

namespace Dimensional.TinyReturns.IntegrationTests.Core
{
    public class CitiReturnSeriesImporterTests : DatabaseTestBase
    {
        private readonly IReturnsSeriesRepository _returnsSeriesRepository;

        public CitiReturnSeriesImporterTests()
        {
            _returnsSeriesRepository = MasterFactory.GetReturnsSeriesRepository();
        }

        [Fact]
        public void ImportPortfolioNetReturnSeriesFilesShouldInsertCorrectNumberOfSeriesWhenGivenValidFile()
        {
            DeleteTestData();
            
            ImportTestFile();

            var series = _returnsSeriesRepository.GetReturnSeries(
                new[] {100, 101, 102});

            Assert.Equal(series.Length, 3);

            DeleteTestData();
        }

        [Fact]
        public void ImportPortfolioNetReturnSeriesFilesShouldInsertCorrectSeriesWhenGivenValidFile()
        {
            DeleteTestData();

            ImportTestFile();

            var series = _returnsSeriesRepository.GetReturnSeries(
                new[] { 100 });

            Assert.Equal(series[0].EntityNumber, 100);
            Assert.Equal(series[0].FeeTypeCode, 'N');

            DeleteTestData();
        }

        [Fact]
        public void ImportPortfolioNetReturnSeriesFilesShouldInsertCorrectNumberOfMonthlyReturns()
        {
            DeleteTestData();

            ImportTestFile();

            var monthlyReturns = GetMonthlyReturnSeries(100);

            Assert.Equal(monthlyReturns.Length, 9);

            DeleteTestData();
        }

        [Fact]
        public void ImportPortfolioNetReturnSeriesFilesShouldCorrectlyMapMonthlyReturns()
        {
            DeleteTestData();

            ImportTestFile();

            var monthlyReturns = GetMonthlyReturnSeries(100);

            var target = monthlyReturns.FirstOrDefault(r => r.Month == 10 && r.Year == 2013);

            Assert.NotNull(target);
            Assert.Equal(target.ReturnValue, 4.40055m);

            DeleteTestData();
        }

        private MonthlyReturn[] GetMonthlyReturnSeries(int entityNumber)
        {
            var returnSeries = _returnsSeriesRepository.GetReturnSeries(new[] {entityNumber}).First();
            var monthlyReturns = _returnsSeriesRepository.GetMonthlyReturns(returnSeries.ReturnSeriesId);
            return monthlyReturns;
        }

        private void ImportTestFile()
        {
            var importer = MasterFactory.GetCitiReturnSeriesImporter();

            var filePath = GetNetReturnsTestFilePath();

            importer.ImportPortfolioNetReturnSeriesFiles(filePath);
        }

        private void DeleteTestData()
        {
            const string deleteMonthlyReturnsSql = "DELETE FROM [MonthlyReturn]";

            ConnectionExecuteWithLog(
                MasterFactory.GetTinyReturnsDatabaseSettings().ReturnsDatabaseConnectionString,
                connection =>
                {
                    connection.Execute(deleteMonthlyReturnsSql);
                },
                deleteMonthlyReturnsSql);

            const string deleteReturnSeriesSql = "DELETE FROM [ReturnSeries]";
            
            ConnectionExecuteWithLog(
                MasterFactory.GetTinyReturnsDatabaseSettings().ReturnsDatabaseConnectionString,
                connection =>
                {
                    connection.Execute(deleteReturnSeriesSql);
                },
                deleteReturnSeriesSql);
        }

        private string GetNetReturnsTestFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var targetFile = currentDirectory
                 + @"\Core\TestNetReturnsForEntity100_101_102.csv";

            return targetFile;
        }

    }
}