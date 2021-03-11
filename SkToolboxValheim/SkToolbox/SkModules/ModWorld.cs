using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkToolbox.SkModules
{
    internal class ModWorld : SkBaseModule, IModule
    {
        int radius = 5;
        int height = 5;
        public ModWorld() : base()
        {
            base.ModuleName = "World";
            base.Loading();
        }

        public void Start()
        {
            BeginMenu();
            base.Ready(); // Must be called when the module has completed initialization. // End of Start
        }

        public void BeginMenu()
        {
            SkMenu GenericMenu = new SkMenu();
            GenericMenu.AddItem("List Portals", new Action(ListPortals));
            GenericMenu.AddItem("Remove Drops", new Action(RemoveDrops));
            GenericMenu.AddItem("T - Level - " + radius, new Action(TLevel), "Current radius: " + radius);
            GenericMenu.AddItem("T - Raise - " + radius + " - " + height, new Action(TRaise), "Current radius: " + radius + "\nCurrent height: " + height);
            GenericMenu.AddItem("T - Dig - " + radius + " - " + height, new Action(TLower), "Current radius: " + radius + "\nCurrent height: " + height);
            GenericMenu.AddItem("T - Reset - " + radius + " - " + height, new Action(TReset), "Current radius: " + radius + "\nCurrent height: " + height);
            GenericMenu.AddItem("Set T Radius\t►", new Action(BeginRadiusMenu), "Current: " + radius);
            GenericMenu.AddItem("Set T Height\t►", new Action(BeginHeightMenu), "Current: " + height);
            //    GenericMenu.AddItem("Gravity >", new Action(BeginGravityMenu));
            MenuOptions = GenericMenu;
        }

        public void BeginRadiusMenu()
        {
            SkMenu GenericMenu = new SkMenu();
            GenericMenu.AddItem("5", new Action<string>(SetRadius), "Current: " + radius);
            GenericMenu.AddItem("7", new Action<string>(SetRadius), "Current: " + radius);
            GenericMenu.AddItem("10", new Action<string>(SetRadius), "Current: " + radius);
            GenericMenu.AddItem("20", new Action<string>(SetRadius), "Current: " + radius);
            GenericMenu.AddItem("30", new Action<string>(SetRadius), "Current: " + radius);
            GenericMenu.AddItem("40", new Action<string>(SetRadius), "Current: " + radius);
            base.RequestMenu(GenericMenu);
        }

        public void SetRadius(string ln = "")
        {
            radius = int.Parse(ln);
            BeginMenu();
        }


        public void BeginHeightMenu()
        {
            SkMenu GenericMenu = new SkMenu();
            GenericMenu.AddItem("1", new Action<string>(SetHeight), "Current: " + height);
            GenericMenu.AddItem("2", new Action<string>(SetHeight), "Current: " + height);
            GenericMenu.AddItem("3", new Action<string>(SetHeight), "Current: " + height);
            GenericMenu.AddItem("5", new Action<string>(SetHeight), "Current: " + height);
            GenericMenu.AddItem("6", new Action<string>(SetHeight), "Current: " + height);
            GenericMenu.AddItem("8", new Action<string>(SetHeight), "Current: " + height);
            base.RequestMenu(GenericMenu);
        }
        public void SetHeight(string ln = "")
        {
            height = int.Parse(ln);
            BeginMenu();
        }


        public void ListPortals()
        {
            SkCommandProcessor.ProcessCommand("/portals", SkCommandProcessor.LogTo.Chat);
        }
        public void RemoveDrops()
        {
            SkCommandProcessor.ProcessCommand("/removedrops", SkCommandProcessor.LogTo.Chat);
        }

        public void TLevel()
        {
            SkCommandProcessor.ProcessCommand("/tl " + radius, SkCommandProcessor.LogTo.Chat);
        }

        public void TRaise()
        {
            SkCommandProcessor.ProcessCommand("/tr " + radius + " " + height, SkCommandProcessor.LogTo.Chat);
        }
        public void TLower()
        {
            SkCommandProcessor.ProcessCommand("/td " + radius + " " + height, SkCommandProcessor.LogTo.Chat);
        }

        public void TReset()
        {
            SkCommandProcessor.ProcessCommand("/tu " + radius + " " + height, SkCommandProcessor.LogTo.Chat);
        }

    }
}
