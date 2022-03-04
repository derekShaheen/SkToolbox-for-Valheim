using HarmonyLib;
using SkToolbox.Utility;
using System;

namespace SkToolbox
{
    internal static class SkCommandPatcher
    {
        private static Harmony harmony = null;

        private static bool initComplete = false;

        private static bool bCheat = false;
        private static bool bFreeSupport = false;
        public static bool bBuildAnywhere = false;

        public static Harmony Harmony { get => harmony; set => harmony = value; }
        public static bool BCheat { get => bCheat; set => bCheat = value; }
        public static bool BFreeSupport { get => bFreeSupport; set => bFreeSupport = value; }
        public static bool InitComplete { get => initComplete; set => initComplete = value; }

        public static void InitPatch()
        {
            
            if (!InitComplete)
            {
                //SkUtilities.Logz(new string[] { "SkCommandPatcher", "INJECT" }, new string[] { "Attempting injection..." });
                try
                {
                    //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                    harmony = Harmony.CreateAndPatchAll(typeof(SkCommandPatcher).Assembly);
                    //SkUtilities.Logz(new string[] { "SkCommandPatcher", "INJECT" }, new string[] { "INJECT => COMPLETE" });
                }
                catch (Exception ex)
                //catch (Exception)
                {
                    SkCommandProcessor.PrintOut("Something failed, there is a strong possibility another mod blocked this operation.", SkCommandProcessor.LogTo.Console);
                    SkUtilities.Logz(new string[] { "SkCommandPatcher", "PATCH" }, new string[] { "PATCH => FAILED. CHECK FOR OTHER MODS BLOCKING PATCHES.\n", ex.Message, ex.StackTrace }, UnityEngine.LogType.Error);
                }
                finally
                {
                    InitComplete = true;
                }
            }
        }

        [HarmonyPatch(typeof(Console), "IsConsoleEnabled")]
        public static class PatchIsConsoleEnabled
        {
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(bool __result)
            {
                if (Configuration.SkConfigEntry.CConsoleEnabled != null && Configuration.SkConfigEntry.CConsoleEnabled.Value)
                {
                    __result = Configuration.SkConfigEntry.CConsoleEnabled.Value;
                }
            }
        }

        //[HarmonyPatch(typeof(Console), "IsCheatsEnabled")]
        //public static class PatchIsCheatsEnabled
        //{
        //    public static void Postfix(bool __result)
        //    {
        //        __result = SkCommandPatcher.BCheat;
        //    }
        //}

        [HarmonyPatch(typeof(WearNTear), "UpdateSupport")]
        private static class PatchUpdateSupport
        {
            private static bool Prefix(ref float ___m_support, ref ZNetView ___m_nview)
            {
                if (SkCommandPatcher.BFreeSupport)
                {
                    ___m_support += ___m_support;
                    ___m_nview.GetZDO().Set("support", ___m_support);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        private static class PatchUpdatePlacementGhost
        {
            private static void Postfix(bool flashGuardStone)
            {
                if (SkCommandPatcher.bBuildAnywhere)
                {
                    try
                    {
                        if (Player.m_localPlayer != null)
                        {
                            int plsout = (int)SkUtilities.GetPrivateField<int>(Player.m_localPlayer, "m_placementStatus");
                            if (plsout != 4)
                            {
                                SkUtilities.SetPrivateField(Player.m_localPlayer, "m_placementStatus", 0);
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Location), "IsInsideNoBuildLocation")]
        private static class PatchIsInsideNoBuildLocation
        {
            private static void Postfix(ref bool __result)
            {
                if (SkCommandPatcher.bBuildAnywhere)
                {
                    __result = false;
                }
            }
        }
    }
}
