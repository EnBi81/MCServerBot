using System;
using System.Collections.Generic;

namespace MCWebServer.Log
{
    public class LogService
    {
        private readonly Dictionary<Type, Logger> _loggers = new();
        private static LogService _instance = new LogService();

        public LogService()
        {

        }


        public LogService SetupLogger<T>() where T : Logger
        {
            Type loggerType = typeof(T);

            if (_loggers.ContainsKey(loggerType))
            {
                throw new Exception(loggerType.FullName + " is already present in the loggers");
            }

            T logger = (T)Activator.CreateInstance(loggerType);
            _loggers.Add(loggerType, logger);

            return this;
        }

        

        public static void RegisterLogService(LogService service)
        {
            _instance = service;
        }


        public static T GetService<T>() where T : Logger
        {
            return (T)_instance._loggers[typeof(T)];
        }


        

        // type: 
        //   0: debug
        //   1: info
        //   2: error
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void Log<T>(string message, int type) where T : Logger
        //{
        //    var loggerType = typeof(T);
        //    if (_loggers.TryGetValue(loggerType, out Logger logger))
        //        logger.Log(message, type);
        //}

        //public static void LogDebug<T>(string message) where T : Logger
        //{
        //    _instance.Log<T>(message, 0);
        //}

        //public static void LogInfo<T>(string message) where T : Logger
        //{
        //    _instance.Log<T>(message, 1);
        //}

        //public static void LogError<T>(string message) where T : Logger
        //{
        //    _instance.Log<T>(message, 2);
        //}

        //public static void LogError<T>(Exception e) where T : Logger
        //{
        //    _instance.Log<T>(e.Message, 2);
        //}
    }
}
