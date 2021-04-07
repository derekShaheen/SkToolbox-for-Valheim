using SkToolbox.Configuration;
using SkToolbox.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkToolbox.Utility.SkUtilities;

namespace SkToolbox
{
    /// <summary>
    /// Designed to control the menu processes. Menu operations can be requested/performed via this class.
    /// </summary>

    internal class SkMenuController : MonoBehaviour
    {
        private string contextTipInfo1 = "NumPad Arrows";
        private string contextTipInfo2 = "NumPad 5 to Select";
        private string contextTipInfo3 = "NumPad . to Back";
        internal static Version SkMenuControllerVersion = new Version(1, 1, 4); // 12/2020

        internal static Status SkMenuControllerStatus = Status.Initialized;

        private readonly string appName = "SkToolbox";
        private readonly string welcomeMsg = "[SkToolbox Loaded]\nPress NumPad 0\nto Toggle Menu.";
        //private readonly string welcomeMsg = "[SkToolbox Loaded]\nPress NumPad 0\nto Acknowledge.";
        private readonly string welcomeMotd = "";

        private bool firstRun = false;
        private bool InitialCheck = true;
        internal bool logResponse = false;

        private bool MenuOpen = false;
        private bool SubMenuOpen = false;
        private bool menuProcessInitialOptSize = true; // Scale the main menu on start
        private bool subMenuProcessInitialOptSize = true; // Scale the submenu upon request
        //public float InputDelay = 0.2f;
        //public float CurrentInputDelay = 0f;
        private int MenuSelection = 1;
        private int SubMenuSelection = 1;
        private int maxSelectionOption;
        private int subMaxSelectionOption = 1;
        private int subMenuMaxItemsPerPage = 12;
        private int subMenuCurrentPage = 1;
        //private List<SkMenuItem> menuOptions;
        public List<SkModules.SkBaseModule> MenuOptions;
        public List<SkMenuItem> SubMenuOptions;
        public List<SkMenuItem> SubMenuOptionsDisplay;
        private SkModuleController SkModuleController;

        //GUI Positions
        private int ypos_initial = 0;
        private int ypos_offset = 22;
        private int mWidth = 0; // Main Menu width
        private int subMenu_xpos_offset = 35;
        private int sWidth = 0; // Extra submenu width

        private Color menuOptionHighlight = Color.cyan; // Colorblind friendly colors // Cyan for the currently highlighted item
        private Color menuOptionSelected = Color.yellow; // Set to yellow when the option is actually selected

        //Keycodes
        internal Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>()
        {
            { "selToggle",  KeyCode.Keypad0 },
            { "selUp",      KeyCode.Keypad8 },
            { "selDown",    KeyCode.Keypad2 },
            { "selChoose",  KeyCode.Keypad5 },
            { "selBack",    KeyCode.KeypadPeriod }
        };

        void Start()
        {
            SkMenuControllerStatus = Status.Loading;
            SkUtilities.Logz(new string[] { "CONTROLLER", "NOTIFY" }, new string[] { "LOADING...", "WAITING FOR TOOLBOX." }); // Notify the console that the menu is ready
            SkModuleController = gameObject.AddComponent<SkModuleController>(); // Load our module controller
        }


