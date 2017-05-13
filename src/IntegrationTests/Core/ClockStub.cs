using System;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.IntegrationTests.Core
{
    public class ClockStub : IClock
    {
        private readonly DateTime _currentDate;

        public ClockStub(
            DateTime currentDate)
        {
            _currentDate = currentDate;
        }

        public DateTime GetCurrentDate()
        {
            return _currentDate;
        }
    }
}