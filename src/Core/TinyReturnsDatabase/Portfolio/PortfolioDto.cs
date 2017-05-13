using System;

namespace Dimensional.TinyReturns.Core.TinyReturnsDatabase.Portfolio
{
    public class PortfolioDto
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime InceptionDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}