        void Update()
        {
            if(Console.instance != null && global::Console.IsVisible())
            {
                MenuOpen = false;
            }
            if (SkCommandProcessor.altOnScreenControls)
            {
                if(SkConfigEntry.OAltToggle != null && SkConfigEntry.OAltUp != null && SkConfigEntry.OAltDown != null && SkConfigEntry.OAltChoose != null && SkConfigEntry.OAltBack != null)
                {
                    contextTipInfo1 = SkConfigEntry.OAltToggle.Value + "/" + SkConfigEntry.OAltUp.Value + "/" + SkConfigEntry.OAltDown.Value;
                    contextTipInfo2 = SkConfigEntry.OAltChoose.Value + " Button";
                    contextTipInfo3 = SkConfigEntry.OAltBack.Value + " Button";
                    keyBindings["selToggle"] = (KeyCode)Enum.Parse(typeof(KeyCode), SkConfigEntry.OAltToggle.Value);
                    keyBindings["selUp"] = (KeyCode)Enum.Parse(typeof(KeyCode), SkConfigEntry.OAltUp.Value);
                    keyBindings["selDown"] = (KeyCode)Enum.Parse(typeof(KeyCode), SkConfigEntry.OAltDown.Value);
                    keyBindings["selChoose"] = (KeyCode)Enum.Parse(typeof(KeyCode), SkConfigEntry.OAltChoose.Value);
                    keyBindings["selBack"] = (KeyCode)Enum.Parse(typeof(KeyCode), SkConfigEntry.OAltBack.Value);
                } else
                {
                    contextTipInfo1 = "Home/PgUp/PgDn";
                    contextTipInfo2 = "Insert Button";
                    contextTipInfo3 = "Delete Button";
                    keyBindings["selToggle"] = KeyCode.Home;
                    keyBindings["selUp"] = KeyCode.PageUp;
                    keyBindings["selDown"] = KeyCode.PageDown;
                    keyBindings["selChoose"] = KeyCode.Insert;
                    keyBindings["selBack"] = KeyCode.Delete;
                }
            }
            else
            {
                contextTipInfo1 = "NumPad Arrows";
                contextTipInfo2 = "NumPad 5 to Select";
                contextTipInfo3 = "NumPad . to Back";
                keyBindings["selToggle"] = KeyCode.Keypad0;
                keyBindings["selUp"] = KeyCode.Keypad8;
                keyBindings["selDown"] = KeyCode.Keypad2;
                keyBindings["selChoose"] = KeyCode.Keypad5;
                keyBindings["selBack"] = KeyCode.KeypadPeriod;
            }
            if (InitialCheck) // It takes a frame to load the components. Attempt to load menu options in second frame.
            {
                if (MenuOptions?.Count == 0) // If there is no menu, try to refresh it from SkMain
                { // There will be at least one frame where there is no menu when initialized
                    UpdateMenuOptions(SkModuleController.GetOptions());
                }
                else
                {
                    SkMenuControllerStatus = Status.Ready;
                    if (SkMenuControllerStatus == Status.Ready && SkModuleController.SkMainStatus == Status.Ready)
                    {
                        InitialCheck = false;
                        SkUtilities.Logz(new string[] { "CONTROLLER", "NOTIFY" }, new string[] { "READY." }); // Notify the console that the menu is ready
                    }
                }
            }
            //Keycode menu activation
            if (Input.GetKeyDown(keyBindings["selToggle"]) 
                && Console.instance != null && !global::Console.IsVisible() 
                && Chat.instance != null && !global::Chat.instance.m_input.isFocused)
            {
                firstRun = false;
                MenuOpen = !MenuOpen;
                //MenuOpen = false;
            }
            if (MenuOpen) // Menu is open
            {
                if (!SubMenuOpen) // Main menu
                {

                    //if (Input.GetKey(keyBindings["selDown"]) && CurrentInputDelay <= 0)
                    if (Input.GetKeyDown(keyBindings["selDown"]))
                    {
                        //CurrentInputDelay = InputDelay;
                        SubMenuSelection = 1;
                        if (MenuSelection != maxSelectionOption)
                        {
                            MenuSelection += 1;
                        }
                        else
                        {
                            MenuSelection = 1;
                        }
                    }

                    //if (Input.GetKey(keyBindings["selUp"]) && CurrentInputDelay <= 0)
                    if (Input.GetKeyDown(keyBindings["selUp"]))
                    {
                        //CurrentInputDelay = InputDelay;
                        SubMenuSelection = 1;
                        if (MenuSelection != 1)
                        {
                            MenuSelection -= 1;
                        }
                        else
                        {
                            MenuSelection = maxSelectionOption;
                        }
                    }

                    if (Input.GetKeyDown(keyBindings["selChoose"]))
                    {
                        try
                        {
                            RunMethod(MenuOptions[MenuSelection - 1].CallerEntry.ItemClass);
                        }
                        catch (Exception ex)
                        {
                            SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
                        }
                    }
                }
                else // We are in the submenu
                {
                    //if (Input.GetKey(keyBindings["selDown"]) && CurrentInputDelay <= 0)
                    if (Input.GetKeyDown(keyBindings["selDown"]))
                    {
                        //CurrentInputDelay = InputDelay;
                        if (SubMenuSelection != subMaxSelectionOption)
                        {
                            SubMenuSelection += 1;
                        }
                        else
                        {
                            SubMenuSelection = 1;
                        }
                    }

                    //if (Input.GetKey(keyBindings["selUp"]) && CurrentInputDelay <= 0)
                    if (Input.GetKeyDown(keyBindings["selUp"]))
                    {
                        //CurrentInputDelay = InputDelay;
                        if (SubMenuSelection != 1)
                        {
                            SubMenuSelection -= 1;
                        }
                        else
                        {
                            SubMenuSelection = subMaxSelectionOption;
                        }
                    }

                    if (Input.GetKeyDown(keyBindings["selChoose"]))
                    {
                        SubMenuOpen = false;
                        try
                        {
                            RunMethod(SubMenuOptionsDisplay[SubMenuSelection - 1].ItemClass); // Pass back the method and parameter
                        }
                        catch (Exception)
                        {
                            try
                            {
                                if (SubMenuOptionsDisplay[SubMenuSelection - 1].ItemText.Equals("Next >"))
                                {
                                    IncreasePage();
                                }
                                else if (SubMenuOptionsDisplay[SubMenuSelection - 1].ItemText.Equals("< Previous"))
                                {
                                    DecreasePage();
                                }
                                else
                                {
                                    RunMethod(SubMenuOptionsDisplay[SubMenuSelection - 1].ItemClassStr, SubMenuOptionsDisplay[SubMenuSelection - 1].ItemText); // Pass back the method and parameter

                                }
                            }
                            catch (Exception ex)
                            {
                                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
                            }
                        }
                    }
                }
                if (Input.GetKeyDown(keyBindings["selBack"])) // Menu is open, but regardless of main or submenu...
                {
                    if (!SubMenuOpen)
                    {
                        MenuOpen = false;
                    }
                    SubMenuOpen = false;
                }
            }
            //CurrentInputDelay -= Time.deltaTime;
            //CurrentInputDelay = Mathf.Clamp(CurrentInputDelay, 0f, 999f); // Prevent this from dipping beneath 0
        }

