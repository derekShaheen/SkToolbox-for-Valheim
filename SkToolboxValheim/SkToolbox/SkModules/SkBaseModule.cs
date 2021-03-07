using System.Collections.Generic;
using UnityEngine;

namespace SkToolbox.SkModules
{
    /// <summary>
    /// All modules must inerhit from this base class. Within those modules, base.Ready() must be called when the module is ready for use, and this must happen within 3 frames of module initialization.
    /// </summary>
    public class SkBaseModule : MonoBehaviour
    {
        internal SkMenuController SkMC;
        public SkMenu MenuOptions { get; set; } = new SkMenu();
        internal SkMenuItem CallerEntry { get; set; } = new SkMenuItem();
        public SkUtilities.Status ModuleStatus { get; set; } = SkUtilities.Status.Initialized;
        internal string ModuleName = "UNNAMED";

        public SkBaseModule()
        {
            //Loading();
            //

            //Ready();
        }


        public void Awake()
        {
            SkMC = GetComponent<SkMenuController>();
        }

        public List<SkMenuItem> FlushMenu()
        {
          return MenuOptions.FlushMenu();
        }

        public void RequestMenu()
        {
            SkMC.RequestSubMenu(MenuOptions.FlushMenu());
        }

        public void RequestMenu(SkMenu Menu)
        {
            SkMC.RequestSubMenu(Menu);
        }

        public void RemoveModule()
        {

            Destroy(this);
        }

        internal void Ready()
        {
            ModuleStatus = SkUtilities.Status.Ready;
        }
        internal void Loading()
        {
            ModuleStatus = SkUtilities.Status.Loading;
        }
        internal void Error()
        {
            ModuleStatus = SkUtilities.Status.Error;
        }
        internal void Unload()
        {
            ModuleStatus = SkUtilities.Status.Unload;
        }
    }
}
