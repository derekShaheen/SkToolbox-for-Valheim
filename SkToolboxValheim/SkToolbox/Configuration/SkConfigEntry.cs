using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkToolbox.Configuration
{
    internal static class SkConfigEntry
    {
        public static ConfigEntry<bool> CAutoRun { get; set; }
        public static bool CAutoRunComplete { get; set; }
        public static ConfigEntry<string> CAutoRunCommand { get; set; }

        public static ConfigEntry<bool> CAllowChatCommandInput { get; set; }
        public static ConfigEntry<bool> CAllowPublicChatOutput { get; set; }
        public static ConfigEntry<bool> CAllowExecuteOnClear { get; set; }

        public static ConfigEntry<bool> COpenChatWithSlash { get; set; }
        public static ConfigEntry<bool> COpenConsoleWithSlash { get; set; }

        public static ConfigEntry<bool> cAllowLookCustomizations { get; set; }
        public static ConfigEntry<int> CConsoleFontSize { get; set; }
        public static ConfigEntry<string> CConsoleFont { get; set; }
        public static ConfigEntry<string> CConsoleOutputTextColor { get; set; }
        public static ConfigEntry<string> CConsoleInputTextColor { get; set; }
        public static ConfigEntry<string> CConsoleSelectionColor { get; set; }
        public static ConfigEntry<string> CConsoleCaretColor { get; set; }

        public static ConfigEntry<string> OAltToggle { get; set; }
        public static ConfigEntry<string> OAltUp { get; set; }
        public static ConfigEntry<string> OAltDown { get; set; }
        public static ConfigEntry<string> OAltChoose { get; set; }
        public static ConfigEntry<string> OAltBack { get; set; }
    }
}
