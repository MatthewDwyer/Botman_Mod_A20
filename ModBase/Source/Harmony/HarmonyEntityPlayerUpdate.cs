//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Botman.Commands;
//using UnityEngine;
//using HarmonyLib;
//using JetBrains.Annotations;

//namespace Botman.Patches
//{
//    [HarmonyPatch(typeof(EntityPlayer), "Update")]
//    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]

//    class EntityPlayerUpdate

//    {

//        public static Dictionary<string, int> Players = new Dictionary<string, int>();
//        //public class State
//        //{
//        //    public int EntityId;
//        //    public int Level;
//        //}
//        //public static bool Prefix([NotNull]EntityPlayer __instance, ref State __state)
//        //{
//        //    __state = new State()
//        //    {
//        //        EntityId = __instance.entityId,
//        //        Level = XUiM_Player.GetLevel(__instance),
//        //    };
//        //    return true;
//        //}

//        public static void Postfix([NotNull]EntityPlayer __instance)
//        {
//            ClientInfo _cInfo = ConsoleHelper.ParseParamEntityIdToClientInfo(__instance.entityId.ToString());
//            if (_cInfo == null)
//            {
//                return;
//            }
//            if (!Players.ContainsKey(_cInfo.playerName))
//            {
//                Players.Add(_cInfo.playerName, XUiM_Player.GetLevel(__instance));
//                return;
//            }
//            if (Players[_cInfo.playerName] != XUiM_Player.GetLevel(__instance))
//            {
//                LevelSystem.AnnounceMilestone(_cInfo, XUiM_Player.GetLevel(__instance));
//                Players.Remove(_cInfo.playerName);
//                Players.Add(_cInfo.playerName, XUiM_Player.GetLevel(__instance));
//                return;
//            }
//        }
//    }
//}