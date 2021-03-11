using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SkToolbox.SkModules
{
    internal class ModPlayer : SkBaseModule, IModule
    {
        //private Rect EnemyWindow;
        Player selectedPlayer = null;
        int spawnQuantity = 1;
        bool bTeleport = false;

        //GameObject gParent;
        //GameObject gObject;
        //bool gPickPut = false;

        Rect rectClock =    new Rect(05, 005, 125, 20);
        Rect rectCoords =   new Rect(05, 027, 125, 20);
        Rect rectEnemy =    new Rect(05, 280, 425, 50);

        List<Character> nearbyCharacters = new List<Character>();

        public ModPlayer() : base()
        {
            base.ModuleName = "Player";
            base.Loading();
        }

        public void Start()
        {
            
            selectedPlayer = Player.m_localPlayer;
            BeginMenu();
            base.Ready(); // Must be called when the module has completed initialization. // End of Start
        }

        public void BeginMenu()//◄ ►
        {
            SkMenu GenericMenu = new SkMenu();
            //GenericMenu.AddItem("Select Player\t►", new Action(BeginPromptPlayers));
            GenericMenu.AddItem("Repair All", new Action(RepairAll), "Repair all your items");
            GenericMenu.AddItem("Heal Self", new Action(Heal), "Heal yourself");
            GenericMenu.AddItem("Tame", new Action(Tame), "Tame all nearby creatures");
            GenericMenu.AddItemToggle("Enable Teleport to Mouse", ref bTeleport, new Action(ToggleTeleport), "Press tilde (~) to teleport!");
            GenericMenu.AddItemToggle("Build Anywhere", ref SkCommandPatcher.bBuildAnywhere, new Action(ToggleAnywhere), "Remove build restrictions");
            GenericMenu.AddItemToggle("No Cost Building", ref SkCommandProcessor.noCostEnabled, new Action(ToggleNoCost), "Unlock all pieces and no cost");
            GenericMenu.AddItemToggle("Detect Nearby Enemies", ref SkCommandProcessor.bDetectEnemies, new Action(ToggleESPEnemies), "Range: 20m");
            GenericMenu.AddItemToggle("Display Coordinates", ref SkCommandProcessor.bCoords, new Action(ToggleCoords), "Display coords in top left corner");
            GenericMenu.AddItemToggle("Godmode", ref SkCommandProcessor.godEnabled, new Action(ToggleGodmode), "Godmode for yourself");
            GenericMenu.AddItemToggle("Flying", ref SkCommandProcessor.flyEnabled, new Action(ToggleFlying), "Flying for yourself");
            GenericMenu.AddItemToggle("Infinite Stamina", ref SkCommandProcessor.infStamina, new Action(ToggleInfStam), "Infinite stamina for yourself");
            GenericMenu.AddItem("Give Item\t\t►", new Action(BeginListItems), "Give item(s) to self");
            MenuOptions = GenericMenu;
        }

        //public void SelectPlayer(string ln = "")
        //{
        //    foreach (Player pl in Player.GetAllPlayers())
        //    {
        //        if (pl.GetPlayerName().Equals(ln))
        //        {
        //            selectedPlayer = pl;
        //        }
        //    }
        //    if (selectedPlayer != null)
        //    {
        //        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Player set to : " + selectedPlayer.GetPlayerName() + ".", 0, null);
        //    }
        //    else
        //    {
        //        selectedPlayer = Player.m_localPlayer;
        //        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Player not found. Setting to : " + selectedPlayer.GetPlayerName() + ".", 0, null);
        //    }
        //    base.RequestMenu();
        //}


        public void BeginListItems()
        {
            SkMenu GenericMenu = new SkMenu();

            //Get all possible items
            List<ItemDrop> list = new List<ItemDrop>();
            foreach (GameObject gameObject in ObjectDB.instance.m_items)
            {
                ItemDrop component = gameObject.GetComponent<ItemDrop>();
                list.Add(component);
            }

            GenericMenu.AddItem("Quantity\t►", new Action(BeginPromptQuantity), "Quantity: " + spawnQuantity);

            foreach (ItemDrop item in list)
            {
                GenericMenu.AddItem(item.name, new Action<string>(GiveItem));
            }
            base.RequestMenu(GenericMenu);
        }

        public void BeginPromptQuantity()
        {
            SkMenu GenericMenu = new SkMenu();
            GenericMenu.AddItem("1", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("2", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("5", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("10", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("25", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("50", new Action<string>(SetSpawnQuantity));
            GenericMenu.AddItem("100", new Action<string>(SetSpawnQuantity));
            base.RequestMenu(GenericMenu);
        }

        public void SetSpawnQuantity(string ln = "")
        {
            int tempQty = 1;
            int.TryParse(ln, out tempQty);
            spawnQuantity = tempQty;
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Quantity set: " + spawnQuantity, 0, null);
        }

        public void GiveItem(string ln = "")
        {
            if (selectedPlayer != null)
            {
                GameObject item = ZNetScene.instance.GetPrefab(ln);
                if (item != null)
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + ln, 0, null);
                    for (int x = 0; x < spawnQuantity; x++)
                    {
                        try
                        {
                            Character component2 = UnityEngine.Object.Instantiate<GameObject>(item, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
                            component2.SetLevel(1);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                else
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Object " + ln + " not found.", 0, null);
                }

            }
            else
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Player not found. Set to local player.", 0, null);
                selectedPlayer = Player.m_localPlayer;
                GiveItem(ln);
            }
            BeginMenu();
        }

        public void ToggleTeleport()
        {
            bTeleport = !bTeleport;
            if (bTeleport)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Teleport enabled. Press tilde (~)!", 0, null);
            }
            else
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Teleport disabled.", 0, null);
            }
            BeginMenu();
        }

        public void Heal()
        {
            SkCommandProcessor.ProcessCommand("/heal", SkCommandProcessor.LogTo.Chat);
        }
        public void Tame()
        {
            SkCommandProcessor.ProcessCommand("/tame", SkCommandProcessor.LogTo.Chat);
        }

        public void ToggleNoCost()
        {
            SkCommandProcessor.ProcessCommand("/nocost", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleESPEnemies()
        {
            SkCommandProcessor.ProcessCommand("/detect 20", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleCoords()
        {
            SkCommandProcessor.ProcessCommand("/coords", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleAnywhere()
        {
            SkCommandProcessor.ProcessCommand("/nores", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleGodmode()
        {
            SkCommandProcessor.ProcessCommand("/god", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleFlying()
        {
            SkCommandProcessor.ProcessCommand("/fly", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void ToggleInfStam()
        {
            SkCommandProcessor.ProcessCommand("/infstam", SkCommandProcessor.LogTo.Chat);
            BeginMenu();
        }

        public void RepairAll()
        {
            SkCommandProcessor.ProcessCommand("/repair", SkCommandProcessor.LogTo.Chat);
        }

        void Update()
        {
            if (bTeleport)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(rayCast, out hit))
                    {
                        Vector3 targetLoc = hit.point;
                        Debug.DrawRay(Player.m_localPlayer.transform.position, targetLoc, Color.white);
                        Player.m_localPlayer.transform.position = targetLoc;
                        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Warp!", 0, null);
                    }
                }
            }
            //if (Input.GetKeyDown(KeyCode.KeypadMinus))
            //{
            //    Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;
            //    if (Physics.Raycast(rayCast, out hit))
            //    {
            //        Destroy(hit.collider.gameObject);
            //        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Removed!", 0, null);
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.KeypadDivide))
            //{
            //    Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;
            //    if (Physics.Raycast(rayCast, out hit))
            //    {
            //        CommandProcessor.PrintOut("GO Name: " + hit.collider.gameObject.name, false);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            //{

            //    Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;
            //    if (Physics.Raycast(rayCast, out hit))
            //    {
            //        CommandProcessor.PrintOut("GO Name: " + hit.collider.gameObject.name, false);
            //        if (!gPickPut)
            //        {
            //            gPickPut = true;
            //            CommandProcessor.PrintOut("PICK" + hit.collider.gameObject.name, false);
            //            gObject = hit.collider.gameObject;
            //            gParent = gObject.transform.parent.gameObject;
            //            gObject.transform.parent = Player.m_localPlayer.gameObject.transform;
            //        } else
            //        {
            //            gPickPut = false;
            //            CommandProcessor.PrintOut("PUT" + gObject.name, false);
            //            gObject.transform.parent = gParent.transform;
            //            gParent = null;
            //        }
                    
            //    }
            //}

            //if (bDetectEnemies)
            //{
            //    List<Character> charList = Character.GetAllCharacters();
            //    if (charList.Count > 0)
            //    {
            //        foreach (Character character in charList)
            //        {
            //            if (character != null && !character.IsDead() && !character.IsPlayer())
            //            {
            //                if (Vector3.Distance(character.transform.position, Player.m_localPlayer.transform.position) < 20f)
            //                {
            //                    if (!nearbyCharacters.Contains(character))
            //                    {
            //                        nearbyCharacters.Add(character);
            //                    }
            //                }
            //                else
            //                {
            //                    if (nearbyCharacters.Contains(character))
            //                    {
            //                        nearbyCharacters.Remove(character);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (nearbyCharacters.Contains(character))
            //                {
            //                    nearbyCharacters.Remove(character);
            //                }
            //            }
            //        }

            //        if (nearbyCharacters.Count > 0)
            //        {
            //            List<Character> tempCharList = new List<Character>(nearbyCharacters);
            //            foreach (Character character in tempCharList)
            //            {
            //                if (!charList.Contains(character))
            //                {
            //                    nearbyCharacters.Remove(character);
            //                }
            //            }
            //        }
            //    }
            //    if (nearbyCharacters.Count > 0 && btDetectEmeiesSwitch)
            //    {
            //        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Enemy nearby!", 0, null);
            //        btDetectEmeiesSwitch = false;
            //    }
            //    else if (nearbyCharacters.Count == 0)
            //    {
            //        btDetectEmeiesSwitch = true;
            //    }
            //}
        }

        void OnGUI()
        {
            //if (Player.m_localPlayer != null)
            //{
            //    if (bDetectEnemies && nearbyCharacters.Count > 0)
            //    {
            //        EnemyWindow = GUILayout.Window(39999, rectEnemy, ProcessEnemies, "Enemy Information");
            //    }
            //    if (bClock)
            //    {
            //        GUI.Label(rectClock, "Time (0-1): " + EnvMan.instance.m_debugTime);
            //    }
            //    if (bCoords)
            //    {
            //        Vector3 plPos = Player.m_localPlayer.transform.position;
            //        GUI.Label(rectCoords, "Coords: " + Mathf.RoundToInt(plPos.x) + "/" + Mathf.RoundToInt(plPos.y));
            //    }
            //}
        }

        //void ProcessEnemies(int WindowID)
        //{
        //    GUILayout.BeginVertical();
        //    if (nearbyCharacters?.Count > 0)
        //    {
        //        Vector3 playerPos = Player.m_localPlayer.transform.position;

        //        foreach (Character toon in nearbyCharacters)
        //        {
        //            float toonDist = Vector3.Distance(playerPos, toon.transform.position);

        //            if(toonDist > 15)
        //            {
        //                GUI.color = Color.green;
        //            }
        //            else if (toonDist > 10 && toonDist < 15)
        //            {
        //                GUI.color = Color.yellow;
        //            }
        //            else if (toonDist > 5 && toonDist < 10)
        //            {
        //                GUI.color = Color.yellow + Color.red;
        //            }
        //            else if (toonDist > 0 && toonDist < 5)
        //            {
        //                GUI.color = Color.red;
        //            }

        //            //GUI.color = Faction.getColor();
        //            GUILayout.BeginHorizontal();

        //            GUILayout.Label("Name: " + toon.GetHoverName());
        //            GUILayout.Label("HP: " + Mathf.RoundToInt(toon.GetHealth()) + "/" + toon.GetMaxHealth() 
        //                + " | Level: " + toon.GetLevel()
        //                + " | Dist: " + Mathf.RoundToInt(toonDist));

        //            GUILayout.EndHorizontal();
        //            GUI.color = Color.white;
        //        }
        //    }
        //    GUILayout.EndVertical();
        //    GUI.DragWindow(new Rect(0, 0, 10000, 20));
        //}

    }
}
