using System;

namespace Dimensional.TinyReturns.Core.SharedContext.Services
{
    public interface ISystemLog
    {
        void Error(string message);
        void Error(string message, Exception ex);

        void Warn(string message);
        void Warn(string message, Exception ex);

        void Info(string message);
        void Info(string message, Exception ex);

        void Debug(string message);
    }
}