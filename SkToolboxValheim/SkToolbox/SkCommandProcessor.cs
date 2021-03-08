using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SkToolbox
{
    internal static class SkCommandProcessor
    {


        public static bool flyEnabled = false;
        public static bool godEnabled = false;
        public static bool farInteract = false;
        public static bool infStamina = false;
        //public static bool infStacks = false;
        public static bool noCostEnabled = false;
        public static bool bTeleport = false;
        public static bool bDebugTime = false;

        public static bool bDetectEnemies = false;
        public static bool btDetectEnemiesSwitch = true;
        public static int bDetectRange = 20;

        public static bool altOnScreenControls = false;

        public static bool bCoords = false;

        public static SkModules.ModConsoleOpt consoleOpt = null;

        public static int pageSize = 11;

        [Flags]
        public enum LogTo
        {
            Console,
            Chat,
            DebugConsole
        }

        //Clear
        //Twilight_Clear
        //Misty
        //Darklands_dark
        //Heath clear
        //DeepForest Mist
        //GDKing
        //Rain
        //LightRain
        //ThunderStorm
        //Eikthyr
        //GoblinKing
        //nofogts
        //SwampRain
        //Bonemass
        //Snow
        //Twilight_Snow
        //Twilight_SnowStorm
        //SnowStorm
        //Moder
        //Ashrain
        //Crypt
        //SunkenCrypt

        public static Dictionary<string, string> commandList = new Dictionary<string, string>()
        {
             {"/alt", "Use alternate on-screen controls. Press HOME to toggle if active."}
            ,{"/coords", "Show coords in corner of the screen"}
            ,{"/clear", "Clear the current output shown in the console"}
            ,{"/clearinventory", "Removes all items from your inventory. There is no confirmation, be careful."}
            ,{"/detect [Range=20]", "Toggle enemy detection"}
            ,{"/farinteract [Distance=50]", "Toggles far interactions (building as well). To change distance, toggle this off then back on with new distance"}
            ,{"/findtomb", "Pin nearby dead player tombstones on the map if any currently exist"}
            //,{"/findtrader", "Pin the trader on the map if he currently exists"}
            ,{"/fly", "Toggle flying"}
            ,{"/ghost", "Toggle Ghostmode"}
            ,{"/give [Item] [Qty=1], OR /give [Item] [Qty=1] [Player] [Level=1]", "Gives item to player. If player has a space in name, only provide name before the space. Capital letters matter in item / player name!"}
            ,{"/god", "Toggle Godmode"}
            ,{"/heal [Player=local]", "Heal Player"}
            ,{"/imacheater", "Use the toolbox to force enable standard cheats on any server"}
            ,{"/infstam", "Toggles infinite stamina"}
            //,{"/infstac", "Toggles infinite stacks"}
            //,{"/itp", "Make all items in your inventory able to be teleported"}
            ,{"/killall", "Kills all nearby creatures"}
            ,{"/listitems [Name Contains]", "List all items. Optionally include name starts with. \n\tEx. /listitems Woo returns any item that contains the letters 'Woo'"}
            ,{"/listskills", "Lists all skills"}
            ,{"/nocost", "Toggle no requirement building"}
            ,{"/nores", "Toggle no restrictions to where you can build (except ward zones)"}
            ,{"/nosup", "Toggle no supports required for buildings - WARNING! - IF YOU REJOIN AND THIS IS DISABLED, YOUR STRUCTURES MAY FALL APART - USE WITH CARE. Maybe use the AutoRun functionality?"}
            ,{"/portals", "List all portal tags"}
            ,{"/removedrops", "Removes items from the ground"}
            ,{"/resetwind", "If wind has been set, this will allow the game to take control of the wind again" }
            ,{"/repair", "Repair your inventory"}
            ,{"/resetmap", "Reset the map exploration"}
            ,{"/revealmap", "Reveals the entire minimap"}
            ,{"/q", "Quickly exit the game. Commands are sometimes just more convenient."}
            ,{"/seed", "Reveals the map seed"}
            ,{"/set cw [Weight]", "Set your weight limit (default 300)"}
            ,{"/set difficulty [Player Count]", "Set the difficulty (default is number of connected players)"}
            ,{"/set exploreradius [Radius=100]", "Set the explore radius"}
            ,{"/set jumpforce [Force]", "Set jump force (default 10). Careful if you fall too far!"}
            ,{"/set pickup [Radius]", "Set your auto pickup radius (default 2)"}
            ,{"/set skill [Skill] [Level]", "Set your skill level"}
            ,{"/set speed [Speed Type] [Speed]", "Speed Types: crouch (def: 2), run (def: 120), swim (def: 2)"}
            ,{"/td [Radius=5] [Height=1]", "Dig nearby terrain. Radius 30 max."}
            ,{"/tl [Radius=5]", "Level nearby terrain. Radius 30 max."}
            ,{"/tr [Radius=5] [Height=1]", "Raise nearby terrain. Radius 30 max."}
            ,{"/tu [Radius=5]", "Undo terrain modifications around you. Radius 50 max."}
            ,{"/spawn [Creature Name] [Level=1]", "Spawns a creature or prefab in front of you. Capitals in name matter! Ex. /spawn Boar 3 (use /give for items!)"}
            ,{"/stopevent", "Stops a current event"}
            ,{"/tame", "Tame all nearby creatures"}
            ,{"/tod [0-1]", "Set (and lock) time of day (-1 to unlock time) - Ex. /tod 0.5"}
            ,{"/tp [X,Y]", "Teleport you to the coords provided" }
            //,{"/tp [x, y] OR /tp [TO PLAYER] [FROM PLAYER=SELF]", "Teleport you to the coords or the target player to target other player If player has a space in name, only use first portion of the name.. "
            //                                                        + "\nEx. /tp 60,40 | /tp Skrip (Teleport to Skrip) | /tp TSkrip FSkrip (Teleport player FSkrip to player TSkrip)"}
            //,{"/ttp", "Teleport to tombstone"}
            ,{"/wind [Angle] [Intensity]", "Set the wind direction and intensity"}
            ,{"/whois", "List all players"}
        };

        public static void Announce()
        {
            PrintOut("Toolbox by Skrip (DS) is enabled. Custom commands are working.", LogTo.Console);

            if(!SkVersionChecker.VersionCurrent())
            {
                PrintOut("New Version Available on NexusMods! Current: " + SkVersionChecker.currentVersion + " Latest: " + SkVersionChecker.latestVersion, LogTo.Console | LogTo.DebugConsole);
            }

            PrintOut("====  Press numpad 0 to open on-screen menu or type /? 1  ====", LogTo.Console);
            try
            {
                commandList = commandList.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value); // Try to sort the commands in case I gave up with it eventually, lol.
                SkCommandPatcher.InitPatch();
            }
            catch (Exception)
            {

            }
        }

        public static void ProcessCommands(string inCommand, LogTo source, GameObject go = null)
        {
            if (inCommand.Length > 0)
            {
                string[] inCommandSplt = inCommand.Split(';');
                foreach (string command in inCommandSplt)
                {
                    string commandTrimmed = command.Trim();
                    if (commandTrimmed.StartsWith(" "))
                    {
                        commandTrimmed = commandTrimmed.Remove(0, 1);
                    }
                    //PrintOut("~'" + commandTrimmed + "'", true);
                    if (!ProcessCommand(commandTrimmed, source, go)) // Process SkToolbox Command
                    { // Unless the command wasn't found, then push it to the console to try and run it elsewhere
                        if (inCommandSplt.Length > 1)
                        {
                            Console.instance.m_input.text = commandTrimmed;
                        }
                        Console.instance.GetType().GetMethod("InputText", SkUtilities.BindFlags).Invoke(Console.instance, null);
                        Console.instance.m_input.text = string.Empty;

                    }
                }
            }
        }


        public static bool ProcessCommand(string inCommand, LogTo source, GameObject go = null) // source = true is console out, false is chat
        {
            if (source.HasFlag(LogTo.Console))
            {
                if (inCommand.StartsWith("help"))
                {
                    Console.instance.Print("imacheater - Enable in-game cheats");
                    Console.instance.Print("/? [Page] - SkToolbox Commands - Ex /? 1");
                    return false;
                }
            }

            if (inCommand.StartsWith("/?") || inCommand.StartsWith("/help"))
            {
                inCommand = inCommand.Remove(0, 2);

                if (inCommand.StartsWith(" "))
                {
                    inCommand = inCommand.Remove(0, 1);
                }

                int displayPage = 1;
                if (inCommand.Length > 0 && int.TryParse(inCommand, out displayPage))
                {
                    if (displayPage > (Mathf.Ceil(commandList.Count / pageSize) + (commandList.Count % pageSize == 0 ? 0 : 1)))
                    {
                        displayPage = Mathf.RoundToInt(Mathf.Ceil(commandList.Count / pageSize) + (commandList.Count % pageSize == 0 ? 0 : 1));
                    }
                    List<string> commands = new List<string>(commandList.Keys);
                    List<string> descriptions = new List<string>(commandList.Values);
                    PrintOut("Command List Page " + displayPage + " / " + (Mathf.Ceil(commandList.Count / pageSize) + (commandList.Count % pageSize == 0 ? 0 : 1)), source);
                    for (int x = ((pageSize * displayPage) - pageSize); // This will iterate over all items by page. The ternary on next line allows final page to have correct number of elements
                            x < ((commandList.Count > ((pageSize * (displayPage + 1)) - pageSize)) ? ((pageSize * (displayPage + 1)) - pageSize) : commandList.Count);
                            // Example: Is 35 items > (( 10 * (2 + 1)) - 10)? If it is, then there is another page after this one, and we can select a full page worth of items. Otherwise, use the final menu item as the end so we over get an index exception.
                            x++) // This selects the correct menu items to display for this page number
                    {
                        PrintOut(commands[x] + " - " + descriptions[x], source);
                    }

                }
                else
                {
                    PrintOut("Type /? # to see the help for that page number. Ex. /? 1", source, false);
                }

                return true;
            }


            if (commandList.ContainsKey(inCommand) && Player.m_localPlayer == null && !inCommand.StartsWith("/q") && !inCommand.StartsWith("/clear"))
            {
                PrintOut("You must be in-game to run commands!", source, false);
                return true;
            }


            if (inCommand.StartsWith("/repair"))
            {
                List<ItemDrop.ItemData> itemList = new List<ItemDrop.ItemData>();
                Player.m_localPlayer.GetInventory().GetWornItems(itemList);
                foreach (ItemDrop.ItemData itemData in itemList)
                {
                    try
                    {
                        itemData.m_durability = itemData.GetMaxDurability();
                    }
                    catch (Exception)
                    {

                    }
                }
                PrintOut("All items repaired!", source);
                return true;
            }

            if (inCommand.StartsWith("/portals"))
            {
                PrintOut(ListPortals(), source, true);
                return true;
            }

            if (inCommand.StartsWith("/tl"))
            {
                GameObject tLevel = ZNetScene.instance.GetPrefab("digg_v2");
                if (tLevel == null)
                {
                    PrintOut("Terrain level failed. Report to mod author - terrain level error 1", source);
                    return true;
                }
                string[] command = inCommand.Split(' ');
                float radius = 5f;
                if (command.Length > 1)
                {
                    try
                    {
                        radius = int.Parse(command[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                TerrainModification.ModifyTerrain(0, Player.m_localPlayer.transform.position, tLevel, radius);

                PrintOut("Terrain levelled!", source);
                return true;
            }

            if (inCommand.StartsWith("/tu"))
            {
                string[] command = inCommand.Split(' ');
                float radius = 5f;
                if (command.Length > 1)
                {
                    try
                    {
                        radius = int.Parse(command[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                ResetTerrain(Player.m_localPlayer.transform.position, radius);

                PrintOut("Terrain reset!", source);
                return true;
            }

            if (inCommand.StartsWith("/tr"))
            {
                GameObject tLevel = ZNetScene.instance.GetPrefab("raise");
                if (tLevel == null)
                {
                    PrintOut("Terrain raise failed. Report to mod author - terrain raise error 1", source);
                    return true;
                }
                string[] command = inCommand.Split(' ');
                float radius = 5f;
                float height = 2f;
                if (command.Length > 1)
                {
                    try
                    {
                        radius = int.Parse(command[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (command.Length > 2)
                {
                    try
                    {
                        height = int.Parse(command[2]);
                    }
                    catch (Exception)
                    {
                    }
                }
                TerrainModification.ModifyTerrain(1, Player.m_localPlayer.transform.position + (Vector3.up * height), tLevel, radius);

                PrintOut("Terrain raised!", source);
                return true;
            }

            if (inCommand.StartsWith("/td"))
            {
                GameObject tLevel = ZNetScene.instance.GetPrefab("digg_v2");
                if (tLevel == null)
                {
                    PrintOut("Terrain dig failed. Report to mod author - terrain dig error 1", source);
                    return true;
                }
                string[] command = inCommand.Split(' ');
                float radius = 5f;
                float height = 1f;
                if (command.Length > 1)
                {
                    try
                    {
                        radius = int.Parse(command[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (command.Length > 2)
                {
                    try
                    {
                        height = int.Parse(command[2]);
                    }
                    catch (Exception)
                    {
                    }
                }
                TerrainModification.ModifyTerrain(-1, Player.m_localPlayer.transform.position - (Vector3.up * height), tLevel, radius);

                PrintOut("Terrain dug!", source);
                return true;
            }

            if (inCommand.StartsWith("/resetwind"))
            {
                EnvMan.instance.ResetDebugWind();
            }

            if (inCommand.StartsWith("/wind"))
            {
                string[] inCommandSpl = inCommand.Split(' ');
                if (inCommandSpl.Length == 3)
                {
                    float angle = float.Parse(inCommandSpl[1]);
                    float intensity = float.Parse(inCommandSpl[2]);
                    EnvMan.instance.SetDebugWind(angle, intensity);
                }
                else
                {
                    PrintOut("Failed to set wind. Check parameters!", source);
                }

            }

            //if (inCommand.StartsWith("/imaxstacks"))
            //{
            //    PrintOut("Command removed until it is working. Apologies for any inconvenience.", source);
            //    //List<ItemDrop.ItemData> itemList = new List<ItemDrop.ItemData>();
            //    //Player.m_localPlayer.GetInventory().GetAllItems();
            //    //foreach (ItemDrop.ItemData itemData in itemList)
            //    //{
            //    //    try
            //    //    {
            //    //        Player.m_localPlayer.GetInventory().GetItemAt(1, 1).m_stack = 99;
            //    //        itemData.m_stack = itemData.m_shared.m_maxStackSize;
            //    //    }
            //    //    catch (Exception)
            //    //    {

            //    //    }
            //    //}
            //    //PrintOut("Stacks maxed!", source);
            //    //return true;
            //}

            //if (inCommand.StartsWith("/itp"))
            //{
            //    PrintOut("Command removed until it is working. Apologies for any inconvenience.", source);
            //    return true;

            //    //List <ItemDrop.ItemData> itemList = new List<ItemDrop.ItemData>();
            //    //Player.m_localPlayer.GetInventory().GetAllItems();
            //    //foreach (ItemDrop.ItemData itemData in itemList)
            //    //{
            //    //    try
            //    //    {
            //    //        itemData.m_shared.m_teleportable = true;
            //    //    }
            //    //    catch (Exception)
            //    //    {

            //    //    }
            //    //}
            //    //PrintOut("All items can now be teleported!", source);
            //    //return true;
            //}

            if (inCommand.StartsWith("/fly"))
            {
                flyEnabled = !flyEnabled;
                Player.m_debugMode = flyEnabled;
                SkUtilities.SetPrivateField(Player.m_localPlayer, "m_debugFly", flyEnabled);
                PrintOut("Fly toggled! (" + flyEnabled.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/alt"))
            {
                altOnScreenControls = !altOnScreenControls;
                PrintOut("Alt controls toggled! (" + altOnScreenControls.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/stopevent"))
            {
                RandEventSystem.instance.ResetRandomEvent();
                PrintOut("Event stopped!", source);
                return true;
            }

            if (inCommand.StartsWith("/revealmap"))
            {
                Minimap.instance.ExploreAll();
                PrintOut("Map revealed!", source);
            }

            if (inCommand.StartsWith("/whois"))
            {
                string playerStr = string.Empty;
                //foreach (Player pl in Player.GetAllPlayers())
                foreach (ZNetPeer pl in ZNet.instance.GetConnectedPeers())
                {
                    if (pl != null)
                    {
                        playerStr = playerStr + ", " + pl.m_playerName + "(" + pl.m_uid + ")";
                    }
                }
                if (playerStr.Length > 2)
                {
                    playerStr = playerStr.Remove(0, 2);
                }
                PrintOut("Active Players (" + Player.GetAllPlayers().Count + ") - " + playerStr, source, true);
            }

            if (inCommand.StartsWith("/give"))
            {
                inCommand = inCommand.Remove(0, 6);
                string[] cmdSplt = inCommand.Split(' ');
                string cmdPlr = Player.m_localPlayer.GetPlayerName();
                string cmdItem = string.Empty;
                int cmdQty = 1;
                int cmdLvl = 1;
                if (cmdSplt.Length == 0)
                {
                    PrintOut("Failed. No item provided. /give [Item] [Qty=1] [Player] [Level=1]", source);
                }
                if (cmdSplt.Length >= 1) // if they provided an item
                {
                    cmdItem = cmdSplt[0];
                }
                if (cmdSplt.Length == 2) // if they provided a player name
                {
                    try
                    {
                        cmdQty = int.Parse(cmdSplt[1]);
                    }
                    catch (Exception)
                    {

                    }

                }
                if (cmdSplt.Length > 2) // if they provided a player name
                {
                    try
                    {
                        cmdQty = int.Parse(cmdSplt[1]);
                    }
                    catch (Exception)
                    {
                        cmdQty = 1;
                    }

                }
                if (cmdSplt.Length >= 3) // if they provided a quantity
                {
                    try
                    {
                        cmdPlr = cmdSplt[2];

                    }
                    catch (Exception)
                    {

                    }
                }
                if (cmdSplt.Length >= 4) // if they provided a level
                {
                    try
                    {
                        cmdLvl = int.Parse(cmdSplt[3]);
                    }
                    catch (Exception)
                    {
                        cmdLvl = 1;
                    }
                }

                bool itemExists = false; // verify the item exists

                foreach (GameObject itm in ObjectDB.instance.m_items)
                {
                    ItemDrop component = itm.GetComponent<ItemDrop>();
                    if (component.name.StartsWith(cmdSplt[0]))
                    {
                        itemExists = true;
                        break;
                    }
                }

                if (!itemExists)
                {
                    PrintOut("Failed. Item does not exist. /give [Item] [Qty=1] [Player] [Level=1]. Check for items with /listitems. Capital letters matter on this command!", source);
                    return true;
                }

                Player plrObj = null;
                foreach (Player pl in Player.GetAllPlayers())
                {
                    if (pl != null)
                    {
                        if (pl.GetPlayerName().ToLower().StartsWith(cmdPlr.ToLower()))
                        {
                            plrObj = pl;
                            break;
                        }
                    }
                }

                if (plrObj == null)
                {
                    PrintOut("Failed. Player does not exist. /give [Item] [Qty=1] [Player] [Level=1]", source);
                    return true;
                }

                GameObject item = ZNetScene.instance.GetPrefab(cmdSplt[0]);
                if (item)
                {
                    PrintOut("Spawning " + cmdQty + " of item " + cmdSplt[0] + "(" + cmdLvl + ") on " + cmdPlr, source, true);

                    //for (int x = 0; x < cmdQty; x++)
                    //{
                    try
                    {
                        GameObject createdObj = UnityEngine.Object.Instantiate<GameObject>(item, plrObj.transform.position + plrObj.transform.forward * 1.5f + Vector3.up, Quaternion.identity);
                        ItemDrop itemObj = (ItemDrop)createdObj.GetComponent(typeof(ItemDrop));
                        if (itemObj != null && itemObj.m_itemData != null)
                        {
                            itemObj.m_itemData.m_quality = cmdLvl;
                            itemObj.m_itemData.m_stack = cmdQty;
                            itemObj.m_itemData.m_durability = itemObj.m_itemData.GetMaxDurability();
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintOut("Something unexpected failed.", source);
                        SkUtilities.Logz(new string[] { "ERR", "/give" }, new string[] { ex.Message, ex.Source }, LogType.Warning);
                    }
                    //}
                }
                else
                {
                    PrintOut("Failed. Check parameters. /give [Item] [Qty=1] [Player] [Level=1]", source);
                }
                return true;
            }

            if (inCommand.StartsWith("/god"))
            {
                godEnabled = !godEnabled;
                Player.m_localPlayer.SetGodMode(godEnabled);
                PrintOut("God toggled! (" + godEnabled.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/clearinventory"))
            {
                Player.m_localPlayer.GetInventory().RemoveAll();
                PrintOut("All items removed from inventory.", source);
                return true;
            }

            if (inCommand.StartsWith("/findtomb"))
            {
                TombStone[] listTraders = GameObject.FindObjectsOfType<TombStone>();
                if (listTraders.Length > 0)
                {
                    foreach (TombStone tr in listTraders)
                    {
                        if (tr != null && tr.enabled)
                        {
                            Minimap.instance.AddPin(tr.transform.position, Minimap.PinType.Ping, "TS", true, true);
                        }
                    }
                }
                PrintOut("Tombstone sought out! Potentially " + listTraders.Length + " found.", source);
                return true;
            }

            if (inCommand.StartsWith("/seed"))
            {
                World wrld = SkUtilities.GetPrivateField<World>(WorldGenerator.instance, "m_world");
                PrintOut("Map seed: " + wrld.m_seedName, source, true);
                return true;
            }

            if (inCommand.StartsWith("/heal"))
            {
                inCommand = inCommand.Remove(0, 5);
                if (inCommand.Length > 0)
                {
                    inCommand = inCommand.Remove(0, 1);
                    foreach (Player pl in Player.GetAllPlayers())
                    {
                        if (pl != null)
                        {
                            if (pl.GetPlayerName().Equals(inCommand))
                            {
                                pl.Heal(pl.GetMaxHealth());
                                PrintOut("Player healed: " + pl.GetPlayerName(), source, true);
                            }
                        }
                    }
                }
                else
                {
                    Player.m_localPlayer.Heal(Player.m_localPlayer.GetMaxHealth(), true);
                    PrintOut("Self healed.", source, true);
                }

                return true;
            }

            if (inCommand.StartsWith("/nores"))
            {
                SkCommandPatcher.InitPatch();
                SkCommandPatcher.bBuildAnywhere = !SkCommandPatcher.bBuildAnywhere;
                PrintOut("No build restrictions toggled! (" + SkCommandPatcher.bBuildAnywhere.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/nocost"))
            {
                noCostEnabled = !noCostEnabled;
                Player.m_debugMode = noCostEnabled;
                SkUtilities.SetPrivateField(Player.m_localPlayer, "m_noPlacementCost", noCostEnabled);
                PrintOut("No build cost/requirements toggled! (" + noCostEnabled.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/dump"))
            {
                PrintOut(SkUtilities.GetPrivateField<List<Minimap.PinData>>(Minimap.instance, "m_playerPins").Count.ToString(), source);
                return true;
            }

            if (inCommand.StartsWith("/tp"))
            {
                //if (inCommand.Contains(","))
                //{
                inCommand = inCommand.Remove(0, 3);
                inCommand = inCommand.Replace(" ", string.Empty);
                string[] loc = inCommand.Split(',');
                if (loc.Length > 2 || loc.Length < 2 || !inCommand.Contains(","))
                {
                    PrintOut("Syntax /tp X,Z", source);
                    return true;
                }
                try
                {
                    float x = float.Parse(loc[0]);
                    float z = float.Parse(loc[1]);
                    float y = ZoneSystem.instance.GetGroundHeight(new Vector3(x, 750, z));
                    y = Mathf.Clamp(y, 0, 100);
                    if (y > 99)
                    {
                        y = Player.m_localPlayer.transform.position.y;
                    }
                    Player localPlayer2 = Player.m_localPlayer;
                    if (localPlayer2)
                    {
                        Vector3 pos2 = new Vector3(x, y, z);
                        localPlayer2.TeleportTo(pos2, localPlayer2.transform.rotation, false);
                        PrintOut("Teleporting...", source, true);
                    }
                }
                catch (Exception)
                {
                    PrintOut("Syntax /tp X,Z", source);
                }

                //}
                //else
                //{
                //    List<ZDO> playerList = new List<ZDO>();
                //    if (ZNet.instance != null)
                //    {
                //        playerList = ZNet.instance.GetAllCharacterZDOS();
                //    }

                //    if (playerList == null)
                //    {
                //        PrintOut("Could not access player list for some reason?", source);
                //        return true;
                //    }
                //    if (playerList != null)
                //    {
                //        if (playerList.Count > 0)
                //        {
                //            foreach (var pl in playerList) // Debug
                //            {
                //                PrintOut("!!" + pl. + " - " + pl.transform.position.ToString(), source, true);
                //            }
                //            inCommand = inCommand.Remove(0, 4);
                //            string[] inCommandSplit = inCommand.Split(' ');
                //            if (inCommandSplit.Length > 0)
                //            {
                //                Player pl1 = null;
                //                Player pl2 = null;
                //                if (inCommandSplit.Length == 1)
                //                {
                //                    pl2 = Player.m_localPlayer;
                //                }

                //                foreach (var pl in playerList)
                //                {
                //                    if (pl != null)
                //                    {
                //                        if (pl.GetPlayerName().StartsWith(inCommandSplit[0]))
                //                        {
                //                            PrintOut("P1 = " + pl.GetPlayerName(), source, true);
                //                            pl1 = pl;
                //                        }
                //                        if (inCommandSplit.Length > 1)
                //                        {
                //                            if (pl.GetPlayerName().StartsWith(inCommandSplit[1]))
                //                            {
                //                                PrintOut("P2 = " + pl.GetPlayerName(), source, true);
                //                                pl2 = pl;
                //                            }
                //                        }
                //                        if (pl1 != null && pl2 != null)
                //                        {
                //                            break;
                //                        }
                //                    }
                //                }

                //                if (pl1 != null && pl2 != null)
                //                {
                //                    pl2.TeleportTo(pl1.transform.position, pl1.transform.rotation, false);
                //                    PrintOut("Teleporting...", source, true);
                //                }
                //                else
                //                {
                //                    PrintOut("Player not found. P1:'" + pl1.GetPlayerName() + "' P2:'" + pl2.GetPlayerName(), source, true);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            PrintOut("No other connected players found.", source, true);
                //        }
                //    }
                //    else
                //    {
                //        PrintOut("No other connected players found.", source, true);
                //    }
                //}
                return true;
            }

            if (inCommand.StartsWith("/detect"))
            {
                bDetectEnemies = !bDetectEnemies;
                inCommand = inCommand.Remove(0, 8);
                if (inCommand.Length > 0)
                {
                    try
                    {
                        bDetectRange = int.Parse(inCommand);
                    }
                    catch (Exception)
                    {
                        bDetectRange = 20;
                    }
                }
                PrintOut("Detect enemies toggled! (" + bDetectEnemies.ToString() + ", range: " + bDetectRange + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/imacheater"))
            {
                SkCommandPatcher.InitPatch();
                SkCommandPatcher.bCheat = !SkCommandPatcher.bCheat;
                PrintOut("Cheats toggled! (" + SkCommandPatcher.bCheat.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/nosup"))
            {
                SkCommandPatcher.InitPatch();
                SkCommandPatcher.bFreeSupport = !SkCommandPatcher.bFreeSupport;
                PrintOut("No build support requirements toggled! (" + SkCommandPatcher.bFreeSupport.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/coords"))
            {
                bCoords = !bCoords;
                PrintOut("Show coords toggled! (" + bCoords.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/resetmap"))
            {
                Minimap.instance.Reset();
                return true;
            }

            if (inCommand.StartsWith("/infstam"))
            {
                infStamina = !infStamina;
                if (infStamina)
                {
                    Player.m_localPlayer.m_staminaRegenDelay = 0.1f;
                    Player.m_localPlayer.m_staminaRegen = 99f;
                    Player.m_localPlayer.m_runStaminaDrain = 0f;
                    Player.m_localPlayer.SetMaxStamina(999f, true);
                }
                else
                {
                    Player.m_localPlayer.m_staminaRegenDelay = 1f;
                    Player.m_localPlayer.m_staminaRegen = 5f;
                    Player.m_localPlayer.m_runStaminaDrain = 10f;
                    Player.m_localPlayer.SetMaxStamina(100f, true);
                }
                PrintOut("Infinite stamina toggled! (" + infStamina.ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/tame"))
            {
                Tameable.TameAllInArea(Player.m_localPlayer.transform.position, 20f);
                PrintOut("Creatures tamed!", source);
                return true;
            }

            if (inCommand.StartsWith("/farinteract"))
            {
                farInteract = !farInteract;
                if (farInteract)
                {
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length > 1)
                    {
                        try
                        {
                            Player.m_localPlayer.m_maxInteractDistance = float.Parse(commandSpl[1]);
                            Player.m_localPlayer.m_maxPlaceDistance = float.Parse(commandSpl[1]);
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set far interaction distance. Check params. /farinteract 50", source);
                        }
                    }
                    else
                    {
                        Player.m_localPlayer.m_maxInteractDistance = 50f;
                        Player.m_localPlayer.m_maxPlaceDistance = 50f;
                    }
                    PrintOut("Far interactions toggled! (" + farInteract.ToString() + " Distance: " + Player.m_localPlayer.m_maxInteractDistance + ")", source);
                }
                else
                {
                    Player.m_localPlayer.m_maxInteractDistance = 5f;
                    Player.m_localPlayer.m_maxPlaceDistance = 5f;
                    PrintOut("Far interactions toggled! (" + farInteract.ToString() + ")", source);
                }

                return true;
            }

            if (inCommand.StartsWith("/ghost"))
            {
                Player.m_localPlayer.SetGhostMode(!Player.m_localPlayer.InGhostMode());
                PrintOut("Ghost mode toggled! (" + Player.m_localPlayer.InGhostMode().ToString() + ")", source);
                return true;
            }

            if (inCommand.StartsWith("/tod"))
            {

                inCommand = inCommand.Remove(0, 5);
                float num10;
                if (!float.TryParse(inCommand, NumberStyles.Float, CultureInfo.InvariantCulture, out num10))
                {
                    return true;
                }
                if (num10 < 0f)
                {
                    EnvMan.instance.m_debugTimeOfDay = false;
                }
                else
                {
                    EnvMan.instance.m_debugTimeOfDay = true;
                    EnvMan.instance.m_debugTime = Mathf.Clamp01(num10);
                }
                PrintOut("Setting time of day:" + num10, source);
                return true;
            }

            if (inCommand.StartsWith("/set"))
            {
                inCommand = inCommand.Remove(0, 5);
                if (inCommand.StartsWith("cw"))
                {
                    inCommand = inCommand.Remove(0, 3);
                    try
                    {
                        int newWeight = int.Parse(inCommand);
                        Player.m_localPlayer.m_maxCarryWeight = newWeight;
                        PrintOut("New carry weight set to: " + newWeight, source);
                    }
                    catch (Exception)
                    {
                        PrintOut("Failed to set new carry weight. Check params.", source);
                    }
                    return true;
                }

                if (inCommand.StartsWith("skill"))
                {
                    inCommand = inCommand.Remove(0, 6);
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length == 2 && !commandSpl[0].Contains("None") && !commandSpl[0].Contains("All") && !commandSpl[0].Contains("FireMagic") && !commandSpl[0].Contains("FrostMagic"))
                    {
                        try
                        {
                            string cmdSkill = commandSpl[0];
                            int cmdLvl = int.Parse(commandSpl[1]);
                            Player.m_localPlayer.GetSkills().CheatResetSkill(cmdSkill.ToLower());
                            Player.m_localPlayer.GetSkills().CheatRaiseSkill(cmdSkill.ToLower(), (float)cmdLvl);
                            return true;
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set skill. Check params / skill name. See /listskills. /set skill [skill] [level]", source);
                            return true;
                        }
                    }
                    PrintOut("Failed to set skill. Check params / skill name. See /listskills.  /set skill [skill] [level]", source);
                    return true;
                }

                if (inCommand.StartsWith("pickup"))
                {
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length == 2)
                    {
                        try
                        {
                            int cmdRange = int.Parse(commandSpl[1]);
                            Player.m_localPlayer.m_autoPickupRange = cmdRange;
                            PrintOut("New range set to: " + cmdRange, source);
                            return true;
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set pickup range. Check params. /set pickup 2", source);
                            return true;
                        }
                    }

                    PrintOut("Failed to set pickup range. Check params.  /set pickup 2", source);
                    return true;
                }

                if (inCommand.StartsWith("jumpforce"))
                {
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length == 2)
                    {
                        try
                        {
                            int cmdRange = int.Parse(commandSpl[1]);
                            Player.m_localPlayer.m_jumpForce = cmdRange;
                            PrintOut("New range set to: " + cmdRange, source);
                            return true;
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set jump force. Check params. /set jumpforce 10", source);
                            return true;
                        }
                    }

                    PrintOut("Failed to set jump force. Check params.  /set jumpforce 10", source);
                    return true;
                }

                if (inCommand.StartsWith("exploreradius"))
                {
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length == 2)
                    {
                        try
                        {
                            int cmdRange = int.Parse(commandSpl[1]);
                            Minimap.instance.m_exploreRadius = cmdRange;
                            PrintOut("New range set to: " + cmdRange, source);
                            return true;
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set explore radius. Check params. /set exploreradius 100", source);
                            return true;
                        }
                    }

                    PrintOut("Failed to set explore radius. Check params.  /set exploreradius 100", source);
                    return true;
                }

                if (inCommand.StartsWith("speed"))
                {
                    string[] commandSpl = inCommand.Split(' ');
                    int cmdSpeed;
                    if (commandSpl.Length == 3)
                    {
                        string cmdType = commandSpl[1];
                        String[] types = { "crouch", "run", "swim" };
                        if (types.Contains(cmdType))
                        {
                            try
                            {

                                cmdSpeed = int.Parse(commandSpl[2]);
                                switch (cmdType)
                                {
                                    case "crouch":
                                        Player.m_localPlayer.m_crouchSpeed = cmdSpeed;
                                        break;
                                    case "run":
                                        Player.m_localPlayer.m_runSpeed = cmdSpeed;
                                        break;
                                    case "swim":
                                        Player.m_localPlayer.m_swimSpeed = cmdSpeed;
                                        break;
                                }
                                PrintOut("New " + cmdType + " speed set to: " + cmdSpeed, source);
                            }
                            catch (Exception)
                            {
                                PrintOut("Failed to set speed. Check params name. Ex. /set speed crouch 2", source);
                                return true;
                            }
                        }
                        else
                        {
                            PrintOut("Failed to set speed. Check params name. Ex.  /set speed crouch 2", source);
                        }
                        return true;
                    }
                    PrintOut("Failed to set speed. Check params name. Ex.  /set speed crouch 2", source);
                    return true;
                }

                if (inCommand.StartsWith("difficulty"))
                {
                    string[] commandSpl = inCommand.Split(' ');
                    if (commandSpl.Length == 2)
                    {
                        try
                        {
                            int diffLvl = int.Parse(commandSpl[1]);
                            Game.instance.SetForcePlayerDifficulty(diffLvl);
                            PrintOut("Difficulty set to " + diffLvl.ToString(), source);
                            return true;
                        }
                        catch (Exception)
                        {
                            PrintOut("Failed to set difficulty. Check params. /set difficulty 5", source);
                            return true;
                        }
                    }
                    PrintOut("Failed to set difficulty. Check params.  /set difficulty 5", source);
                    return true;
                }

                return true;
            }

            if (inCommand.StartsWith("/removedrops"))
            {
                ItemDrop[] array2 = UnityEngine.Object.FindObjectsOfType<ItemDrop>();
                for (int i = 0; i < array2.Length; i++)
                {
                    ZNetView component = array2[i].GetComponent<ZNetView>();
                    if (component)
                    {
                        component.Destroy();
                    }
                }
                PrintOut("Items cleared.", source, true);
                return true;
            }

            if (inCommand.StartsWith("/spawn"))
            {
                ZNetView[] ZNetObject = GameObject.FindObjectsOfType<ZNetView>();
                if (ZNetObject.Length == 0)
                {
                    PrintOut("Couldn't find zdo...", source);
                }

                inCommand = inCommand.Remove(0, 7);
                string[] commandSpl = inCommand.Split(' ');
                if (commandSpl.Length > 0)
                {
                    GameObject creature = ZNetScene.instance.GetPrefab(commandSpl[0]);
                    if (creature == null)
                    {
                        PrintOut("Creature not found.", source);
                        return true;
                    }

                    Vector3 position = Player.m_localPlayer.transform.position;

                    GameObject createdCreature = UnityEngine.Object.Instantiate<GameObject>(creature, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 1.5f, Quaternion.identity);

                    ZNetView component = createdCreature.GetComponent<ZNetView>();
                    BaseAI component2 = createdCreature.GetComponent<BaseAI>();

                    if (commandSpl.Length > 1) // A level was included
                    {
                        Character creatureComponent = createdCreature.GetComponent<Character>();
                        if (creatureComponent)
                        {
                            int lvl = int.Parse(commandSpl[1]);
                            if (lvl > 10) lvl = 10;
                            creatureComponent.SetLevel(lvl);
                        }
                    }
                    if (ZNetObject.Length > 0)
                    {
                        component.GetZDO().SetPGWVersion(ZNetObject[0].GetZDO().GetPGWVersion());
                        ZNetObject[0].GetZDO().Set("spawn_id", component.GetZDO().m_uid);
                        ZNetObject[0].GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
                        //this.SpawnEffect(createdCreature);
                    }
                    PrintOut("Creature spawned - " + inCommand, source);
                }

                return true;
            }

            if (inCommand.StartsWith("/killall"))
            {
                List<Character> CharList = new List<Character>();
                Character.GetCharactersInRange(Player.m_localPlayer.transform.position, 50f, CharList);
                foreach (Character character in CharList)
                {
                    if (!character.IsPlayer())
                    {
                        HitData hitData = new HitData();
                        hitData.m_damage.m_damage = 1E+10f;
                        character.Damage(hitData);
                    }
                }
                PrintOut("Nearby creatures killed! (50m)", source);
                return true;
            }

            if (source.HasFlag(LogTo.Console)) // Run from console only
            {
                if (inCommand.StartsWith("/listitems"))
                {
                    inCommand = inCommand.Remove(0, 10);
                    if (inCommand.Length > 0)
                    { //starts with
                        if (inCommand.StartsWith(" "))
                        {
                            inCommand = inCommand.Remove(0, 1);
                        }
                        foreach (GameObject gameObject in ObjectDB.instance.m_items)
                        {
                            ItemDrop component = gameObject.GetComponent<ItemDrop>();
                            if (component.name.ToLower().Contains(inCommand.ToLower()))
                            {
                                PrintOut("Item: '" + component.name + "'", source);
                            }
                        }
                    }
                    else
                    { // return all
                        foreach (GameObject gameObject in ObjectDB.instance.m_items)
                        {
                            ItemDrop component = gameObject.GetComponent<ItemDrop>();
                            PrintOut("Item: '" + component.name + "'", source);
                        }
                    }
                    return true;
                }

                if (inCommand.StartsWith("/listskills"))
                {
                    string skillList = "Skills found: ";
                    foreach (object obj in Enum.GetValues(typeof(Skills.SkillType)))
                    {
                        if (!obj.ToString().Contains("None") && !obj.ToString().Contains("All") && !obj.ToString().Contains("FireMagic") && !obj.ToString().Contains("FrostMagic"))
                            skillList = skillList + obj.ToString() + ", ";
                    }
                    skillList = skillList.Remove(skillList.Length - 2);
                    PrintOut(skillList, source);
                    return true;
                }
            }

            if (inCommand.StartsWith("/q"))
            {
                PrintOut("Quitting game...", source);
                Application.Quit();
                return true;
            }

            if (inCommand.StartsWith("/clear"))
            {
                if (Console.instance != null)
                {
                    Console.instance.m_output.text = string.Empty;
                    try
                    {
                        SkUtilities.SetPrivateField(Console.instance, "m_chatBuffer", new List<string>());
                    }
                    catch (Exception)
                    {

                    }
                }
                return true;
            }
            return false;
        }

        public static void PrintOut(string text, LogTo source, bool playerSay = false)
        {
            if (text.Equals(string.Empty) || text.Equals(" "))
            {
                return;
            }
            if (source.HasFlag(LogTo.Console) && Console.instance != null)
            {
                Console.instance.Print("(SkToolbox) " + text);
                if (consoleOpt != null && consoleOpt.conWriteToFile)
                {
                    SkUtilities.Logz(new string[] { "DUMP", "ITEM" }, new string[] { text, });
                }

            }
            if (source.HasFlag(LogTo.Chat) && Chat.instance != null)
            {
                if (playerSay && Player.m_localPlayer != null && (SkConfigEntry.cAllowPublicChatOutput != null && SkConfigEntry.cAllowPublicChatOutput.Value))
                {
                    Player.m_localPlayer.GetComponent<Talker>().Say(Talker.Type.Normal, text);
                }
                else
                {
                    ChatPrint(text);
                }
            }
            if (source.HasFlag(LogTo.DebugConsole))
            {
                SkUtilities.Logz(new string[] { "TOOLBOX" }, new string[] { text });
            }
        }


        static Vector3 chatPos = new Vector3(0, 0 - 99);
        public static void ChatPrint(string ln, string source = "Server")
        {
            if (Chat.instance != null)
            {
                Chat.instance.OnNewChatMessage(null, 999, chatPos, Talker.Type.Normal, source, ln);
                SkUtilities.SetPrivateField(Chat.instance, "m_hideTimer", 0f);
                Chat.instance.m_chatWindow.gameObject.SetActive(true);
                Chat.instance.m_input.gameObject.SetActive(true);
                List<Chat.WorldTextInstance> worldTexts = SkUtilities.GetPrivateField<List<Chat.WorldTextInstance>>(Chat.instance, "m_worldTexts") as List<Chat.WorldTextInstance>;

                //Destroy in-world text
                foreach (Chat.WorldTextInstance worldTextInstance in worldTexts)
                {
                    if (worldTextInstance.m_talkerID == 999)
                    {
                        worldTextInstance.m_timer = 999f;
                        continue;

                    }
                }
            }
        }

        public static string ListPortals(bool printToChat = false)
        {
            List<ZDO> m_tempPortalList = new List<ZDO>();

            int index = 0;
            bool done = false;
            do
            {
                done = ZDOMan.instance.GetAllZDOsWithPrefabIterative(Game.instance.m_portalPrefab.name, m_tempPortalList, ref index);
            }
            while (!done);
            string outStr = string.Empty;
            foreach (ZDO zdo in m_tempPortalList)
            {
                ZDOID zdoid = zdo.GetZDOID("target");
                string str = zdo.GetString("tag", "");
                if (!str.Equals(string.Empty) && !str.Equals(" "))
                {
                    outStr = outStr + "'" + str + "', ";
                }
            }
            if (!outStr.Equals(string.Empty))
            {
                outStr = outStr.Substring(0, outStr.Length - 2);
                return ("Portals found: " + outStr);
            }
            else
            {
                return ("No portals found.");
            }
        }

        internal static  class TerrainModification {
            // Thank you to BlueAmulet for this code
        private static void CreateTerrain(GameObject prefab, Vector3 position, ZNetView component)
        {
            float levelOffset = prefab.GetComponent<TerrainModifier>().m_levelOffset;
            GameObject terrainObject = UnityEngine.Object.Instantiate(prefab, position - Vector3.up * levelOffset, Quaternion.identity);
            terrainObject.GetComponent<ZNetView>().GetZDO().SetPGWVersion(component.GetZDO().GetPGWVersion());
        }

        //Thank you to BlueAmulet for this code
        public static void ModifyTerrain(int operation, Vector3 centerLocation, GameObject prefab, float radius)
        {
            if (radius > 30f)
            {
                PrintOut("Radius clamped to 30 max!", LogTo.Console);
            }
            //radius = Mathf.Clamp(radius, 0f, 25f) / 2f + 0.25f;
            radius = Mathf.Clamp(radius, 0f, 30f);
            Vector3 a = centerLocation;
            ZNetView component = Player.m_localPlayer.gameObject.GetComponent<ZNetView>();
            int iSize = Mathf.CeilToInt(radius / 3) * 3;
            for (int x = -iSize; x <= iSize; x += 3)
            {
                for (int z = -iSize; z <= iSize; z += 3)
                {
                    Vector3 vector = new Vector3(a.x + x, a.y, a.z + z);
                    if (Utils.DistanceXZ(a, vector) <= radius)
                    {
                        CreateTerrain(prefab, vector, component);
                    }
                }
            }

            int lx = int.MaxValue;
            int lz = int.MaxValue;
            for (int x = -iSize; x <= 0; x++)
            {
                for (int z = -iSize; z <= -iSize / 2; z++)
                {
                    Vector3 vector = new Vector3(a.x + x, a.y, a.z + z);
                    if (Utils.DistanceXZ(a, vector) <= radius)
                    {
                        if (x >= z && (z < lz || x > lx + 2 || x == 0))
                        {
                            lx = x;
                            lz = z;
                            if (x / 3 * 3 != x || z / 3 * 3 != z)
                            {
                                CreateTerrain(prefab, new Vector3(a.x + x, a.y, a.z + z), component);
                                CreateTerrain(prefab, new Vector3(a.x + x, a.y, a.z - z), component);
                                if (x != 0)
                                {
                                    CreateTerrain(prefab, new Vector3(a.x - x, a.y, a.z + z), component);
                                    CreateTerrain(prefab, new Vector3(a.x - x, a.y, a.z - z), component);
                                }
                                if (x != z)
                                {
                                    CreateTerrain(prefab, new Vector3(a.x + z, a.y, a.z + x), component);
                                    CreateTerrain(prefab, new Vector3(a.x - z, a.y, a.z + x), component);
                                    if (x != 0)
                                    {
                                        CreateTerrain(prefab, new Vector3(a.x + z, a.y, a.z - x), component);
                                        CreateTerrain(prefab, new Vector3(a.x - z, a.y, a.z - x), component);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

        

        public static void ResetTerrain(Vector3 centerLocation, float radius)
        {
            if (radius > 50)
            {
                PrintOut("Radius clamped to 50 max!", LogTo.Console);
            }
            radius = Mathf.Clamp(radius, 2, 50);
            Vector3 centerpos = centerLocation;
            try
            {
                var tList = TerrainModifier.GetAllInstances();
                foreach (TerrainModifier terrainModifier in tList)
                {
                    if (terrainModifier != null)
                    {
                        if (Utils.DistanceXZ(Player.m_localPlayer.transform.position, terrainModifier.transform.position) < radius)
                        {
                            ZNetView component = terrainModifier.GetComponent<ZNetView>();
                            if (component != null && component.IsValid())
                            {
                                component.ClaimOwnership();
                                component.Destroy();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //
            }
            return;
        }
    }
}