        void OnGUI()
        {
            GUI.color = Color.white;

            if (firstRun)
            { // Display the greeting message
                //GUI.Box(new Rect(10, ypos_initial + ypos_offset, 150, 55), welcomeMsg);
                var WelcomeWindow = GUILayout.Window(49101, new Rect(7, ypos_initial, 150, 55), ProcessWelcome, "");
            }

            if (MenuOptions == null || MenuOptions.Count == 0 || ypos_initial == 0) // If there is no menu, try to refresh it from SkMain // Also if the Screen.* was not able to be calculated,
                                                                                    //      ypos_initial will also still be 0, and the menu will appear in the top left corner.
            { // There will be at least one frame where there is no menu when initialized
                UpdateMenuOptions(SkModuleController.GetOptions());
            }
            else // We've received a menu from SkMain and can now display it
            {
                if (MenuOpen) // Display the menu components
                {
                    if (menuProcessInitialOptSize) // Calculate the X size of the text and store the highest value for the width
                    {
                        float largestCalculatedWidth = 0;
                        GUIStyle style = GUI.skin.box;
                        // Calculate width
                        foreach (SkModules.SkBaseModule optList in MenuOptions)
                        {
                            GUIContent menuTextItem = new GUIContent(optList.CallerEntry.ItemText);
                            Vector2 size = style.CalcSize(menuTextItem);
                            if (size.x > largestCalculatedWidth) largestCalculatedWidth = size.x;
                        }
                        mWidth = (mWidth == 0 ? Mathf.CeilToInt(largestCalculatedWidth) : mWidth);
                        mWidth = Mathf.Clamp(mWidth, 125, 1024); // Min/max size

                        menuProcessInitialOptSize = false; // Processing of the main menu size is complete, let's not calculate this every frame...
                    }
                    var MainWindow = GUILayout.Window(49000, new Rect(7, ypos_initial, mWidth + ypos_offset, 30 + (ypos_offset * MenuOptions.Count)), ProcessMainMenu, "- [" + appName + "] -");

                    //GUILayout.Window(49002, new Rect(7, MainWindow.y + MainWindow.height + ypos_offset, mWidth + ypos_offset, 30 + (ypos_offset * menuTipSize)), ProcessContextMenu, "- Context -");

                    if (SubMenuOpen)
                    {
                        if (subMenuProcessInitialOptSize) // only calculate the size once after the submenu was sent
                        {
                            float largestCalculatedWidth = 0;
                            GUIStyle style = GUI.skin.box;
                            // Calculate width
                            foreach (SkMenuItem optList in SubMenuOptions)
                            {
                                GUIContent menuTextItem = new GUIContent(optList.ItemText);
                                Vector2 size = style.CalcSize(menuTextItem);
                                if (size.x > largestCalculatedWidth) largestCalculatedWidth = size.x;
                            }
                            sWidth = (sWidth == 0 ? Mathf.CeilToInt(largestCalculatedWidth) : sWidth);
                            sWidth = Mathf.Clamp(sWidth, 105, 1024); // Min/max width
                        }

                        if (SubMenuOptions.Count > subMenuMaxItemsPerPage)
                        { // This will display the submenu box and title. It will also display what page we are on, and the maximum number of pages.
                            GUILayout.Window(49001, new Rect(mWidth + subMenu_xpos_offset, ypos_initial - ypos_offset, sWidth + subMenu_xpos_offset, (30 + (ypos_offset * subMenuMaxItemsPerPage))), ProcessSubMenu,
                                "- Submenu - " + subMenuCurrentPage + "/" + (Mathf.Ceil(SubMenuOptions.Count / subMenuMaxItemsPerPage) + (SubMenuOptions.Count % subMenuMaxItemsPerPage == 0 ? 0 : 1)));
                        }
                        else
                        {
                            GUILayout.Window(49001, new Rect(mWidth + subMenu_xpos_offset, ypos_initial - ypos_offset, sWidth + subMenu_xpos_offset, (30 + (ypos_offset * SubMenuOptions.Count))), ProcessSubMenu, "- Submenu -");
                        }
                    }
                }
            }

        }

