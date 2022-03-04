using BepInEx;
using SkToolbox.Configuration;
using SkToolbox.Utility;
using System;
using UnityEngine;
// Thank you to wh0am15533 for the BepInEx examples
namespace SkToolbox
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class SkBepInExLoader : BaseUnityPlugin
    {
        public const string
            MODNAME = "SkToolbox",
            AUTHOR = "Skrip",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.10.5.0";

        private void Start()
        {
            InitConfig();

            base.transform.parent = null;
            UnityEngine.Object.DontDestroyOnLoad(this);
            SkLoader.InitBepThreading(this);
        }

        public void InitConfig()
        {
            try
            {
                SkConfigEntry.CDescriptor = Config.Bind("- Index", "ThisIsJustAnIndex-NotASetting", true
                    , "Config sections:" +
                    "\n0 - General" +
                    "\n1 - Auto Run [Currently Disabled due to Hearth and Home patch. Fix coming soon]" +
                    "\n2 - Customize Console Look" +
                    "\n3 - Command Aliasing [Currently Disabled due to Hearth and Home patch. Fix coming soon]" +
                    "\n4 - On-Screen Menu" +
                    "\n5 - Command Hotkeys [Currently Disabled due to Hearth and Home patch. Fix coming soon]" +
                    "\n");

                SkConfigEntry.CConsoleEnabled = Config.Bind("0 - General", "ConsoleEnabled", true
                    , "Enables the console without launch option.");
                SkConfigEntry.CScrollable = Config.Bind("0 - General", "ConsoleScrollable", true
                    , "Enables the console to be scrollable.");
                SkConfigEntry.CScrollableLimit = Config.Bind("0 - General", "ConsoleScrollableLimit", 500
                    , "Maximum number of lines to store in the console. Game default = 30 (lol)");
                SkConfigEntry.CConsoleAutoComplete = Config.Bind("0 - General", "AutoComplete", true
                    , "Press tab to auto-complete SkToolbox commands if you have partially typed a command.");
                SkConfigEntry.CAllowChatCommandInput = Config.Bind("0 - General", "AllowChatCommandInput", true
                    , "Toggle this if you want to allow or disable the entry of commands in the chat. If this is disabled, you can only input commands into the console.");
                SkConfigEntry.CAllowPublicChatOutput = Config.Bind("0 - General", "AllowPublicResponse", true
                    , "Toggle this to allow the mod to respond publicly with certain commands, when entered into chat." +
                    "\nThe /portal command for example, if used in chat and this is true, others nearby will be able to see the response." +
                    "\nNOTE: If you see a response from your name, it is shown publicly and everyone can see it. If it is a response from (SkToolbox), only you see it.");
                SkConfigEntry.CAllowChatOutput = Config.Bind("0 - General", "AllowResponseInChat", true
                    , "Toggle this to allow the mod to show output in the chat. If this is disabled, the mod will not output to the chat at all, publicly or not.");
                SkConfigEntry.CAllowExecuteOnClear = Config.Bind("0 - General", "AllowExecuteOnClear", false
                    , "Toggle this to enable the ability to execute commands by clearing the input (by hitting escape, or selecting all and removing).");
                SkConfigEntry.COpenConsoleWithSlash = Config.Bind("0 - General", "OpenConsoleWithSlash", false
                    , "Toggle this to enable the ability to open the console with the slash (/) button." +
                    "\nThis option takes precedence over OpenChatWithSlash if both are true.");
                SkConfigEntry.COpenChatWithSlash = Config.Bind("0 - General", "OpenChatWithSlash", false
                    , "Toggle this to enable the ability to open chat with the slash (/) button.");

                SkConfigEntry.CAutoRun = Config.Bind("1 - AutoRun", "AutoRunEnabled", false
                    , "Toggle this to run commands automatically upon spawn into server." +
                    "\nNote this will only occur once per game launch to prevent unintended command executions.");
                SkConfigEntry.CAutoRunCommand = Config.Bind("1 - AutoRun", "AutoRunCommand", "/nosup; /god"
                    , "Enter the commands to run upon spawn here. Seperate commands with a semicolon (;).");

                SkConfigEntry.OAltToggle = Config.Bind("4 - OnScreenMenu", "AltKeyToggle", "Home"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for toggling the menu." +
                    "\nValid key codes are found here: https://docs.unity3d.com/ScriptReference/KeyCode.html");
                SkConfigEntry.OAltUp = Config.Bind("4 - OnScreenMenu", "AltKeyUp", "PageUp"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for changing the on-screen menu selection upwards.");
                SkConfigEntry.OAltDown = Config.Bind("4 - OnScreenMenu", "AltKeyDown", "PageDown"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for changing the on-screen menu selection downwards.");
                SkConfigEntry.OAltChoose = Config.Bind("4 - OnScreenMenu", "AltKeyChoose", "Insert"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for choosing the selected on-screen menu option.");
                SkConfigEntry.OAltBack = Config.Bind("4 - OnScreenMenu", "AltKeyBack", "Delete"
                    , "If /alt has been used (in-game or via auto run), the on screen menu will use this bind for going back in the on-screen menu.");


                SkConfigEntry.CAllowLookCustomizations = Config.Bind("2 - CustomizeConsoleLook", "ConsoleAllowLookCustomizations", true
                    , "Toggle this to enable or disable the customization settings below.");

                SkConfigEntry.CConsoleFont = Config.Bind("2 - CustomizeConsoleLook", "ConsoleFont", "Consolas"
                    , "Set the font size of the text in the console. Game default = AveriaSansLibre-Bold");
                SkConfigEntry.CConsoleFontSize = Config.Bind("2 - CustomizeConsoleLook", "ConsoleFontSize", 18
                    , "Set the font size of the text in the console. Game default = 18");

                SkConfigEntry.CConsoleOutputTextColor = Config.Bind("2 - CustomizeConsoleLook", "ConsoleOutputTextColor", "#E6F7FFFF"
                    , "Set the color of the output text shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");
                SkConfigEntry.CConsoleInputTextColor = Config.Bind("2 - CustomizeConsoleLook", "ConsoleInputTextColor", "#E6F7FFFF"
                    , "Set the color of the input text shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");
                SkConfigEntry.CConsoleSelectionColor = Config.Bind("2 - CustomizeConsoleLook", "ConsoleSelectionColor", "#A8CEFFC0"
                    , "Set the color of the selection highlight in the console. Game default = #A8CEFFC0. Color format is #RRGGBBAA");
                SkConfigEntry.CConsoleCaretColor = Config.Bind("2 - CustomizeConsoleLook", "ConsoleCaretColor", "#DCE6F5FF"
                    , "Set the color of the input text caret shown in the console. Game default = #FFFFFFFF. Color format is #RRGGBBAA");

                SkConfigEntry.CAlias1 = Config.Bind("3 - CommandAliasing", "Alias1", "/creative: /god; /ghost; /imacheater; /nores; /nocost; /echo Creative mode toggled!"
                    , "Set this to create a command alias. Specify what the user will type, and what will be executed. " +
                    "Command chaining does work here, and these aliases can be used with the AutoRun functionality above." +
                    " Note that aliases cannot reference other aliases. The slash (/) prefix is also not required - command aliases can be set to anything you want!" +
                    " Aliases do work from chat as well, if the execute from chat setting is enabled above. Alias commands also appear in the /? help menus." +
                    "\nFormat: WhatToType: WhatToExecute" +
                    "\nExample: This will create a new command, '/Cmd1', and when entered it will execute /god and /fly" +
                    "\n/Cmd1: /god; /fly" +
                    "\n##################" +
                    "\nSet this to create command alias 1.");
                SkConfigEntry.CAlias2 = Config.Bind("3 - CommandAliasing", "Alias2", ""
                    , "Set this to create command alias 2.");
                SkConfigEntry.CAlias3 = Config.Bind("3 - CommandAliasing", "Alias3", ""
                    , "Set this to create command alias 3.");
                SkConfigEntry.CAlias4 = Config.Bind("3 - CommandAliasing", "Alias4", ""
                    , "Set this to create command alias 4.");
                SkConfigEntry.CAlias5 = Config.Bind("3 - CommandAliasing", "Alias5", ""
                    , "Set this to create command alias 5.");
                SkConfigEntry.CAlias6 = Config.Bind("3 - CommandAliasing", "Alias6", ""
                    , "Set this to create command alias 6.");
                SkConfigEntry.CAlias7 = Config.Bind("3 - CommandAliasing", "Alias7", ""
                    , "Set this to create command alias 7.");
                SkConfigEntry.CAlias8 = Config.Bind("3 - CommandAliasing", "Alias8", ""
                    , "Set this to create command alias 8.");
                SkConfigEntry.CAlias9 = Config.Bind("3 - CommandAliasing", "Alias9", ""
                    , "Set this to create command alias 9.");
                SkConfigEntry.CAlias10 = Config.Bind("3 - CommandAliasing", "Alias10", ""
                    , "Set this to create command alias 10.");
                SkConfigEntry.CAlias11 = Config.Bind("3 - CommandAliasing", "Alias11", ""
                    , "Set this to create command alias 11.");
                SkConfigEntry.CAlias12 = Config.Bind("3 - CommandAliasing", "Alias12", ""
                    , "Set this to create command alias 12.");
                SkConfigEntry.CAlias13 = Config.Bind("3 - CommandAliasing", "Alias13", ""
                    , "Set this to create command alias 13.");
                SkConfigEntry.CAlias14 = Config.Bind("3 - CommandAliasing", "Alias14", ""
                    , "Set this to create command alias 14.");
                SkConfigEntry.CAlias15 = Config.Bind("3 - CommandAliasing", "Alias15", ""
                    , "Set this to create command alias 15.");

                SkConfigEntry.CHotkey1 = Config.Bind("5 - CommandHotkeys", "Hotkey 1", ""
                    , "Set this to create a hotkey for a command or command chain." +
                    "\nValid Format is WhatToPress: WhatToExecute" +
                    "\nExample 1 Press z to input /fly; /god        | z: /fly; /god" +
                    "\nExample 2 Press Shift+Z to input /creative   | Z: /creative" +
                    "\nExample 3 Press Tilde to open the console    | `: /console" +
                    "\nOnly 'Printable ASCII characters' are valid! (https://theasciicode.com.ar/). Capital letters make the hotkey require Shift before the hotkey press." +
                    " Only one command or command chain can be assigned to each key (if hotkey 1 assigns to q, hotkey 2 cannot also assign to q).");
                SkConfigEntry.CHotkey2 = Config.Bind("5 - CommandHotkeys", "Hotkey 2", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey3 = Config.Bind("5 - CommandHotkeys", "Hotkey 3", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey4 = Config.Bind("5 - CommandHotkeys", "Hotkey 4", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey5 = Config.Bind("5 - CommandHotkeys", "Hotkey 5", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey6 = Config.Bind("5 - CommandHotkeys", "Hotkey 6", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey7 = Config.Bind("5 - CommandHotkeys", "Hotkey 7", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey8 = Config.Bind("5 - CommandHotkeys", "Hotkey 8", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey9 = Config.Bind("5 - CommandHotkeys", "Hotkey 9", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey10 = Config.Bind("5 - CommandHotkeys", "Hotkey 10", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey11 = Config.Bind("5 - CommandHotkeys", "Hotkey 11", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey12 = Config.Bind("5 - CommandHotkeys", "Hotkey 12", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey13 = Config.Bind("5 - CommandHotkeys", "Hotkey 13", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey14 = Config.Bind("5 - CommandHotkeys", "Hotkey 14", ""
                    , "Set this to create a hotkey for a command or command chain.");
                SkConfigEntry.CHotkey15 = Config.Bind("5 - CommandHotkeys", "Hotkey 15", ""
                    , "Set this to create a hotkey for a command or command chain.");

            }
            catch (Exception Ex)
            {
                SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Could not load config. Please confirm there is a working version of BepInEx installed.",
                                                                            Ex.Message, Ex.Source}, LogType.Error);
            }
        }
    }
}