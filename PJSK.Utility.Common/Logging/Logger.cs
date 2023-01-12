using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PJSK.Utility.Common.Enums;

namespace PJSK.Utility.Common.Logging
{
    public class Logger
    {
        public void WriteLog(string message, LogLevelTypes logLevel)
        {
            //message = _defaultLogInfo + "|" + message;
            //NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            
            //NLog.LogEventInfo logEvent = new NLog.LogEventInfo(ConvertToLogLevel(logLevel), message);
            //AddLogProperties(ref logEvent);

            //logger.Log(typeof(Log), logEvent);
        }

        private NLog.LogLevel ConvertToLogLevel(LogLevelTypes logLevel)
        {
            NLog.LogLevel nlogLogLevel;
            switch (logLevel)
            {
                case LogLevelTypes.Trace:
                    nlogLogLevel = NLog.LogLevel.Trace;
                    break;
                case LogLevelTypes.Debug:
                default:
                    nlogLogLevel = NLog.LogLevel.Debug;
                    break;
                case LogLevelTypes.Info:
                    nlogLogLevel = NLog.LogLevel.Info;
                    break;
                case LogLevelTypes.Error:
                    nlogLogLevel = NLog.LogLevel.Error;
                    break;
                case LogLevelTypes.Fatal:
                    nlogLogLevel = NLog.LogLevel.Fatal;
                    break;
            }
            return nlogLogLevel;
        }
    }
}