        void ProcessMainMenu(int windowID)
        {
            try
            {
                ProcessMainMenu();
                ProcessContextMenu(windowID);
            }
            catch (Exception ex)
            {
                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
            }
        }

        void ProcessSubMenu(int windowID)
        {
            try
            {
                ProcessSubMenu(SubMenuOptions);
            }
            catch (Exception ex)
            {
                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
            }
        }

        void ProcessContextMenu(int windowID)
        {
            try
            {
                ProcessMenuTip();
            }
            catch (Exception ex)
            {
                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
            }
        }

        void ProcessWelcome(int windowID)
        {
            try
            {
                GUILayout.BeginVertical();
                GUILayout.Label(welcomeMsg);
                if (!welcomeMotd.Equals(""))
                {
                    GUILayout.Label(welcomeMotd);
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { ex.Message });
            }
        }

        /// <summary>
        /// Process the main menu options, display the background, and display the menu options
        /// </summary>
        private void ProcessMainMenu()
        {
            GUIStyle styMenuItems = new GUIStyle(GUI.skin.box);
            //styMenuItems.normal.background = null;

            GUILayout.BeginVertical();
            for (var i = 0; i < MenuOptions.Count; i++)
            {
                if (i == (MenuSelection - 1)) // These if statements perform color changing based on selections
                {
                    if (SubMenuOpen)
                    {
                        GUI.color = menuOptionSelected;
                    }
                    else
                    {
                        GUI.color = menuOptionHighlight;
                    }
                }
                else
                {
                    GUI.color = Color.white;
                }

                GUILayout.Label(MenuOptions[i].CallerEntry.ItemText, styMenuItems); // Display the label
            }
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }

        public void IncreasePage()
        {
            subMenuCurrentPage++;
            if (subMenuCurrentPage == 2) { SubMenuSelection++; }
            SubMenuOpen = true;
        }

        public void DecreasePage()
        {
            subMenuCurrentPage--;
            SubMenuOpen = true;
        }

