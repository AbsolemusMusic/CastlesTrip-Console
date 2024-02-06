using System.Collections.Generic;
using UnityEngine;

namespace CT.Console
{
    /// <summary>
    /// Console Renderer. Show all debug logs
    /// </summary>
    public class ConsoleRenderer : MonoBehaviour
    {
        private Vector2 scrollPosition;
        public bool isShow;
        private bool isCollapse;
        private IConsoleHandler console;

        private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        private const int MARGIN = 20;

        private Rect windowRect = new Rect(MARGIN, MARGIN, Screen.width - (MARGIN * 2), Screen.height - (MARGIN * 2));
        private Rect titleBarRect = new Rect(0, 0, 10000, 20);

        private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        private GUIContent closeLabel = new GUIContent("Close", "Close console.");
        private GUIContent copyLabel = new GUIContent("Copy", "Copy to clipboard.");
        private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        private const int WINDOW_ID = 123456;

        private void OnEnable()
        {
            console = ConsoleHandler.Instance;
        }

        private void OnDisable()
        {

        }

        internal static void Write(string v, object userState)
        {
            throw new System.NotImplementedException();
        }

        private void SetShowState(bool state)
        {
            isShow = state;
        }

        private void LateUpdate()
        {
            if (!IsSwitch())
                return;

            isShow = !isShow;
        }

        private void OnGUI()
        {
            if (!isShow)
                return;

            windowRect = GUILayout.Window(WINDOW_ID, windowRect, ConsoleWindow, "Console");
        }

        /// <summary>
        /// A window that displayss the recorded logs.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        private void ConsoleWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate through the recorded logs.
            for (int i = 0; i < console.LogCount; i++)
            {
                var log = console.GetLog(i);

                // Combine identical messages if collapse option is chosen.
                if (isCollapse)
                {
                    if (i <= 0)
                        continue;

                    var prevLog = console.GetLog(i - 1);
                    var messageSameAsPrevious = log.Message.Equals(prevLog.Message);

                    if (messageSameAsPrevious)
                        continue;
                }

                GUI.contentColor = logTypeColors[log.LogType];
                GUILayout.Label(log.Message);
            }

            GUILayout.EndScrollView();

            GUI.contentColor = Color.white;


            GUILayout.BeginHorizontal();

            if (GUILayout.Button(copyLabel))
                GUIUtility.systemCopyBuffer = GetConsoleText();

            if (GUILayout.Button(clearLabel))
                ConsoleHandler.Instance.Clear();

            isCollapse = GUILayout.Toggle(isCollapse, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();


            // New Line
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(closeLabel))
                SetShowState(false);

            GUILayout.EndHorizontal();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        private string GetConsoleText()
        {
            string full = string.Empty;
            for (int i = 0; i < console.LogCount; i++)
            {
                var log = console.GetLog(i);
                string message = log.Message;
                full += message;
                full += "\n";
            }
            return full;
        }

        private bool IsSwitch()
        {
#if UNITY_IPHONE
            // TODO: In Progress
            return false;
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

            bool[] isKeys =
            {
                Input.anyKey,
                Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftApple),
                Input.GetKeyDown(KeyCode.C)
            };

            foreach(bool isKey in isKeys)
            {
                if (!isKey)
                    return false;
            }

            return true;
#endif
            return false;
        }
    }
}