﻿using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using Newtonsoft.Json.Linq;
using StayInTarkov.Configuration;
using StayInTarkov.Coop.Components.CoopGameComponents;
using StayInTarkov.Coop.Matchmaker;
using StayInTarkov.Coop.Players;
using StayInTarkov.Coop.Web;
using StayInTarkov.Core.Player;
using StayInTarkov.Networking;
using StayInTarkov.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;

namespace StayInTarkov.Coop.Player
{
    internal class Player_Init_Coop_Patch : ModulePatch
    {
        private static ConfigFile _config;
        public Player_Init_Coop_Patch(ConfigFile config)
        {
            _config = config;
        }

        protected override MethodBase GetTargetMethod()
        {
            return ReflectionHelpers.GetMethodForType(typeof(EFT.LocalPlayer), "Init");
        }

        [PatchPostfix]
        public static void PatchPostfix(EFT.LocalPlayer __instance)
        {
            if (__instance is HideoutPlayer)
                return;

            var player = __instance;
            var profileId = player.ProfileId;

            //await __result;
            //Logger.LogInfo($"{nameof(EFT.LocalPlayer)}.Init:{accountId}:IsAi={player.IsAI}");


            var coopGC = CoopGameComponent.GetCoopGameComponent();
            if (coopGC == null)
            {
                Logger.LogError("Cannot add player to Coop Game Component because its NULL");
                return;
            }


            if (Singleton<GameWorld>.Instance != null)
            {
                if (!coopGC.Players.ContainsKey(profileId))
                    coopGC.Players.TryAdd(profileId, (CoopPlayer)player);

                if (!Singleton<GameWorld>.Instance.RegisteredPlayers.Any(x => x.ProfileId == profileId))
                    Singleton<GameWorld>.Instance.RegisterPlayer(player);

                //if(CullingManager.Instance != null)
                //{
                //    CullingManager.Instance.ForceEnable(false);
                //    GameObject.Destroy(CullingManager.Instance.gameObject);
                //    CullingManager.Destroy(CullingManager.Instance);
                //}

            }
            else
            {
                Logger.LogError("Cannot add player because GameWorld is NULL");
                return;
            }

            CoopGame.SendPlayerDataToServer(player);

            JObject joinPacket = new();
            joinPacket.Add("profileId", MatchmakerAcceptPatches.Profile.ProfileId);
            joinPacket.Add("serverId", MatchmakerAcceptPatches.Profile.ProfileId);
            joinPacket.Add("m", "JoinMatch");
            //AkiBackendCommunication.Instance.PostDownWebSocketImmediately(joinPacket.SITToJson());
            GameClient.SendData(Encoding.UTF8.GetBytes(joinPacket.ToString())); 

            //if (PluginConfigSettings.Instance.CoopSettings.SETTING_ShowFeed)
            //    DisplayMessageNotifications.DisplayMessageNotification($"{__instance.Profile.Nickname}[{__instance.Side}][{__instance.Profile.Info.Settings.Role}] has spawned");

            // If a Player
            if (
                PluginConfigSettings.Instance.CoopSettings.SETTING_ShowFeed
                && (__instance.ProfileId.StartsWith("pmc") || __instance.ProfileId.StartsWith("scav"))
                )
                DisplayMessageNotifications.DisplayMessageNotification($"{__instance.Profile.Nickname}[{__instance.Side}][{__instance.Profile.Info.Settings.Role}] has spawned");

        }

        //public static void SendPlayerDataToServer(EFT.LocalPlayer player)
        //{
        //    var profileJson = player.Profile.SITToJson();


        //    Dictionary<string, object> packet = new()
        //    {
        //                {
        //                    "serverId",
        //                    MatchmakerAcceptPatches.GetGroupId()
        //                },
        //                {
        //                "isAI",
        //                    player.IsAI || !player.Profile.Id.StartsWith("pmc")
        //                },
        //                //{
        //                //    "accountId",
        //                //    //player.Profile.AccountId
        //                //    player.ProfileId
        //                //},
        //                {
        //                    "profileId",
        //                    player.ProfileId
        //                },
        //                {
        //                    "groupId",
        //                    Matchmaker.MatchmakerAcceptPatches.GetGroupId()
        //                },
        //                {
        //                    "sPx",
        //                    player.Transform.position.x
        //                },
        //                {
        //                    "sPy",
        //                    player.Transform.position.y
        //                },
        //                {
        //                    "sPz",
        //                    player.Transform.position.z
        //                },
        //                {
        //                    "profileJson",
        //                    profileJson
        //                },
        //                { "m", "PlayerSpawn" },
        //            };


        //    //Logger.LogDebug(packet.ToJson());

        //    var prc = player.GetOrAddComponent<PlayerReplicatedComponent>();
        //    prc.player = player;
        //    AkiBackendCommunicationCoop.PostLocalPlayerData(player, packet);

        //}
    }
}
