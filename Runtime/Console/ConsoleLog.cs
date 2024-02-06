using UnityEngine;

namespace CT.Console
{
    public struct ConsoleLog
    {
        public string Message;
        public string StackTrace;
        public LogType LogType;

        public ConsoleLog(string message, string stackTrace, LogType logType)
        {
            Message = message;
            StackTrace = stackTrace;
            LogType = logType;
        }
    }
}