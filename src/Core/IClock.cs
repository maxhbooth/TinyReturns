using System;

namespace Dimensional.TinyReturns.Core
{
    public interface IClock
    {
        DateTime GetCurrentDate();
    }

    public class Clock : IClock
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
    }

    public class ClockForTesting : IClock
    {
        private readonly DateTime _currentDateTime;

        public ClockForTesting(
            DateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public DateTime GetCurrentDate()
        {
            return _currentDateTime;
        }

        public DateTime GetFirstDayOfMonth(IClock clock)
        {
            var currentDay = clock.GetCurrentDate();

            return new DateTime(currentDay.Year, currentDay.Month, 1);
        }
    }


}