        /// <summary>
        /// Used to generate the submenus based on incoming options in the subMenuOptions list. The menu can also be set to refresh automatically. The width can also be set instead of automatically calculated.
        /// </summary>
        /// <param name="subMenuOptions">SkMenuItem List containing the menu text, return methods, and context tips</param>
        /// <param name="refreshTime">If set to >0, the menu will refresh automatically based on the timer. The first method will be called for refresh.</param>
        /// <param name="subWidth">If set to >0, the submenu width will be set manually as opposed to automatic calculation.</param>
        public void RequestSubMenu(List<SkMenuItem> subMenuOptions, float refreshTime = 0, int subWidth = 0)
        {
            if (subMenuOptions != null && subMenuOptions.Count != 0)
            {
                subMenuCurrentPage = 1; // Reset the page to the first
                sWidth = subWidth; // Use custom width if passed in
                SubMenuOpen = true; // A submenu was requested, enable it
                subMenuProcessInitialOptSize = true; // Need to calculate sizes of the new submenu. This is later calculated only if we are not using the custom subWidth (subWidth <> 0)

                this.SubMenuOptions = subMenuOptions; // Set the submenu options
                subMaxSelectionOption = subMenuOptions.Count; // How many options are there?
                if (SubMenuSelection > subMenuOptions.Count) { SubMenuSelection = 1; } // Select 1st item if previous selection is out of bounds

                if (refreshTime > 0)
                { // Real time menu? Call the subroutine
                    refreshTime = Mathf.Clamp(refreshTime, 0.01f, 5f);
                    StartCoroutine(RealTimeMenuUpdate(refreshTime));
                }
                else
                { // Don't show the response for realtime menus, as it just spams the log
                    if (logResponse) SkUtilities.Logz(new string[] { "CONTROLLER", "RESP" }, new string[] { "Submenu created." });
                }
            }
        }

        /// <summary>
        /// This is an overload that allows passing in an SkMenu object which contains our list of menu items. Take in the menu, flush the options, pass it into the RequestSubMenu method to handle the items.
        /// </summary>
        /// <param name="subMenuOptions">SkMenuItem List containing the menu text, return methods, and context tips</param>
        /// <param name="refreshTime">If set to >0, the menu will refresh automatically based on the timer. The first method will be called for refresh.</param>
        /// <param name="subWidth">If set to >0, the submenu width will be set manually as opposed to automatic calculation.</param>
        public void RequestSubMenu(SkMenu subMenuOptions, float refreshTime = 0, int subWidth = 0)
        {
            if (subMenuOptions != null)
            {
                RequestSubMenu(subMenuOptions.FlushMenu(), refreshTime, subWidth);
            }
        }

