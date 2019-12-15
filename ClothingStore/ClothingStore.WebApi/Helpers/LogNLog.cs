using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Helpers
{
    public interface ILogger
    {
        void Debug(object value);
        void Info(string message, params object[] args);
        void Error(string message, params object[] args);
    }

    public class LogNLog : ILogger
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Debug(object value)
        {
            logger.Debug(value);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }
    }

}
