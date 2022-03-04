using BepInEx.Configuration;

namespace SkToolbox.Configuration
{
    internal static class SkConfigEntry
    {
        public static ConfigEntry<bool> CDescriptor { get; set; }
        public static ConfigEntry<bool> CAutoRun { get; set; }
        public static bool CAutoRunComplete { get; set; }
        public static ConfigEntry<string> CAutoRunCommand { get; set; }

        public static ConfigEntry<bool> CAllowChatCommandInput { get; set; }
        public static ConfigEntry<bool> CAllowPublicChatOutput { get; set; }
        public static ConfigEntry<bool> CAllowChatOutput { get; set; }
        public static ConfigEntry<bool> CAllowExecuteOnClear { get; set; }
        public static ConfigEntry<bool> CConsoleEnabled { get; set; }
        public static ConfigEntry<bool> CScrollable { get; set; }
        public static ConfigEntry<int> CScrollableLimit { get; set; }
        public static ConfigEntry<bool> CConsoleAutoComplete { get; set; }

        public static ConfigEntry<bool> COpenChatWithSlash { get; set; }
        public static ConfigEntry<bool> COpenConsoleWithSlash { get; set; }

        public static ConfigEntry<bool> CAllowLookCustomizations { get; set; }
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

        public static ConfigEntry<string> CAlias1 { get; set; }
        public static ConfigEntry<string> CAlias2 { get; set; }
        public static ConfigEntry<string> CAlias3 { get; set; }
        public static ConfigEntry<string> CAlias4 { get; set; }
        public static ConfigEntry<string> CAlias5 { get; set; }
        public static ConfigEntry<string> CAlias6 { get; set; }
        public static ConfigEntry<string> CAlias7 { get; set; }
        public static ConfigEntry<string> CAlias8 { get; set; }
        public static ConfigEntry<string> CAlias9 { get; set; }
        public static ConfigEntry<string> CAlias10 { get; set; }
        public static ConfigEntry<string> CAlias11 { get; set; }
        public static ConfigEntry<string> CAlias12 { get; set; }
        public static ConfigEntry<string> CAlias13 { get; set; }
        public static ConfigEntry<string> CAlias14 { get; set; }
        public static ConfigEntry<string> CAlias15 { get; set; }

        public static ConfigEntry<string> CHotkey1 { get; set; }
        public static ConfigEntry<string> CHotkey2 { get; set; }
        public static ConfigEntry<string> CHotkey3 { get; set; }
        public static ConfigEntry<string> CHotkey4 { get; set; }
        public static ConfigEntry<string> CHotkey5 { get; set; }
        public static ConfigEntry<string> CHotkey6 { get; set; }
        public static ConfigEntry<string> CHotkey7 { get; set; }
        public static ConfigEntry<string> CHotkey8 { get; set; }
        public static ConfigEntry<string> CHotkey9 { get; set; }
        public static ConfigEntry<string> CHotkey10 { get; set; }
        public static ConfigEntry<string> CHotkey11 { get; set; }
        public static ConfigEntry<string> CHotkey12 { get; set; }
        public static ConfigEntry<string> CHotkey13 { get; set; }
        public static ConfigEntry<string> CHotkey14 { get; set; }
        public static ConfigEntry<string> CHotkey15 { get; set; }
    }
}