        private void ProcessSubMenu(List<SkMenuItem> subMenuOptions)
        {
            if (SubMenuOpen)
            {
                GUIStyle styMenuItems = new GUIStyle(GUI.skin.box);

                /// Todo: Fix this so it doesn't need to run every frame
                if (subMenuOptions.Count > subMenuMaxItemsPerPage) // If there are more items than we can display on one page
                {
                    List<SkMenuItem> tempOptionsList = new List<SkMenuItem>();
                    if (subMenuCurrentPage > 1) // Should we display the previous button?
                    {
                        tempOptionsList.Add(new SkMenuItem("◄\tPrevious Page", () => DecreasePage(), "Previous Page"));
                    }
                    try
                    {
                        for (int x = ((subMenuMaxItemsPerPage * subMenuCurrentPage) - subMenuMaxItemsPerPage); // This will iterate over all items by page. The ternary on next line allows final page to have correct number of elements
                            x < ((subMenuOptions.Count > ((subMenuMaxItemsPerPage * (subMenuCurrentPage + 1)) - subMenuMaxItemsPerPage)) ? ((subMenuMaxItemsPerPage * (subMenuCurrentPage + 1)) - subMenuMaxItemsPerPage) : subMenuOptions.Count);
                            // Example: Is 35 items > (( 10 * (2 + 1)) - 10)? If it is, then there is another page after this one, and we can select a full page worth of items. Otherwise, use the final menu item as the end so we over get an index exception.
                            x++) // This selects the correct menu items to display for this page number
                        {
                            tempOptionsList.Add(subMenuOptions[x]);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        for (int x = ((subMenuMaxItemsPerPage * subMenuCurrentPage) - subMenuMaxItemsPerPage); x < subMenuOptions.Count - 1; x++) // This selects the correct menu items to display for this page number
                        {
                            tempOptionsList.Add(subMenuOptions[x]);
                        }
                        subMenuProcessInitialOptSize = true;
                    }

                    if ((subMenuOptions.Count > ((subMenuMaxItemsPerPage * (subMenuCurrentPage + 1)) - subMenuMaxItemsPerPage)))
                    { // Should we display the next button?
                        tempOptionsList.Add(new SkMenuItem("Next Page\t►", () => IncreasePage(), "Next Page"));
                    }

                    subMenuOptions = tempOptionsList;
                    SubMenuOptionsDisplay = tempOptionsList;
                    subMaxSelectionOption = subMenuOptions.Count; // How many options are there?
                    if (SubMenuSelection > subMenuOptions.Count) { SubMenuSelection = 1; }
                }
                else
                {
                    SubMenuOptionsDisplay = subMenuOptions;
                }

                GUILayout.BeginVertical();

                for (var i = 0; i < subMenuOptions.Count; i++)
                {
                    if (i == (SubMenuSelection - 1))
                    {
                        GUI.color = menuOptionHighlight;
                    }
                    else
                    {
                        GUI.color = Color.white;
                    }
                    GUILayout.Label(subMenuOptions[i].ItemText, styMenuItems);
                }
                GUILayout.EndVertical();
                GUI.color = Color.white;

            }
        }

        private void ProcessMenuTip()
        {
            GUIStyle styHeader = new GUIStyle(GUI.skin.label);
            styHeader.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginVertical();

            if (SubMenuOpen) // Display tip for submenu items
            {
                if (SubMenuOptionsDisplay[SubMenuSelection - 1].ItemTip != null)
                {
                    if (!SubMenuOptionsDisplay[SubMenuSelection - 1].ItemTip.Equals(""))
                    {
                        GUILayout.Label("- Context -", styHeader);
                        GUILayout.Label(SubMenuOptionsDisplay[SubMenuSelection - 1].ItemTip);
                    }
                }
            }
            else // Display tip for main menu items
            {
                if (MenuOptions[MenuSelection - 1].CallerEntry.ItemTip != null)
                {
                    if (!MenuOptions[MenuSelection - 1].CallerEntry.ItemTip.Equals(""))
                    {
                        GUILayout.Label("- Context -", styHeader);
                        GUILayout.Label(MenuOptions[MenuSelection - 1].CallerEntry.ItemTip);
                    }
                }
            }

            GUILayout.Label("- Controls -", styHeader);
            GUILayout.Label(contextTipInfo1);

            GUILayout.Label(contextTipInfo2);

            if (SubMenuOpen)
            {
                GUILayout.Label(contextTipInfo3);

            }
            GUILayout.EndVertical();
            GUI.color = Color.white;
        }

        private IEnumerator RealTimeMenuUpdate(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (SubMenuOpen)
            {
                RunMethod(SubMenuOptions[SubMenuSelection - 1].ItemClass);
            }
        }

        public void UpdateMenuOptions(List<SkModules.SkBaseModule> newMenuOptions)
        {
            SubMenuOpen = false;
            MenuOpen = false;
            MenuSelection = 1;
            SubMenuSelection = 1;
            MenuOptions = newMenuOptions;
            if (MenuOptions?.Count > 0)
            {
                ypos_initial = (Screen.height / 2) - (MenuOptions.Count / 2 * ypos_offset); // Rescale the Y axis calculation
                maxSelectionOption = MenuOptions.Count; // How many options were sent?
            }
            menuProcessInitialOptSize = true; // Initialize the main menu resize on next frame
        }

        private void RunMethod(Action methodName)
        {
            methodName.Invoke();
        }

        private void RunMethod(Action<string> methodName, string methodParameter = "")
        {
            try
            {// Try to invoke with string parameter
                methodName?.Invoke(methodParameter);
            }
            catch (Exception ex)
            {
                SkUtilities.Logz(new string[] { "CONTROLLER", "ERROR" }, new string[] { "Error running method. Likely not found... " + ex.Message }, LogType.Error);
            }
        }
    }
}