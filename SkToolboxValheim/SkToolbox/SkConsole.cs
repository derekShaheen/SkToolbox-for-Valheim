using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// Credit: github. com/ mminer/ consolation Thank you for this!!
/// </summary>
namespace SkToolbox
{
    internal class SkConsole : MonoBehaviour
    {
        public static Version SkConsoleVersion = new Version(1, 1, 1);

        #region Inspector Settings

        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.F4;

        /// <summary>
        /// Whether to open as soon as the game starts.
        /// </summary>
        public bool openOnStart = false;

        /// <summary>
        /// The number of seconds that have to pass between visibility toggles.
        /// This threshold prevents closing again while shaking to open.
        /// </summary>
        public float toggleThresholdSeconds = .5f;
        //float lastToggleTime;

        /// <summary>
        /// Whether to only keep a certain number of logs, useful if memory usage is a concern.
        /// </summary>
        public bool restrictLogCount = true;

        /// <summary>
        /// Number of logs to keep before removing old ones.
        /// </summary>
        public int maxLogCount = 3000;

        /// <summary>
        /// Font size to display log entries with.
        /// </summary>
        public int logFontSize = 18;
        private int logLineNumber = 0;
        /// <summary>
        /// Amount to scale UI by.
        /// </summary>
        public float scaleFactor = 1f;

        public static bool writeToFile = false;
        private string logSavePath;
        public static bool cursorUnlock = true;
        #endregion


