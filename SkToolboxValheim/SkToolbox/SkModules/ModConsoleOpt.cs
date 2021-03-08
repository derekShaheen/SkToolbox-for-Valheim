using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace SkToolbox.SkModules
{
    internal class ModConsoleOpt : SkBaseModule, IModule
    {
        internal bool conWriteToFile = false;
        string consoleLastMessage = string.Empty;

        string chatInLastMessage = string.Empty;
        string chatOutLastMessage = string.Empty;

        private Rect EnemyWindow;

        History consoleHistory = new History();

        Rect rectCoords = new Rect(05, 020, 125, 20);
        Rect rectEnemy = new Rect(05, 415, 375, 50);

        bool anncounced1 = false;
        bool anncounced2 = false;


        List<Character> nearbyCharacters = new List<Character>();
        public ModConsoleOpt() : base()
        {
            base.ModuleName = "CC Controller";
            base.Loading();
        }

        public void Start()
        {
            SkCommandProcessor.consoleOpt = this;
            BeginMenu();
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

        //

        public void ToggleWriteFile()
        {
            conWriteToFile = !conWriteToFile;
            //SkToolbox.SkConsole.writeToFile = conWriteToFile;
            BeginMenu();
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
            SkLoader.Unload();
        }

        public void HookConsole()
        {             //Add console commands
            if (Console.instance != null)
            {
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
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        if (inputText.Equals(string.Empty) && !consoleLastMessage.Equals(string.Empty))
                        {
                            //if(!inputText.Equals(consoleHistory.Peek()))
                            //{
                            consoleHistory.Add(consoleLastMessage);
                            //}
                            SkCommandProcessor.ProcessCommands(consoleLastMessage, SkCommandProcessor.LogTo.Console);

                            consoleLastMessage = string.Empty;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        Console.instance.m_input.text = consoleHistory.Fetch(inputText, true);
                        Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        Console.instance.m_input.text = consoleHistory.Fetch(inputText, false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Slash)
                    && ((SkConfigEntry.cOpenConsoleWithSlash != null && SkConfigEntry.cOpenConsoleWithSlash.Value) || SkConfigEntry.cOpenConsoleWithSlash == null)
                    && !global::Console.IsVisible() && !global::Chat.instance.IsChatDialogWindowVisible() && !TextInput.IsVisible())
                {
                    Console.instance.m_chatWindow.gameObject.SetActive(true);
                    Console.instance.m_input.caretPosition = Console.instance.m_input.text.Length;
                }
            }
        }

        public void LoadConsoleCustomizations()
        {
            if(SkConfigEntry.cAllowLookCustomizations != null && SkConfigEntry.cAllowLookCustomizations.Value)
            {
                try
                {
                    int fontSize = Console.instance.m_output.fontSize;
                    string font = "Consolas";
                    Color outputColor = Console.instance.m_output.color;
                    Color inputColor = Console.instance.m_input.textComponent.color;
                    Color selectionColor = Console.instance.m_input.selectionColor;
                    Color caretColor = Color.white;
                    try
                    {
                        fontSize = SkConfigEntry.cConsoleFontSize.Value;
                        font = SkConfigEntry.cConsoleFont.Value;
                        ColorUtility.TryParseHtmlString(SkConfigEntry.cConsoleOutputTextColor.Value, out outputColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.cConsoleInputTextColor.Value, out inputColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.cConsoleSelectionColor.Value, out selectionColor);
                        ColorUtility.TryParseHtmlString(SkConfigEntry.cConsoleCaretColor.Value, out caretColor);
                    }
                    catch (Exception Ex)
                    {
                        SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Failed to load something from the config.", Ex.Message }, LogType.Warning);
                    }

                    Font consoleFont = Font.CreateDynamicFontFromOSFont(font, fontSize);

                    Console.instance.m_output.font = consoleFont;
                    Console.instance.m_output.fontSize = fontSize;
                    Console.instance.m_output.color = outputColor;

                    Console.instance.m_input.textComponent.color = inputColor;
                    Console.instance.m_input.textComponent.font = consoleFont;
                    Console.instance.m_input.selectionColor = selectionColor;
                    Console.instance.m_input.caretColor = caretColor;
                    Console.instance.m_input.customCaretColor = true;
                }
                catch (Exception Ex)
                {
                    SkUtilities.Logz(new string[] { "ERR" }, new string[] { "Failed to set when customizing console style." , Ex.Message }, LogType.Warning);
                }
            }
            
        }

        public void HookChat()
        {             //Add chat commands
            if (Chat.instance != null)
            {
                if (Chat.instance.m_chatWindow.gameObject.activeInHierarchy) // Chat is open
                {
                    string inputText = Chat.instance.m_input.text;
                    string outputText = Chat.instance.m_output.text;

                    //Input
                    if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) && (SkConfigEntry.cAllowExecuteOnClear != null && !SkConfigEntry.cAllowExecuteOnClear.Value))
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
                        && (SkConfigEntry.cOpenConsoleWithSlash != null && !SkConfigEntry.cOpenConsoleWithSlash.Value)
                        && (SkConfigEntry.cOpenChatWithSlash != null && SkConfigEntry.cOpenChatWithSlash.Value)
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

        public void OnCredits()
        {

        }

        void OnGUI()
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
            if(FejdStartup.instance != null && FejdStartup.instance.m_creditsPanel != null && FejdStartup.instance.m_creditsPanel.activeInHierarchy)
            {

                GUILayout.BeginArea(new Rect(0, Screen.height/4, Screen.width, Screen.height));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                Color tempColor = GUI.color;
                GUI.color = Color.yellow;
                GUIStyle credStyle = new GUIStyle();
                credStyle.font = (Font)Resources.Load("AveriaSansLibre-Bold");
                credStyle.fontStyle = FontStyle.Bold;
                credStyle.fontSize = 18;
                GUILayout.Label("<color=yellow>Skrip from NexusMods</color>", credStyle);
                GUI.color = tempColor;

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();
            }
        }

        void Update()
        {
            HookConsole();

            if (SkConfigEntry.cAllowChatCommandInput != null && SkConfigEntry.cAllowChatCommandInput.Value)
            {
                HookChat();
            }

            if (!anncounced1)
            {
                if (Console.instance != null && Player.m_localPlayer == null) // Only announce at main menu
                {
                    SkCommandProcessor.Announce();
                    LoadConsoleCustomizations();
                    anncounced1 = true;
                }
            }
            else
            {
                if (Console.instance == null)
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

            if (!SkConfigEntry.cAutoRunComplete)
            {
                if (SkConfigEntry.cAutoRun != null && SkConfigEntry.cAutoRun.Value == true)
                {
                    if (Player.m_localPlayer != null && Chat.instance != null && Console.instance != null) // Wait until fully logged in
                    {
                        try
                        {
                            SkUtilities.Logz(new string[] { "CMD", "AUTORUN" }, new string[] { "Command List:" + SkConfigEntry.cAutoRunCommand.Value });
                            SkCommandProcessor.PrintOut("==> AutoRun enabled! Command line: " + SkConfigEntry.cAutoRunCommand.Value, SkCommandProcessor.LogTo.Console);
                            SkCommandProcessor.ProcessCommands(SkConfigEntry.cAutoRunCommand.Value, SkCommandProcessor.LogTo.Console); // try to proces SkToolbox command
                        }
                        catch (Exception)
                        {
                            SkUtilities.Logz(new string[] { "Console" }, new string[] { "AutoRun Failed. Something went wrong. Command line: " + SkConfigEntry.cAutoRunCommand.Value }, LogType.Warning);
                            SkCommandProcessor.PrintOut("==> AutoRun Failed. Something went wrong. Command line: " + SkConfigEntry.cAutoRunCommand.Value, SkCommandProcessor.LogTo.Console);
                        }
                        finally
                        {
                            SkConfigEntry.cAutoRunComplete = true;
                        }
                    }
                }
                else
                {
                    SkConfigEntry.cAutoRunComplete = true;
                }
            }

            if (SkCommandProcessor.bTeleport)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
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
                            if (character != null && !character.IsDead() && !character.IsPlayer())
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

        void OnDestroy()
        {
            if (SkCommandPatcher.harmony != null)
            {
                SkCommandPatcher.harmony.UnpatchSelf();
            }
        }

        void ProcessEnemies(int WindowID)
        {
            if (Player.m_localPlayer != null)
            {
                GUILayout.BeginVertical();
                if (nearbyCharacters?.Count > 0)
                {
                    Vector3 playerPos = Player.m_localPlayer.transform.position;

                    foreach (Character toon in nearbyCharacters)
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

            private List<string> history = new List<string>();
            private int index;
            private string current;
        }
    }
}
