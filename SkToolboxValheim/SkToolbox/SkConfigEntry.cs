using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkToolbox
{
    internal static class SkConfigEntry
    {
        public static ConfigEntry<bool> cAutoRun;
        public static bool cAutoRunComplete;
        public static ConfigEntry<string> autoRunCommand;

        public static ConfigEntry<bool> cAllowChatCommandInput;
        public static ConfigEntry<bool> cAllowPublicChatOutput;
        public static ConfigEntry<bool> cAllowExecuteOnClear;

        public static ConfigEntry<bool> cOpenChatWithSlash;
        public static ConfigEntry<bool> cOpenConsoleWithSlash;

        public static ConfigEntry<bool> cAllowLookCustomizations;
        public static ConfigEntry<int> cConsoleFontSize;
        public static ConfigEntry<string> cConsoleFont;
        public static ConfigEntry<string> cConsoleOutputTextColor;
        public static ConfigEntry<string> cConsoleSelectionColor;
        public static ConfigEntry<string> cConsoleCaretColor;

        public static ConfigEntry<string> oAltToggle;
        public static ConfigEntry<string> oAltUp;
        public static ConfigEntry<string> oAltDown;
        public static ConfigEntry<string> oAltChoose;
        public static ConfigEntry<string> oAltBack;
    }
}
