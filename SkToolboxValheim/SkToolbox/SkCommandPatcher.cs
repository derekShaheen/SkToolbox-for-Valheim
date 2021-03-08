using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SkToolbox
{
    internal static class SkCommandPatcher
    {
        public static Harmony harmony = null;

        private static bool initComplete = false;

        public static bool bCheat = false;
        public static bool bFreeSupport = false;
        public static bool bBuildAnywhere = false;

        public static void InitPatch()
        {
            if (!initComplete)
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
                    SkUtilities.Logz(new string[] { "SkCommandPatcher", "INJECT" }, new string[] { "INJECT => FAILED. CHECK FOR OTHER MODS BLOCKING PATCHES.", ex.Message, ex.StackTrace }, UnityEngine.LogType.Error);
                }
                finally
                {
                    initComplete = true;
                }
            }
        }

        [HarmonyPatch(typeof(Console), "IsCheatsEnabled")]
        public static class PatchIsCheatsEnabled
        {
            public static void Postfix(bool __result)
            {
                __result = SkCommandPatcher.bCheat;
            }
        }

        //[HarmonyPatch(typeof(Console), "AddString")]
        //private static class PatchAddString
        //{
        //    static List<string> currentBuffer = new List<string>();
        //    private static bool Prefix(string text)
        //    {
        //        if (Console.instance != null)
        //        {
        //            currentBuffer = SkUtilities.GetPrivateField<List<string>>(Console.instance, "m_chatBuffer");
        //            currentBuffer.Add(text);
        //            while (currentBuffer.Count > 40)
        //            {
        //                currentBuffer.RemoveAt(0);
        //            }
        //            SkUtilities.SetPrivateField(Console.instance, "m_chatBuffer", currentBuffer);
        //            Console.instance.GetType().GetMethod("UpdateChat", SkUtilities.BindFlags).Invoke(Console.instance, null);
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(WearNTear), "UpdateSupport")]
        private static class PatchUpdateSupport
        {
            private static bool Prefix(ref float ___m_support, ref ZNetView ___m_nview)
            {
                if (SkCommandPatcher.bFreeSupport)
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
