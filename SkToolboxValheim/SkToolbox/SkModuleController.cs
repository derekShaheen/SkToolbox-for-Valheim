using SkToolbox.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SkToolbox.Utility.SkUtilities;

namespace SkToolbox
{
    /// <summary>
    /// This class controls the modules that will be used to generate the menu for use in-game. This class will enforce modules being in a ready state, and will automatically unload the module if it enters an error state.
    /// </summary>
    public class SkModuleController : MonoBehaviour
    {
        #region Initializations
        private static Version SkMainVersion = new Version(1, 1, 3); // 12/2020

        internal Status SkMainStatus = Status.Initialized;

        private bool FirstLoad = true;
        private bool NeedLoadModules = true;
        private bool NeedRetry = false;
        private bool ErrorMonitor = false;
        private int RetryCount = 1; // Current load try
        private int RetryCountMax = 3; // How many frames should it check for ready before it unloads the module?

        SkMenuController menuController;
        //SkModules.ModConsoleOpt moduleConsole = new SkModules.ModConsoleOpt();
        //SkModules.ModTestMenu moduleTestMenu = new SkModules.ModTestMenu();


        private List<SkModules.SkBaseModule> MenuOptions { get; set; } = new List<SkModules.SkBaseModule>();
        private List<SkModules.SkBaseModule> RetryModule { get; set; } = new List<SkModules.SkBaseModule>();

        #endregion

        SkModules.ModConsole moduleConsole;
        //SkModules.ModGeneric moduleGeneric;
        SkModules.ModPlayer modulePlayer;
        SkModules.ModWorld moduleWorld;

        //

        #region UnityStandardMethods
        public void Start()
        {
            SkMainStatus = Status.Loading;
            SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "LOADING...", "MODULES LOADING..." }); // Notify the console that the menu is ready

            // Load the main menu
            BeginMainMenu();

            // Get the menu controller
            menuController = GetComponent<SkMenuController>();
            if (MenuOptions.Count > 0 && menuController != null)
            {
                SkMainStatus = Status.Loading;
            }
            else
            {
                SkMainStatus = Status.Error;
            }

