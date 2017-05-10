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
}