        static string outputString = string.Empty;
        static readonly GUIContent submitLabel = new GUIContent("Submit", "Submit the input.");
        static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        const int margin = 350;
        const string windowTitle = "Console [SkToolbox]";

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow }
        };

        bool isCollapsed;
        bool isVisible;
        readonly List<Log> logs = new List<Log>();
        readonly ConcurrentQueue<Log> queuedLogs = new ConcurrentQueue<Log>();

        Vector2 scrollPosition;
        readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        float windowX = margin;
        float windowY = margin;

        readonly Dictionary<LogType, bool> logTypeFilters = new Dictionary<LogType, bool>
        {
            { LogType.Assert, false },
            { LogType.Error, false },
            { LogType.Exception, false },
            { LogType.Log, true },
            { LogType.Warning, false },
        };

        #region MonoBehaviour Messages

        void OnDisable()
        {
            //Application.logMessageReceivedThreaded -= HandleLogThreaded;
        }

        void OnEnable()
        {
            //Application.logMessageReceivedThreaded += HandleLogThreaded;
        }

        void OnGUI()
        {
            if (!isVisible)
            {
                return;
            }

            GUI.matrix = Matrix4x4.Scale(Vector3.one * scaleFactor);

            float width = (Screen.width / (scaleFactor * 2)) - (margin * 2);
            float height = (Screen.height / scaleFactor) - (margin * 2);
            Rect windowRect = new Rect(windowX, windowY, width, height);

            Rect newWindowRect = GUILayout.Window(8675309, windowRect, DrawWindow, windowTitle);
            windowX = newWindowRect.x;
            windowY = newWindowRect.y;

            if (cursorUnlock)
            {
                unlockCursor();
            }
        }

        void Start()
        {
            logSavePath = Application.persistentDataPath + "/!SkToolbox Console Log.txt";
            if (openOnStart)
            {
                isVisible = true;
            }
        }

        void Update()
        {
            UpdateQueuedLogs();

            float curTime = Time.realtimeSinceStartup;

            if (Input.GetKeyDown(toggleKey))
            {
                isVisible = !isVisible;
            }
        }

        #endregion

        void DrawLog(Log log, GUIStyle logStyle, GUIStyle badgeStyle)
        {
            GUI.contentColor = logTypeColors[log.type];

            if (isCollapsed)
            {
                // Draw collapsed log with badge indicating count.
                GUILayout.BeginHorizontal();
                GUILayout.Label(log.GetTruncatedMessage(), logStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(log.count.ToString(), GUI.skin.box);
                GUILayout.EndHorizontal();
            }
            else
            {
                // Draw expanded log.
                for (var i = 0; i < log.count; i += 1)
                {
                    GUILayout.Label(log.GetTruncatedMessage(), logStyle);
                }
            }

            GUI.contentColor = Color.white;
        }

        void DrawLogList()
        {
            GUIStyle badgeStyle = GUI.skin.box;
            badgeStyle.fontSize = logFontSize;
            try
            {
                badgeStyle.font = Font.CreateDynamicFontFromOSFont("Consolas.ttf", 12);
            }
            catch (Exception)
            {

            }

            GUIStyle logStyle = GUI.skin.label;
            logStyle.fontSize = logFontSize;
            try
            {
                logStyle.font = Font.CreateDynamicFontFromOSFont("Consolas.ttf", 12);
            } catch(Exception)
            {

            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Used to determine height of accumulated log labels.
            GUILayout.BeginVertical();

            var visibleLogs = logs.Where(IsLogVisible);

            foreach (Log log in visibleLogs)
            {
                DrawLog(log, logStyle, badgeStyle);
            }

            GUILayout.EndVertical();
            var innerScrollRect = GUILayoutUtility.GetLastRect();
            GUILayout.EndScrollView();
            var outerScrollRect = GUILayoutUtility.GetLastRect();

            // If we're scrolled to bottom now, guarantee that it continues to be in next cycle.
            if (Event.current.type == EventType.Repaint && IsScrolledToBottom(innerScrollRect, outerScrollRect))
            {
                ScrollToBottom();
            }
        }

        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            foreach (LogType logType in Enum.GetValues(typeof(LogType)))
            {
                var currentState = logTypeFilters[logType];
                var label = logType.ToString();
                logTypeFilters[logType] = GUILayout.Toggle(currentState, label, GUILayout.ExpandWidth(false));
                GUILayout.Space(20);
            }

            isCollapsed = GUILayout.Toggle(isCollapsed, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }

        void DrawInput()
        {
            GUILayout.BeginHorizontal();

            outputString = GUILayout.TextField(outputString, 2000);

            if (GUILayout.Button(submitLabel, GUILayout.ExpandWidth(false)))
            {
                SkCommandProcessor.PassBack(outputString);
            }

            GUILayout.EndHorizontal();
        }

        void DrawWindow(int windowID)
        {
            DrawLogList();
            DrawInput();
            DrawToolbar();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        Log? GetLastLog()
        {
            if (logs.Count == 0)
            {
                return null;
            }

            return logs.Last();
        }

        void UpdateQueuedLogs()
        {
            Log log;
            while (queuedLogs.TryDequeue(out log))
            {
                ProcessLogItem(log);
            }
        }

        public void AddLogEntry(string message)
        {
            var log = new Log
            {
                count = 1,
                message = message,
                stackTrace = "",
                type = LogType.Log,
            };
            queuedLogs.Enqueue(log);
        }

        void HandleLogThreaded(string message, string stackTrace, LogType type)
        {
            var log = new Log
            {
                count = 1,
                message = message,
                stackTrace = stackTrace,
                type = type,
            };
            if (writeToFile)
            {
                logLineNumber++;
                using (StreamWriter writer = new StreamWriter(logSavePath, true))
                {
                    //writer.WriteLine(message + "\n\t\t" + type + ": " + stackTrace.Replace("\n", "\n\t\t"));
                    writer.WriteLine(logLineNumber + "\t:\t" + message);
                }
            }

            // Queue the log into a ConcurrentQueue to be processed later in the Unity main thread,
            // so that we don't get GUI-related errors for logs coming from other threads
            queuedLogs.Enqueue(log);
        }

        void ProcessLogItem(Log log)
        {
            var lastLog = GetLastLog();
            var isDuplicateOfLastLog = lastLog.HasValue && log.Equals(lastLog.Value);

            if (isDuplicateOfLastLog)
            {
                // Replace previous log with incremented count instead of adding a new one.
                log.count = lastLog.Value.count + 1;
                logs[logs.Count - 1] = log;
            }
            else
            {
                logs.Add(log);
                TrimExcessLogs();
            }
        }

        bool IsLogVisible(Log log)
        {
            return logTypeFilters[log.type];
        }

        bool IsScrolledToBottom(Rect innerScrollRect, Rect outerScrollRect)
        {
            var innerScrollHeight = innerScrollRect.height;

            // Take into account extra padding added to the scroll container.
            var outerScrollHeight = outerScrollRect.height - GUI.skin.box.padding.vertical;

            // If contents of scroll view haven't exceeded outer container, treat it as scrolled to bottom.
            if (outerScrollHeight > innerScrollHeight)
            {
                return true;
            }

            // Scrolled to bottom (with error margin for float math)
            return Mathf.Approximately(innerScrollHeight, scrollPosition.y + outerScrollHeight);
        }

        void ScrollToBottom()
        {
            scrollPosition = new Vector2(0, Int32.MaxValue);
        }

        void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = logs.Count - maxLogCount;

            if (amountToRemove <= 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }

        void unlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// A basic container for log details.
    /// </summary>
    struct Log
    {
        public int count;
        public string message;
        public string stackTrace;
        public LogType type;

        /// <summary>
        /// The max string length supported by UnityEngine.GUILayout.Label without triggering this error:
        /// "String too long for TextMeshGenerator. Cutting off characters."
        /// </summary>
        const int maxMessageLength = 16382;

        public bool Equals(Log log)
        {
            return message == log.message && stackTrace == log.stackTrace && type == log.type;
        }

        /// <summary>
        /// Return a truncated message if it exceeds the max message length.
        /// </summary>
        public string GetTruncatedMessage()
        {
            if (string.IsNullOrEmpty(message))
            {
                return message;
            }

            return message.Length <= maxMessageLength ? message : message.Substring(0, maxMessageLength);
        }
    }

    /// <summary>
    /// Alternative to System.Collections.Concurrent.ConcurrentQueue
    /// (It's only available in .NET 4.0 and greater)
    /// </summary>
    /// <remarks>
    /// It's a bit slow (as it uses locks), and only provides a small subset of the interface
    /// Overall, the implementation is intended to be simple & robust
    /// </remarks>
    class ConcurrentQueue<T>
    {
        readonly Queue<T> queue = new Queue<T>();
        readonly object queueLock = new object();

        public void Enqueue(T item)
        {
            lock (queueLock)
            {
                queue.Enqueue(item);
            }
        }

        public bool TryDequeue(out T result)
        {
            lock (queueLock)
            {
                if (queue.Count == 0)
                {
                    result = default(T);
                    return false;
                }

                result = queue.Dequeue();
                return true;
            }
        }
    }
}