            Init();

        }
        public void Update()
        {
            if (!FirstLoad)// This is set to false on the 2nd frame, giving the modules one frame to initialize and run their Start() method.
            {
                if (SkMainStatus == Status.Loading && NeedLoadModules && !NeedRetry) // Are we loading, still needing to load the modules, but don't need to retry yet...
                {
                    foreach (SkModules.SkBaseModule Module in MenuOptions)
                    {
                        SkUtilities.Logz(new string[] { "TOOLBOX", "MODULE", "NOTIFY" }, new string[] { "NAME: " + Module.ModuleName.ToUpper(), "STATUS: " + Module.ModuleStatus.ToString().ToUpper() });
                        if (Module.ModuleStatus != Status.Ready) // Log any modules that aren't ready
                        {
                            NeedRetry = true;
                            RetryModule.Add(Module);
                        }
                    }

                    if (!NeedRetry) // Nothing to retry
                    {
                        SkMainStatus = Status.Ready; // Ready
                        ErrorMonitor = true; // Enable the error monitor
                        RetryCount = 1; // Reset the retry counter for later
                    }
                    if (SkMainStatus == Status.Ready && MenuOptions.Count > 0)
                    {
                        NeedLoadModules = false; // Only run this once
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { MenuOptions.Count + " MODULES LOADED", "TOOLBOX READY." }); // Notify the console that the menu is ready
                    }
                    else if (SkMainStatus == Status.Error || MenuOptions.Count <= 0)
                    {
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { MenuOptions.Count + " MODULES LOADED", "TOOLBOX FAILED TO LOAD MODULES." }, LogType.Error); // Notify the console that the menu is ready
                    }
                }
                else if (SkMainStatus == Status.Loading && NeedRetry) // Need to check the modules again for ready status
                {
                    if (RetryCount < (RetryCountMax + 1))
                    {
                        for (var x = 0; x < RetryModule?.Count; x++)
                        {
                            SkUtilities.Logz(new string[] { "TOOLBOX", "MODULE", "NOTIFY", "RECHECK " + RetryCount },
                                    new string[] { "NAME: " + RetryModule[x].ModuleName.ToUpper(), "STATUS: " + RetryModule[x].ModuleStatus.ToString().ToUpper() });
                            if (RetryModule[x].ModuleStatus != Status.Ready)
                            {
                                SkMainStatus = Status.Loading;
                                NeedRetry = true;
                            }
                            else if (RetryModule[x].ModuleStatus == Status.Ready)
                            {
                                RetryModule.Remove(RetryModule[x]);
                                if (RetryModule.Count == 0)
                                {
                                    SkMainStatus = Status.Ready;
                                    break;
                                }
                            }
                        }
                        RetryCount++;
                    }
                    if (MenuOptions.Count <= 0)
                    {
                        SkMainStatus = Status.Error;
                    }

                    if (SkMainStatus == Status.Ready)
                    {
                        ErrorMonitor = true;
                        RetryCount = 1;
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { MenuOptions.Count + " MODULES LOADED", "TOOLBOX READY." }); // Notify the console that the menu is ready
                    }
                    else if (RetryCount >= (RetryCountMax + 1))
                    {
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "MODULE NOT MOVING TO READY STATUS.", "UNLOADING THE MODULE(S)." }, LogType.Warning); // Notify the console that the menu is ready
                        foreach (SkModules.SkBaseModule Module in RetryModule)
                        {
                            if (Module.ModuleStatus != Status.Ready)
                            {
                                Module.RemoveModule();
                                MenuOptions.Remove(Module);
                            }
                        }
                        RetryModule.Clear();
                        NeedRetry = false;
                        SkMainStatus = Status.Loading;
                        menuController.UpdateMenuOptions(MenuOptions);
                    }
                }
            }
            else
            {
                FirstLoad = false;
            }

            if (ErrorMonitor) // Everything is initialized. Monitor each module for error status and unload if required.
            {
                for (var Module = 0; Module < MenuOptions?.Count; Module++)
                {
                    if (MenuOptions[Module]?.ModuleStatus == Status.Error && !RetryModule.Contains(MenuOptions[Module]))
                    {
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "MODULE IN ERROR STATUS.", "CHECKING MODULE: " + MenuOptions[Module].ModuleName.ToUpper() }, LogType.Warning);
                        RetryModule.Add(MenuOptions[Module]);
                    }
                    else if (MenuOptions[Module]?.ModuleStatus == Status.Unload)
                    {
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "MODULE READY TO UNLOAD. UNLOADING MODULE: " + MenuOptions[Module].ModuleName.ToUpper() }, LogType.Warning); // Notify ready to unload
                        MenuOptions[Module].RemoveModule();
                        MenuOptions.Remove(MenuOptions[Module]);
                        menuController.UpdateMenuOptions(MenuOptions);
                    }
                }
                if (RetryModule?.Count > 0 && RetryCount < (RetryCountMax + 1)) // There are modules to check, and we have retry frames available
                {
                    for (var Module = 0; Module < RetryModule.Count; Module++)
                    {
                        if (RetryModule[Module].ModuleStatus == Status.Ready)
                        {
                            SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "MODULE READY.", "MODULE: " + RetryModule[Module].ModuleName.ToUpper() });
                            RetryModule.Remove(RetryModule[Module]);
                            if (RetryModule.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    RetryCount++;
                }
                else if (RetryModule?.Count > 0 && RetryCount >= (RetryCountMax + 1))
                {
                    foreach (SkModules.SkBaseModule Module in RetryModule)
                    {
                        if (Module.ModuleStatus != Status.Ready)
                        {
                            SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "COULD NOT RESOLVE ERROR.", "UNLOADING THE MODULE: " + Module.ModuleName.ToUpper() }, LogType.Warning); // Notify the console that the menu is ready
                            Module.RemoveModule();
                            MenuOptions.Remove(Module);
                        }
                    }
                    RetryModule.Clear();
                    RetryCount = 1;
                    menuController.UpdateMenuOptions(MenuOptions);
                    if (MenuOptions.Count == 0)
                    {
                        SkMainStatus = Status.Error;
                        SkUtilities.Logz(new string[] { "TOOLBOX", "NOTIFY" }, new string[] { "NO MODULES LOADED.", "TOOLBOX ENTERING ERROR STATE." }, LogType.Error); // Notify the console that the menu is ready
                    }

                }
            }

            OnUpdate();
        }

        internal List<SkModules.SkBaseModule> GetOptions()
        {
            return MenuOptions;
        }

        #endregion

        /// <summary>
        /// Each module must have an entry in this method. Each module must be added to this base object as a component, the CallerEntry must be set for that module, and the module must be added to the MenuOptions list.
        /// Example:
        /// moduleTestMenu = gameObject.AddComponent<SkModules.ModTestMenu>(); // Add the module as a component
        /// moduleTestMenu.CallerEntry = new SkMenuItem("Test Menu >", () => menuController.RequestSubMenu(moduleTestMenu.FlushMenu())); // Setup the CallerEntry. This specific CallerEntry is set to create a submenu with the options found in the module.
        /// MenuOptions.Add(moduleTestMenu); // Add the CallerEntry to the Main Menu so the module can be accessed.
        /// </summary>
        public void BeginMainMenu()
        {

            //RegisterModules();
            //Create a game object for each module
            moduleConsole = gameObject.AddComponent<SkModules.ModConsole>();
            //moduleGeneric = gameObject.AddComponent<SkModules.ModGeneric>();
            modulePlayer = gameObject.AddComponent<SkModules.ModPlayer>();
            moduleWorld = gameObject.AddComponent<SkModules.ModWorld>();

            //Create entry points on each module
            moduleConsole.CallerEntry = new SkMenuItem("Console Menu\t►", () => menuController.RequestSubMenu(moduleConsole.FlushMenu()));
            //moduleGeneric.CallerEntry = new SkMenuItem("Generic Menu\t►", () => menuController.RequestSubMenu(moduleGeneric.FlushMenu()), "Empty Menu with a long context tip");
            modulePlayer.CallerEntry = new SkMenuItem("Player Menu\t►", () => menuController.RequestSubMenu(modulePlayer.FlushMenu()));
            moduleWorld.CallerEntry = new SkMenuItem("World Menu\t►", () => menuController.RequestSubMenu(moduleWorld.FlushMenu()));

            // Add modules to the menu list
            // This is the order the menu items will be shown as well.
            MenuOptions.Add(modulePlayer);
            MenuOptions.Add(moduleWorld);
            MenuOptions.Add(moduleConsole);

            //MenuOptions.Add(moduleGeneric);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private void Init()
        {

        }

        private void OnUpdate()
        {

        }

        public void OnGUI()
        {

        }

        //
        //private void RegisterModules()
        //{
        //    var ourAssembly = AppDomain.CurrentDomain.GetAssemblies().
        //        SingleOrDefault(assembly => assembly == typeof(SkModules.SkBaseModule).Assembly);
        //    Type[] theseTypes;
        //    try
        //    {
        //        theseTypes = ourAssembly.GetTypes();
        //    }
        //    catch (ReflectionTypeLoadException e)
        //    {
        //        theseTypes = e.Types;
        //    }

        //    //Assembly thisAss = typeof(SkToolbox.SkModules.SkBaseModule).Assembly;
        //    //var theseTypes = thisAss.GetTypes();

        //    foreach (var subclass in theseTypes)
        //    {
        //        if (subclass.IsSubclassOf(typeof(SkModules.SkBaseModule)))
        //        {
        //            var module = gameObject.AddComponent(subclass);
        //            SkModules.SkBaseModule moduleTyped = (SkModules.SkBaseModule)GameObject.FindObjectOfType(subclass);

        //            moduleTyped.CallerEntry = new SkMenuItem(moduleTyped.ModuleName + "\t►", () => menuController.RequestSubMenu(moduleTyped.FlushMenu()));
        //            MenuOptions.Add(moduleTyped);
        //        }
        //    }

        //}

    }
}