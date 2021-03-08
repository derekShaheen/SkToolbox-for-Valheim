using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;
// Thank you to wh0am15533 for the BepInEx examples
namespace SkToolbox
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    class SkBepInExLoader : BaseUnityPlugin
    {
        public const string
            MODNAME = "SkToolbox",
            AUTHOR = "Skrip",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.8.2.0";

        private void Start()
        {
            InitConfig();

            base.transform.parent = null;
            UnityEngine.Object.DontDestroyOnLoad(this);
            SkLoader.Init();
        }

        private void InitConfig()
        {
            try
            {
                SkConfigEntry.cAllowChatCommandInput = Config.Bind("General", "AllowChatCommandInput", true
                    , "Toggle this if you want to allow or disable the entry of commands in the chat. If this is disabled, you can only input commands into the console.");
                SkConfigEntry.cAllowPublicChatOutput = Config.Bind("General", "AllowPublicResponse", true
                    , "Toggle this to allow the mod to respond publicly with certain commands, when entered into chat." +
                    "\nThe /portal command for example, if used in chat and this is true, others nearby will be able to see the response." +
                    "\nNOTE: If you see a response from your name, it is shown publicly and everyone can see it. If it is a response from (SkToolbox), only you see it.");
                SkConfigEntry.cAllowExecuteOnClear = Config.Bind("General", "AllowExecuteOnClear", false
                    , "Toggle this to enable the ability to execute commands by clearing the input (by hitting escape, or selecting all and removing).");
                SkConfigEntry.cOpenConsoleWithSlash = Config.Bind("General", "OpenConsoleWithSlash", false
                    , "Toggle this to enable the ability to open the console with the slash (/) button." +
                    "\nThis option takes precedence over OpenChatWithSlash if both are true.");
                SkConfigEntry.cOpenChatWithSlash = Config.Bind("General", "OpenChatWithSlash", false
                    , "Toggle this to enable the ability to open chat with the slash (/) button.");

                SkConfigEntry.cAutoRun = Config.Bind("AutoRun", "AutoRunEnabled", false
                    , "Toggle this to run commands automatically upon spawn into server." +
                    "\nNote this will only occur once per game launch to prevent unintended command executions.");
                SkConfigEntry.cAutoRunCommand = Config.Bind("AutoRun", "AutoRunCommand", "/nosup; /god"
                    , "Enter the commands to run upon spawn here. Seperate commands with a semicolon (;).");

                SkConfigEntry.oAltToggle = Config.Bind("OnScreenMenu", "AltKeyToggle", "Home"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for toggling the menu." +
                    "\nValid key codes are found here: https://docs.unity3d.com/ScriptReference/KeyCode.html");
                SkConfigEntry.oAltUp = Config.Bind("OnScreenMenu", "AltKeyUp", "PageUp"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for changing the on-screen menu selection upwards.");
                SkConfigEntry.oAltDown = Config.Bind("OnScreenMenu", "AltKeyDown", "PageDown"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for changing the on-screen menu selection downwards.");
                SkConfigEntry.oAltChoose = Config.Bind("OnScreenMenu", "AltKeyChoose", "Insert"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for choosing the selected on-screen menu option.");
                SkConfigEntry.oAltBack = Config.Bind("OnScreenMenu", "AltKeyBack", "Delete"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for going back in the on-screen menu.");


                SkConfigEntry.cAllowLookCustomizations = Config.Bind("CustomizeConsoleLook", "ConsoleAllowLookCustomizations", true
                    , "Toggle this to enable or disable the customization settings below.");

                SkConfigEntry.cConsoleFont = Config.Bind("CustomizeConsoleLook", "ConsoleFont", "Consolas"
                    , "Set the font size of the text in the console. Game default = AveriaSansLibre-Bold");
                SkConfigEntry.cConsoleFontSize = Config.Bind("CustomizeConsoleLook", "ConsoleFontSize", 18
                    , "Set the font size of the text in the console. Game default = 18");

                SkConfigEntry.cConsoleOutputTextColor = Config.Bind("CustomizeConsoleLook", "ConsoleOutputTextColor", "#E6F7FFFF"
                    , "Set the color of the output text shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");
                SkConfigEntry.cConsoleInputTextColor = Config.Bind("CustomizeConsoleLook", "ConsoleInputTextColor", "#E6F7FFFF"
                    , "Set the color of the input text shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");
                SkConfigEntry.cConsoleSelectionColor = Config.Bind("CustomizeConsoleLook", "ConsoleSelectionColor", "#A8CEFFC0"
                    , "Set the color of the selection highlight in the console. Game default = #A8CEFFC0. Color format is #RRGGBBAA");
                SkConfigEntry.cConsoleCaretColor = Config.Bind("CustomizeConsoleLook", "ConsoleCaretColor", "#DCE6F5FF"
                    , "Set the color of the input text caret shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");
            }
            catch (Exception Ex)
            {
                SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Could not load config. Please confirm there is a working version of BepInEx installed.",
                                                                            Ex.Message, Ex.Source}, LogType.Error);
            }
        }
    }
}