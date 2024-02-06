using System.Collections.Generic;
using UnityEngine;

namespace CT.Console
{
    public class ConsoleHandler : IConsoleHandler
    {
        private static ConsoleHandler instance;
        public static ConsoleHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsoleHandler();
                return instance;
            }
        }

        private List<ConsoleLog> logs = new List<ConsoleLog>();

        private const string CONSOLE_SUBSCRIBE_KEY = "consoleSubscribeKey";

        private static bool isSubscribe
        {
            get => GetSubscribeState();
            set => SetSubscribeState(value);
        }

        public int LogCount => logs.Count;

        private ConsoleHandler()
        {
            logs = new List<ConsoleLog>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashScreen()
        {
            if (!isSubscribe)
                return;

            Instance.Subscribe();
        }

        private static bool GetSubscribeState()
        {
            return PlayerPrefs.GetInt(CONSOLE_SUBSCRIBE_KEY, 1) == 1;
        }

        private static void SetSubscribeState(bool value)
        {
            PlayerPrefs.SetInt(CONSOLE_SUBSCRIBE_KEY, value ? 1 : 0);
        }


        public void Subscribe()
        {
            Unsubscribe();
            Application.logMessageReceived += HandleLog;
        }

        public void Unsubscribe()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public void Clear()
        {
            logs.Clear();
        }

        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="logType">Type of message (error, exception, warning, assert).</param>
        private void HandleLog(string message, string stackTrace, LogType logType)
        {
            ConsoleLog log = new ConsoleLog(message, stackTrace, logType);
            logs.Add(log);
        }

        public ConsoleLog GetLog(int logID)
        {
            if (logID < 0 && logID >= LogCount)
                return default;
            return logs[logID];
        }
    }
}