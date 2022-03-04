using SkToolbox.Configuration;
using SkToolbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkToolbox.SkModules
{
    internal class ModConsole : SkBaseModule, IModule
    {
        internal bool conWriteToFile = false;
        private string consoleLastMessage = string.Empty;
        private string lastOutput = string.Empty;
        private string chatInLastMessage = string.Empty;

        private Rect EnemyWindow;
        private Rect ConsoleOutput;
        private GUIStyle ConsoleOutputStyle;
        private History consoleHistory = new History();
        public List<string> consoleOutputHistory = new List<string>();
        private Vector2 scrollPosition;
        private Rect rectCoords = new Rect(05, 020, 125, 20);
        private Rect rectEnemy = new Rect(05, 415, 375, 50);
        private Rect rectConsoleOutput = new Rect(0, 0, 500, 500);
        private bool anncounced1 = false;
        private bool anncounced2 = false;

        private int ConsoleOutputMaxHistory = 250;

        public Dictionary<string, string> AliasList = new Dictionary<string, string>();

        public List<string> PrefabList = new List<string>();
        public List<string> ItemList = new List<string>();
        private Dictionary<string, string> hotkeyList = new Dictionary<string, string>();
        private List<Character> nearbyCharacters = new List<Character>();
        private Terminal terminal;

        //SkConsole SkConsole;
        public ModConsole() : base()
        {
            base.ModuleName = "CC Controller";
            base.Loading();
        }

        public void Start()
        {
            SkCommandProcessor.ConsoleOpt = this;
            //SkConsole = gameObject.AddComponent<SkConsole>();
            BeginMenu();
            try
            {
                ConsoleOutputStyle = new GUIStyle();
                ConsoleOutputStyle.wordWrap = true;
                //Texture2D OutputTexture = new Texture2D(2, 2);
                //OutputTexture.SetPixels(new Color[] { Color.red, Color.red, Color.black, Color.black });
                //ConsoleOutputStyle.active.background = OutputTexture;
                BuildAliases();
                BuildHotkeys();
                //SkVersionChecker.VersionCurrent(); // Try to load the version so we don't have to when they open the console
                if(SkConfigEntry.CScrollableLimit != null)
                {
                    ConsoleOutputMaxHistory = SkConfigEntry.CScrollableLimit.Value;
                }
            }
            catch (Exception)
            {

            }
            base.Ready();
        }

        public void BeginMenu()
        {
            SkMenu consoleOptMenu = new SkMenu();
            consoleOptMenu.AddItem("Reload Toolbox", new Action(ReloadMenu));
            consoleOptMenu.AddItem("Unload Toolbox", new Action(UnloadMenu));
            consoleOptMenu.AddItem("Open Log Folder", new Action(OpenLogFolder));
            //consoleOptMenu.AddItemToggle("Write to File", ref conWriteToFile, new Action(ToggleWriteFile), "Write log output to file?");
            MenuOptions = consoleOptMenu;
        }


        public void OpenLogFolder()
        {
            SkUtilities.Logz(new string[] { "CMD", "REQ" }, new string[] { "Opening Log Directory" });
            Application.OpenURL(Application.persistentDataPath);
        }

        public void ReloadMenu()
        {
            SkUtilities.Logz(new string[] { "BASE", "CMD", "REQ" }, new string[] { "UNLOADING CONTROLLERS AND MODULES...", "SKTOOLBOX RELOAD REQUESTED.", });
            SkLoader.Reload();
        }

        public void UnloadMenu()
        {
            SkUtilities.Logz(new string[] { "BASE", "CMD", "REQ" }, new string[] { "SKTOOLBOX UNLOAD REQUESTED.", "UNLOADING CONTROLLERS AND MODULES..." });
            if (SkConfigEntry.CScrollable != null & SkConfigEntry.CScrollable.Value && Console.instance != null)
            {
                Console.instance.m_output.gameObject.SetActive(true);
            }
            SkLoader.Unload();
        }

        public void HandleConsole()
        {
            if(terminal == null)
            {
                if (Console.instance != null)
                {
                    terminal = SkUtilities.GetPrivateProperty<Terminal>(Console.instance, "m_terminalInstance");
                }
            }
            if (Console.instance != null && terminal != null)
            {
                //Scrollable console
                if (SkConfigEntry.CScrollable != null & SkConfigEntry.CScrollable.Value)
                {
                    List<string> consoleOutputBuffer = SkUtilities.GetPrivateField<List<string>>(Console.instance, "m_chatBuffer");
                    if (consoleOutputBuffer != null
                           && consoleOutputBuffer.Count > 0)
                    {
                        try
                        {
                            while (consoleOutputBuffer.Count > 0)
                            {
                                consoleOutputHistory.Add(consoleOutputBuffer[0]);
                                consoleOutputBuffer.RemoveAt(0);
                            }
                        } catch (Exception)
                        {
                            //
                        }

                        if (consoleOutputBuffer.Count == 0)
                        {
                            Console.instance.m_output.text = string.Empty;
                        }
                        scrollPosition = new Vector2(0, Int32.MaxValue);

                        // Make sure we don't actually make too many console entries
                        while (consoleOutputHistory.Count > ConsoleOutputMaxHistory)
                        {
                            consoleOutputHistory.RemoveAt(0);
                        }
                    }
                }
                if (Console.instance.m_chatWindow.gameObject.activeInHierarchy)
                {
                    string inputText = Console.instance.m_input.text;

                    if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
                    {
                        consoleLastMessage = string.Empty;
                    }
                    if (!inputText.Equals(string.Empty) && !inputText.Equals(consoleLastMessage))
                    {
                        consoleLastMessage = inputText;
                    }
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        if (inputText.Equals(string.Empty) && !consoleLastMessage.Equals(string.Empty))
                        {
                            consoleHistory.Add(consoleLastMessage);
                            //SkCommandProcessor.ProcessCommands(consoleLastMessage, SkCommandProcessor.LogTo.Console);

                            consoleLastMessage = string.Empty;
                        }
                    }
                    //if (Input.GetKeyDown(KeyCode.UpArrow))
                    //{
                    //    Console.instance.m_input.text = consoleHistory.Fetch(inputText, true);
                    //    Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                    //}
                    //if (Input.GetKeyDown(KeyCode.DownArrow))
                    //{
                    //    Console.instance.m_input.text = consoleHistory.Fetch(inputText, false);
                    //}
                }
                if (Input.GetKeyDown(KeyCode.Slash) // Open console with slash
                    && (SkConfigEntry.COpenConsoleWithSlash != null && SkConfigEntry.COpenConsoleWithSlash.Value)
                    && !global::Console.IsVisible() && !global::Chat.instance.IsChatDialogWindowVisible() && !TextInput.IsVisible())
                {
                    Console.instance.m_chatWindow.gameObject.SetActive(true);
                    Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                }

                //if (Input.GetKeyDown(KeyCode.Tab)) // Autocomplete
                //{
                //    if (SkConfigEntry.CConsoleAutoComplete != null && SkConfigEntry.CConsoleAutoComplete.Value
                //        && Console.instance != null && global::Console.IsVisible())
                //    {
                //        if (!string.IsNullOrEmpty(consoleLastMessage))
                //        {
                //            if (consoleLastMessage.StartsWith("/spawn")) // Spawn command
                //            {
                //                BuildPrefabs();

                //                string matchString = consoleLastMessage.Remove(0, 6);
                //                matchString = matchString.Trim();

                //                if (string.IsNullOrEmpty(matchString))
                //                {
                //                    matchString = "a";
                //                }
                //                try
                //                {
                //                    string matchPrefab = PrefabList.FirstOrDefault(item => !item.Equals(matchString)
                //                    && item.StartsWith(matchString, true,
                //                    System.Globalization.CultureInfo.InvariantCulture));
                //                    if (!string.IsNullOrEmpty(matchString))
                //                    {
                //                        Console.instance.m_input.text = "/spawn " + matchPrefab;
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }
                //            }
                //            else if (consoleLastMessage.StartsWith("/spawntamed")) // Spawn command
                //            {
                //                BuildPrefabs();

                //                string matchString = consoleLastMessage.Remove(0, 11);
                //                matchString = matchString.Trim();

                //                if (string.IsNullOrEmpty(matchString))
                //                {
                //                    matchString = "a";
                //                }

                //                try
                //                {
                //                    string matchItem = ItemList.FirstOrDefault(item => !item.Equals(matchString)
                //                    && item.StartsWith(matchString, true,
                //                    System.Globalization.CultureInfo.InvariantCulture));
                //                    if (!string.IsNullOrEmpty(matchString))
                //                    {
                //                        Console.instance.m_input.text = "/spawntamed " + matchItem;
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }
                //            }
                //            else if (consoleLastMessage.StartsWith("/give")) // Spawn command
                //            {
                //                BuildPrefabs();

                //                string matchString = consoleLastMessage.Remove(0, 5);
                //                matchString = matchString.Trim();

                //                if (string.IsNullOrEmpty(matchString))
                //                {
                //                    matchString = "Woo";
                //                }

                //                try
                //                {
                //                    string matchItem = ItemList.FirstOrDefault(item => !item.Equals(matchString)
                //                    && item.StartsWith(matchString, true,
                //                    System.Globalization.CultureInfo.InvariantCulture));
                //                    if (!string.IsNullOrEmpty(matchString))
                //                    {
                //                        Console.instance.m_input.text = "/give " + matchItem;
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }
                //            }
                //            else if (consoleLastMessage.StartsWith("/give")) // Spawn command
                //            {
                //                BuildPrefabs();

                //                string matchString = consoleLastMessage.Remove(0, 5);
                //                matchString = matchString.Trim();

                //                if (string.IsNullOrEmpty(matchString))
                //                {
                //                    matchString = "Woo";
                //                }

                //                try
                //                {
                //                    string matchItem = ItemList.FirstOrDefault(item => !item.Equals(matchString)
                //                    && item.StartsWith(matchString, true,
                //                    System.Globalization.CultureInfo.InvariantCulture));
                //                    if (!string.IsNullOrEmpty(matchString))
                //                    {
                //                        Console.instance.m_input.text = "/give " + matchItem;
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }
                //            }
                //            else if (consoleLastMessage.StartsWith("/env")) // env command
                //            {
                //                string matchString = consoleLastMessage.Remove(0, 4);
                //                matchString = matchString.Trim();

                //                if (string.IsNullOrEmpty(matchString))
                //                {
                //                    matchString = "Cle";
                //                }

                //                try
                //                {
                //                    string matchEnv = SkCommandProcessor.weatherList.FirstOrDefault(item => !item.Equals(matchString)
                //                    && item.StartsWith(matchString, true,
                //                    System.Globalization.CultureInfo.InvariantCulture));
                //                    if (!string.IsNullOrEmpty(matchString))
                //                    {
                //                        Console.instance.m_input.text = "/env " + matchEnv;
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }
                //            }
                //            else // Sktoolbox command or Alias
                //            {
                //                try
                //                {
                //                    KeyValuePair<string, string> matchCommand = SkCommandProcessor.commandList.FirstOrDefault(item => !item.Key.Equals(consoleLastMessage)
                //                        && item.Key.StartsWith(consoleLastMessage.Substring(0, Console.instance.m_input.caretPosition), true,
                //                        System.Globalization.CultureInfo.InvariantCulture));

                //                    KeyValuePair<string, string> matchAlias = AliasList.FirstOrDefault(item => !item.Key.Equals(consoleLastMessage)
                //                        && item.Key.StartsWith(consoleLastMessage.Substring(0, Console.instance.m_input.caretPosition), true,
                //                        System.Globalization.CultureInfo.InvariantCulture));

                //                    if (!string.IsNullOrEmpty(matchCommand.Key) || !string.IsNullOrEmpty(matchAlias.Key))
                //                    {
                //                        Console.instance.m_input.text = (string.IsNullOrEmpty(matchCommand.Key) ? matchAlias.Key : matchCommand.Key);
                //                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                //                    }
                //                }
                //                catch (Exception)
                //                {
                //                    //
                //                }

                //            }
                //        }
                //    }
                //}
            }
            base.ModuleStatus = SkUtilities.Status.Ready;
        }

        public void HandleChat()
        {             //Add chat commands
            if (Chat.instance != null)
            {
                if (Chat.instance.m_chatWindow.gameObject.activeInHierarchy) // Chat is open
                {
                    string inputText = Chat.instance.m_input.text;
                    string outputText = Chat.instance.m_output.text;

                    //Input
                    if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
                        && ((SkConfigEntry.CAllowExecuteOnClear != null && !SkConfigEntry.CAllowExecuteOnClear.Value) || SkConfigEntry.CAllowExecuteOnClear == null))
                    {
                        chatInLastMessage = string.Empty;
                    }
                    if (!inputText.Equals(string.Empty) && !inputText.Equals(consoleLastMessage))
                    {
                        chatInLastMessage = inputText;
                    }
                    if (inputText.Equals(string.Empty) && !chatInLastMessage.Equals(string.Empty)) // Enter was pressed
                    {
                        SkCommandProcessor.ProcessCommands(chatInLastMessage, SkCommandProcessor.LogTo.Chat);

                        chatInLastMessage = string.Empty;
                    }
                } // Chat is closed
                if (Input.GetKeyDown(KeyCode.Slash) && Player.m_localPlayer != null
                        && (SkConfigEntry.COpenConsoleWithSlash != null && !SkConfigEntry.COpenConsoleWithSlash.Value)
                        && (SkConfigEntry.COpenChatWithSlash != null && SkConfigEntry.COpenChatWithSlash.Value)
                        && !global::Console.IsVisible() && !global::Chat.instance.IsChatDialogWindowVisible() && !TextInput.IsVisible()
                        && !Minimap.InTextInput() && !Menu.IsVisible())
                {
                    SkUtilities.SetPrivateField(Chat.instance, "m_hideTimer", 0f);
                    Chat.instance.m_chatWindow.gameObject.SetActive(true);
                    Chat.instance.m_input.gameObject.SetActive(true);
                    Chat.instance.m_input.ActivateInputField();
                }
            }
        }

        public void LoadConsoleCustomizations()
        {
            if (SkConfigEntry.CAllowLookCustomizations != null && SkConfigEntry.CAllowLookCustomizations.Value)
            {
                try
                {
                    ConsoleOutputStyle = new GUIStyle();
                    ConsoleOutputStyle.wordWrap = true;

                    int fontSize = Console.instance.m_output.fontSize;
                    string font = "Consolas";
                    Color outputColor = Console.instance.m_output.color;
                    Color inputColor = Console.instance.m_input.textComponent.color;
                    Color selectionColor = Console.instance.m_input.selectionColor;
                    Color caretColor = Color.white;
                    try
                    {
                        fontSize = SkConfigEntry.CConsoleFontSize.Value;
                        font = SkConfigEntry.CConsoleFont.Value;
                        ColorUtility.TryParseHtmlString(SkConfigEntry.CConsoleOutputTextColor.Value, out outputColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.CConsoleInputTextColor.Value, out inputColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.CConsoleSelectionColor.Value, out selectionColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.CConsoleCaretColor.Value, out caretColor);
                    }
                    catch (Exception Ex)
                    {
                        SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Failed to load something from the config.", Ex.Message }, LogType.Warning);
                    }

                    Font consoleFont = Font.CreateDynamicFontFromOSFont(font, fontSize);

                    Console.instance.m_input.textComponent.color = inputColor;
                    Console.instance.m_input.textComponent.font = consoleFont;
                    Console.instance.m_input.selectionColor = selectionColor;
                    Console.instance.m_input.caretColor = caretColor;
                    Console.instance.m_input.customCaretColor = true;

                    Console.instance.m_output.font = consoleFont;
                    Console.instance.m_output.fontSize = fontSize;
                    Console.instance.m_output.color = outputColor;

                    if (SkConfigEntry.CScrollable != null & SkConfigEntry.CScrollable.Value)
                    {
                        ConsoleOutputStyle.fontSize = fontSize;
                        ConsoleOutputStyle.font = consoleFont;
                        ConsoleOutputStyle.normal.textColor = outputColor;
                    }
                }
                catch (Exception Ex)
                {
                    SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Failed to set when customizing console style.", Ex.Message }, LogType.Warning);
                }
            } else
            {
                if (SkConfigEntry.CScrollable != null & SkConfigEntry.CScrollable.Value)
                {
                    Font consoleFont = Font.CreateDynamicFontFromOSFont("Consolas", 18);
                    ConsoleOutputStyle.fontSize = 18;
                    ConsoleOutputStyle.font = consoleFont;
                    ConsoleOutputStyle.normal.textColor = Color.white;
                }
            }
            Console.instance.m_output.gameObject.SetActive(false);
            SkCommandProcessor.ProcessCommand("/clear", SkCommandProcessor.LogTo.Console);
            //Console.instance.Print("type \"help\" - for commands");
            //Console.instance.Print("");
        }

        private void OnGUI()
        {
            if (Player.m_localPlayer != null)
            {
                if (SkCommandProcessor.bDetectEnemies && nearbyCharacters.Count > 0)
                {
                    EnemyWindow = GUILayout.Window(39999, rectEnemy, ProcessEnemies, "Enemy Information");
                }
                if (SkCommandProcessor.bCoords)
                {
                    Vector3 plPos = Player.m_localPlayer.transform.position;
                    GUI.Label(rectCoords, "Coords: " + Mathf.RoundToInt(plPos.x) + "/" + Mathf.RoundToInt(plPos.z));
                }
            }
                if (Console.instance != null && global::Console.IsVisible())
                {
                    rectConsoleOutput = new Rect
                    {
                        x = 7,
                        y = 1,
                        width = Console.instance.m_output.rectTransform.rect.width - 7,
                        height = Console.instance.m_output.rectTransform.rect.height
                    };
                    if (SkVersionChecker.VersionCurrent())
                    {
                        ConsoleOutput = GUILayout.Window(39979, rectConsoleOutput, ProcessConsoleOutput, " SkToolbox (" + SkVersionChecker.currentVersion + ") by Skrip (DS)", ConsoleOutputStyle);
                    }
                    else
                    {
                        ConsoleOutput = GUILayout.Window(39979, rectConsoleOutput, ProcessConsoleOutput, " SkToolbox by Skrip (DS) ► " +
                            "New Version Available on NexusMods!\t► Current: " + SkVersionChecker.currentVersion + " Latest: " + SkVersionChecker.latestVersion, ConsoleOutputStyle);
                    }
                }
        }

        private void ProcessConsoleOutput(int windowID)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (SkConfigEntry.CConsoleFontSize != null)
            {
                GUILayout.Space(SkConfigEntry.CConsoleFontSize.Value);
            } else
            {
                GUILayout.Space(18);
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (string msg in consoleOutputHistory)
            {
                GUILayout.Label(msg, ConsoleOutputStyle);
                GUILayout.Space(7);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void BuildPrefabs()
        {
            if (PrefabList.Count == 0 && ZNetScene.instance != null)
            {
                try
                {
                    foreach (GameObject Prefab in SkUtilities.GetPrivateField<Dictionary<int, GameObject>>(ZNetScene.instance, "m_namedPrefabs").Values)
                    {
                        PrefabList.Add(Utils.GetPrefabName(Prefab));
                        ItemDrop ItemComponent = Prefab.GetComponent<ItemDrop>();
                        if (ItemComponent != null)
                        {
                            ItemList.Add(Utils.GetPrefabName(Prefab));
                        }
                    }
                }
                catch(Exception)
                {

                }
            }
        }

        private void Update()
        {
            if (Console.instance != null && SkConfigEntry.CConsoleEnabled.Value && !Console.instance.IsConsoleEnabled())
            {
                SkUtilities.SetPrivateField(Console.instance, "m_consoleEnabled", true);
            }

            try
            {
                HandleConsole();
            }
            catch (Exception Ex)
            {
                base.ModuleStatus = SkUtilities.Status.Error;
                SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Error detected!", Ex.Message, Ex.Source }, LogType.Warning);
                SkCommandProcessor.PrintOut("An error was detected. You may need to restart your game.", SkCommandProcessor.LogTo.Console);
            }


            if ((SkConfigEntry.CAllowChatCommandInput != null && SkConfigEntry.CAllowChatCommandInput.Value) || SkConfigEntry.CAllowChatCommandInput == null)
            {
                try
                {
                    HandleChat();
                }
                catch (Exception)
                {
                    base.ModuleStatus = SkUtilities.Status.Error;
                }
            }

            if (!anncounced1)
            {
                if (Console.instance != null && Player.m_localPlayer == null) // Only announce at main menu
                {
                    SkCommandProcessor.Announce();
                    LoadConsoleCustomizations();
                    SkCommandProcessor.InitCommands();
                    anncounced1 = true;
                }
            }
            else
            {
                if (Player.m_localPlayer != null)
                {
                    anncounced1 = false;
                }
            }

            if (!anncounced2) // Announce in game
            {
                if (Chat.instance != null)
                {
                    SkCommandProcessor.Announce();
                    LoadConsoleCustomizations();
                    anncounced2 = true;
                }
            }
            else
            {
                if (Chat.instance == null)
                {
                    anncounced2 = false;
                }
            }

            ProcessAutoRun();

            ProcessHotkeys();

            if (SkCommandProcessor.bTeleport)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    try
                    {
                        Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(rayCast, out hit))
                        {
                            Vector3 targetLoc = hit.point;
                            Player.m_localPlayer.transform.position = targetLoc;
                            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Warp!", 0, null);
                        }
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }
            if (SkCommandProcessor.bDetectEnemies)
            {
                try
                {
                    List<Character> charList = Character.GetAllCharacters();
                    if (charList.Count > 0)
                    {
                        foreach (Character character in charList)
                        {
                            if (character != null && Player.m_localPlayer != null && !character.IsDead() && !character.IsPlayer())
                            {
                                if (Vector3.Distance(character.transform.position, Player.m_localPlayer.transform.position) < SkCommandProcessor.bDetectRange)
                                {
                                    if (!nearbyCharacters.Contains(character))
                                    {
                                        nearbyCharacters.Add(character);
                                    }
                                }
                                else
                                {
                                    if (nearbyCharacters.Contains(character))
                                    {
                                        nearbyCharacters.Remove(character);
                                    }
                                }
                            }
                            else
                            {
                                if (nearbyCharacters.Contains(character))
                                {
                                    nearbyCharacters.Remove(character);
                                }
                            }
                        }

                        if (nearbyCharacters.Count > 0)
                        {
                            List<Character> tempCharList = new List<Character>(nearbyCharacters);
                            foreach (Character character in tempCharList)
                            {
                                if (!charList.Contains(character))
                                {
                                    nearbyCharacters.Remove(character);
                                }
                            }
                        }
                    }
                    if (nearbyCharacters.Count > 0 && SkCommandProcessor.btDetectEnemiesSwitch)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Enemy nearby!", 0, null);
                        SkCommandProcessor.btDetectEnemiesSwitch = false;
                    }
                    else if (nearbyCharacters.Count == 0)
                    {
                        SkCommandProcessor.btDetectEnemiesSwitch = true;
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        private void OnDestroy()
        {
            if (SkConfigEntry.CScrollable != null & SkConfigEntry.CScrollable.Value && Console.instance != null)
            {
                Console.instance.m_output.gameObject.SetActive(true);
            }
            if (SkCommandPatcher.Harmony != null)
            {
                SkCommandPatcher.Harmony.UnpatchSelf();
            }
        }

        private void ProcessHotkeys()
        {
            ////FIX
            //if (hotkeyList != null && hotkeyList.Count > 0)
            //{

            //    foreach (char c in Input.inputString)
            //    {
            //        if (Console.instance != null && !global::Console.IsVisible() && Chat.instance != null && !global::Chat.instance.m_input.isFocused)
            //        {
            //            try
            //            {
            //                string matchHotkey = hotkeyList.Keys.First(item => item.ToString().Equals(c.ToString()));
            //                //SkUtilities.Logz(new string[] { "CONTROLLER" }, new string[] { "Found: " + matchHotkey });

            //                if (!string.IsNullOrEmpty(matchHotkey.ToString()))
            //                {
            //                    //SkUtilities.Logz(new string[] { "CONTROLLER", "RUN HOTKEY" }, new string[] { "Command List: " + matchHotkey, hotkeyList[matchHotkey] });
            //                    SkCommandProcessor.ProcessCommands(hotkeyList[matchHotkey], SkCommandProcessor.LogTo.Chat | SkCommandProcessor.LogTo.Console);
            //                }
            //            }
            //            catch (Exception)
            //            {
            //                //
            //            }
            //        }
            //    }
            //}
        }

        private void ProcessAutoRun()
        {
            if (!SkConfigEntry.CAutoRunComplete)
            {
                if (SkConfigEntry.CAutoRun != null && SkConfigEntry.CAutoRun.Value == true)
                {
                    if (Player.m_localPlayer != null && Chat.instance != null && Console.instance != null) // Wait until fully logged in
                    {
                        try
                        {
                            ////FIX
                            SkCommandProcessor.PrintOut("Apologies, autorun was broken by Hearth and Home and is disabled this patch. Fix is in progress.", SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);
                            //SkUtilities.Logz(new string[] { "CONTROLLER", "AUTORUN" }, new string[] { "Command List: " + SkConfigEntry.CAutoRunCommand.Value });
                            //SkCommandProcessor.PrintOut("==> AutoRun enabled! Command line: " + SkConfigEntry.CAutoRunCommand.Value, SkCommandProcessor.LogTo.Console);
                            //SkCommandProcessor.ProcessCommands(SkConfigEntry.CAutoRunCommand.Value, SkCommandProcessor.LogTo.Console); // try to proces SkToolbox command
                        }
                        catch (Exception)
                        {
                            SkUtilities.Logz(new string[] { "CONTROLLER" }, new string[] { "AutoRun Failed. Something went wrong. Command line: " + SkConfigEntry.CAutoRunCommand.Value }, LogType.Warning);
                            SkCommandProcessor.PrintOut("==> AutoRun Failed. Something went wrong. Command line: " + SkConfigEntry.CAutoRunCommand.Value, SkCommandProcessor.LogTo.Console);
                        }
                        finally
                        {
                            SkConfigEntry.CAutoRunComplete = true;
                        }
                    }
                }
                else
                {
                    SkConfigEntry.CAutoRunComplete = true;
                }
            }
        }

        private void BuildHotkeys()
        {
            try
            {
                if (SkConfigEntry.CHotkey1 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey1.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey1.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey2 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey2.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey2.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey3 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey3.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey3.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey4 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey4.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey4.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey5 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey5.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey5.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey6 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey6.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey6.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey7 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey7.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey7.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey8 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey8.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey8.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey9 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey9.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey9.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey10 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey10.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey10.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey11 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey11.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey11.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey12 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey12.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey12.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey13 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey13.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey13.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey14 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey14.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey14.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                if (SkConfigEntry.CHotkey15 != null && !string.IsNullOrEmpty(SkConfigEntry.CHotkey15.Value))
                {
                    string[] HotkeySplit = SkConfigEntry.CHotkey15.Value.Split(':');
                    if (HotkeySplit.Length == 2)
                    {
                        hotkeyList.Add(HotkeySplit[0], HotkeySplit[1]);
                    }
                }
                ////FIX
                if(hotkeyList.Count > 0)
                {
                    SkCommandProcessor.PrintOut("Apologies, hotkeys was broken by Hearth and Home and is disabled this patch. Fix is in progress. Please attempt to use the new bind command.", SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);

                    hotkeyList.Clear();
                }
            }
            catch (Exception Ex)
            {
                SkCommandProcessor.PrintOut("Something failed while loading command hotkeys! Check your config file. " + Ex.Message, SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);
            }
        }

        private void BuildAliases()
        {
            try
            {
                if (SkConfigEntry.CAlias1 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias1.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias1.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias2 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias2.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias2.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias3 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias3.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias3.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias4 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias4.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias4.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias5 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias5.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias5.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias6 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias6.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias6.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias7 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias7.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias7.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias8 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias8.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias8.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias9 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias9.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias9.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias10 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias10.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias10.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias11 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias11.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias11.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias12 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias12.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias12.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias13 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias13.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias13.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias14 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias14.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias14.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }
                if (SkConfigEntry.CAlias15 != null && !string.IsNullOrEmpty(SkConfigEntry.CAlias15.Value))
                {
                    string[] AliasSplit = SkConfigEntry.CAlias15.Value.Split(':');
                    if (AliasSplit.Length == 2)
                    {
                        AliasList.Add(AliasSplit[0], AliasSplit[1]);
                    }
                }

                if (AliasList != null && AliasList.Count > 0)
                {
                    ////FIX
                    if (AliasList.Count > 0)
                    {
                        SkCommandProcessor.PrintOut("Apologies, command aliasing was broken by Hearth and Home and is disabled this patch. Fix is in progress.", SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);
                        hotkeyList.Clear();
                    }
                    //List<string> trashKeys = new List<string>();
                    //foreach (string key in AliasList.Keys)
                    //{
                    //    if (!SkCommandProcessor.commandList.Keys.Contains(key))
                    //    {
                    //        SkCommandProcessor.commandList.Add(key, "- (Alias) " + AliasList[key].Trim());
                    //    }
                    //    else
                    //    {
                    //        SkCommandProcessor.PrintOut("Alias\t► '" + key + "' is invalid. This command is already in the list.", SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);
                    //        trashKeys.Add(key);
                    //    }
                    //}
                    //if (trashKeys.Count > 0)
                    //{
                    //    foreach (string key in trashKeys)
                    //    {
                    //        AliasList.Remove(key);
                    //    }
                    //}
                }
            }
            catch (Exception Ex)
            {
                SkCommandProcessor.PrintOut("Something failed while loading command aliases! Check your config file. " + Ex.Message, SkCommandProcessor.LogTo.Console | SkCommandProcessor.LogTo.DebugConsole);
            }
        }

        private void ProcessEnemies(int WindowID)
        {
            if (Player.m_localPlayer != null)
            {
                GUILayout.BeginVertical();
                if (nearbyCharacters?.Count > 0)
                {
                    Vector3 playerPos = Player.m_localPlayer.transform.position;

                    foreach (Character toon in nearbyCharacters)
                    {
                        if(toon != null)
                        {
                            float toonDist = Vector3.Distance(playerPos, toon.transform.position);

                            if (toonDist > 15)
                            {
                                GUI.color = Color.green;
                            }
                            else if (toonDist > 10 && toonDist < 15)
                            {
                                GUI.color = Color.yellow;
                            }
                            else if (toonDist > 5 && toonDist < 10)
                            {
                                GUI.color = Color.yellow + Color.red;
                            }
                            else if (toonDist > 0 && toonDist < 5)
                            {
                                GUI.color = Color.red;
                            }
                            GUILayout.BeginHorizontal();

                            GUILayout.Label("Name: " + toon.GetHoverName());
                            GUILayout.Label("HP: " + Mathf.RoundToInt(toon.GetHealth()) + "/" + toon.GetMaxHealth()
                                + " | Level: " + toon.GetLevel()
                                + " | Dist: " + Mathf.RoundToInt(toonDist));

                            GUILayout.EndHorizontal();
                            GUI.color = Color.white;
                        }
                    }
                }
                GUILayout.EndVertical();
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
            }
        }

        private class History
        {
            public void Add(string item)
            {
                this.history.Add(item);
                this.index = 0;
            }

            public string Fetch(string current, bool next)
            {
                if (this.index == 0)
                {
                    this.current = current;
                }
                if (this.history.Count == 0)
                {
                    return current;
                }
                this.index += ((!next) ? 1 : -1);
                if (this.history.Count + this.index < 0 || this.history.Count + this.index > this.history.Count - 1)
                {
                    this.index = 0;
                    return this.current;
                }
                return this.history[this.history.Count + this.index];
            }

            public List<string> history = new List<string>();
            private int index;
            private string current;
        }
    }
}
