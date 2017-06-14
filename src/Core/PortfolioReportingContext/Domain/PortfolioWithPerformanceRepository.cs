﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Performance;
using Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio;

namespace Dimensional.TinyReturns.Core.PortfolioReportingContext.Domain
{
    public class PortfolioWithPerformanceRepository
    {
        private readonly IPortfolioDataTableGateway _portfolioDataTableGateway;
        private readonly IPortfolioToReturnSeriesDataTableGateway _portfolioToReturnSeriesDataTableGateway;
        private readonly IPortfolioToBenchmarkDataTableGateway _portfolioToBenchmarkDataTableGateway;
        private readonly ICountriesDataTableGateway _countriesDataTableGateway;

        private readonly ReturnSeriesRepository _returnSeriesRepository;
        private readonly BenchmarkWithPerformanceRepository _benchmarkWithPerformanceRepository;

        public PortfolioWithPerformanceRepository(
            IPortfolioDataTableGateway portfolioDataTableGateway,
            IPortfolioToReturnSeriesDataTableGateway portfolioToReturnSeriesDataTableGateway,
            IPortfolioToBenchmarkDataTableGateway portfolioToBenchmarkDataTableGateway,
            ICountriesDataTableGateway countriesDataTableGateway,
            ReturnSeriesRepository returnSeriesRepository,
            BenchmarkWithPerformanceRepository benchmarkWithPerformanceRepository)
        {
            _countriesDataTableGateway = countriesDataTableGateway;
            _portfolioToBenchmarkDataTableGateway = portfolioToBenchmarkDataTableGateway;
            _benchmarkWithPerformanceRepository = benchmarkWithPerformanceRepository;
            _returnSeriesRepository = returnSeriesRepository;
            _portfolioToReturnSeriesDataTableGateway = portfolioToReturnSeriesDataTableGateway;
            _portfolioDataTableGateway = portfolioDataTableGateway;
        }

        public PortfolioWithPerformance[] GetAll()
        {
            var portfolioDtos = _portfolioDataTableGateway.GetAll();

            var portfolioModels = new List<PortfolioWithPerformance>();

            var portfolioNumbers = portfolioDtos.Select(p => p.Number).ToArray();

            var portfolioToReturnSeriesDtos = _portfolioToReturnSeriesDataTableGateway.Get(portfolioNumbers);

            var targetReturnSeriesIds = portfolioToReturnSeriesDtos
                .Select(d => d.ReturnSeriesId)
                .ToArray();

            var returnSeries = _returnSeriesRepository.GetReturnSeries(targetReturnSeriesIds);

            var benchmarkWithPerformances = _benchmarkWithPerformanceRepository.GetAll();
            var portfolioToBenchmarkDtos = _portfolioToBenchmarkDataTableGateway.GetAll();
            var countryDtos = _countriesDataTableGateway.GetAll();

            foreach (var portfolioDto in portfolioDtos)
            {
                var netDto = portfolioToReturnSeriesDtos.FindNet(
                    portfolioDto.Number);

                var grossDto = portfolioToReturnSeriesDtos.FindGross(
                    portfolioDto.Number);

                var inceptionDate = portfolioDto.InceptionDate;

                var closeDate = portfolioDto.CloseDate;

                ReturnSeries netReturnSeries = null;
                ReturnSeries grossReturnSeries = null;

                if (netDto != null)
                    netReturnSeries = returnSeries.FirstOrDefault(r => r.Id == netDto.ReturnSeriesId);

                if (grossDto != null)
                    grossReturnSeries = returnSeries.FirstOrDefault(r => r.Id == grossDto.ReturnSeriesId);

                var benchmarkNumbers = portfolioToBenchmarkDtos
                    .Where(d => d.PortfolioNumber == portfolioDto.Number)
                    .Select(b => b.BenchmarkNumber)
                    .ToArray();

                String countryName="";

                var country = countryDtos.FirstOrDefault(d => d.CountryId == portfolioDto.CountryId);

                if (country != null)
                {
                    countryName = country.CountryName;
                }
                
                var withPerformances = benchmarkWithPerformances.Where(b => benchmarkNumbers.Any(n => n == b.Number)).ToArray();

                portfolioModels.Add(new PortfolioWithPerformance(portfolioDto.Number, portfolioDto.Name, netReturnSeries, grossReturnSeries, withPerformances, inceptionDate, countryName, closeDate));
            }

            return portfolioModels.ToArray();
        }

    }
}