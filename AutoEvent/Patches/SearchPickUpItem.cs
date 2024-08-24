using System;
using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Searching;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchPickUpItemPatch
    {
        private static bool Prefix(SearchCoordinator __instance, out SearchSession? session, out SearchCompletor completor)
        {
            DebugLogger.LogDebug("ReceiveRequestUnsafe");
            SearchRequest request = __instance.SessionPipe.Request;
            completor = SearchCompletor.FromPickup(__instance, request.Target, (double) __instance.ServerMaxRayDistanceSqr);
            if (!completor.ValidateStart())
            {
                session = new SearchSession?();
                completor = (SearchCompletor) null;
                return true;
            }
            
            SearchPickUpItemArgs ev = new(Player.Get(__instance.Hub), request.Target, request.Body, completor, request.Target.SearchTimeForPlayer(__instance.Hub));
            Players.OnSearchPickUpItem(ev);
            if (ev.IsAllowed)
            {
                session = null;
                completor = null;
                return true;
            }
            completor = ev.SearchCompletor;
            session = ev.SearchSession;
            
            SearchSession body = request.Body;
            if (!__instance.isLocalPlayer)
            {
                double num1 = NetworkTime.time - request.InitialTime;
                double num2 = (double) LiteNetLib4MirrorServer.Peers[__instance.connectionToClient.connectionId].Ping * 0.001 * __instance.serverDelayThreshold;
                float num3 = request.Target.SearchTimeForPlayer(__instance.Hub);
                if (num1 < 0.0 || num2 < num1)
                {
                    body.InitialTime = NetworkTime.time - num2;
                    body.FinishTime = body.InitialTime + (double) num3;
                }
                else if (Math.Abs(body.FinishTime - body.InitialTime - (double) num3) > 0.001)
                    body.FinishTime = body.InitialTime + (double) num3;
            }
            session = new SearchSession?(body);
            return false;
        }
    }
}