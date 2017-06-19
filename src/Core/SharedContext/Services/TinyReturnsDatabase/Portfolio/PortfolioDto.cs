using System;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio
{
    public class PortfolioDto
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public DateTime InceptionDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}