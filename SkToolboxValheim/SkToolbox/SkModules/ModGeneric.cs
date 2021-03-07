//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SkToolbox.SkModules
//{
//    class ModGeneric : SkBaseModule, IModule
//    {

//        public ModGeneric() : base()
//        {
//            base.ModuleName = "Generic";
//            base.Loading();
//        }

//        public void Start()
//        {
//            BeginMenu();
//            base.Ready(); // Must be called when the module has completed initialization. // End of Start
//        }

//        public void BeginMenu()
//        {
//            SkMenu GenericMenu = new SkMenu();
//            //    GenericMenu.AddItem("Timescale >", new Action(SayHello));
//            //    GenericMenu.AddItem("Gravity >", new Action(BeginGravityMenu));
//            MenuOptions = GenericMenu;
//        }

//        //public void BeginTimescaleMenu()
//        //{
//        //    SkMenu GenericMenu = new SkMenu();
//        //    GenericMenu.AddItem("Timescale >", new Action(toggleWriteFile), "Write log output to file?");
//        //    GenericMenu.AddItem("Open Log Folder", new Action(OpenLogFolder));
//        //    GenericMenu.AddItem("Reload Menu", new Action(ReloadMenu));
//        //    GenericMenu.AddItem("Unload Toolbox", new Action(UnloadMenu));
//        //    base.RequestMenu(GenericMenu);
//        //}
//        //public void BeginGravityMenu()
//        //{
//        //    SkMenu GenericMenu = new SkMenu();
//        //    GenericMenu.AddItem("Timescale >", new Action(toggleWriteFile), "Write log output to file?");
//        //    GenericMenu.AddItem("Open Log Folder", new Action(OpenLogFolder));
//        //    GenericMenu.AddItem("Reload Menu", new Action(ReloadMenu));
//        //    GenericMenu.AddItem("Unload Toolbox", new Action(UnloadMenu));
//        //    base.RequestMenu(GenericMenu);
//        //}


//    }
//}
