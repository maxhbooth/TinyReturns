using System;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class SystemLogForIntegrationTests : ISystemLog
    {
        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Error : " + message);
        }

        public void Error(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Error : " + message + " Exception :" + ex.StackTrace);
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Warn : " + message);
        }

        public void Warn(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Warn : " + message + " Exception :" + ex.StackTrace);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Info : " + message);
        }

        public void Info(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Info : " + message + " Exception :" + ex.StackTrace);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("** " + DateTime.Now.ToLongTimeString() + " Debug : " + message);
        }
    }
}