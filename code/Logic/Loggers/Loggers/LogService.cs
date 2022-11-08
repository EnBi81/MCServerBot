using System;
using System.Collections.Generic;

namespace Loggers
{
    public class LogService
    {
        private readonly Dictionary<Type, Logger> _loggers = new();
        private static LogService _instance = new ();

        private LogService()
        {

        }


        /// <summary>
        /// Adds a logger to the logservice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public LogService AddLogger<T>() where T : Logger
        {
            Type loggerType = typeof(T);

            if (_loggers.ContainsKey(loggerType))
            {
                throw new Exception(loggerType.FullName + " is already present in the loggers");
            }

            T logger = (T)Activator.CreateInstance(loggerType)!;
            _loggers.Add(loggerType, logger);

            return this;
        }



        /// <summary>
        /// Creates a log service and assigns it as a global log service
        /// </summary>
        /// <returns></returns>
        public static LogService CreateLogService() =>
            _instance = new LogService();


        /// <summary>
        /// Gets the log service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : Logger =>
            (T)_instance._loggers[typeof(T)];
    }
}
