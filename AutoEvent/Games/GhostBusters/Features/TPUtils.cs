// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         TPUtils.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 5:32 PM
//    Created Date:     10/28/2023 5:32 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem.Commands.RemoteAdmin;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.GhostBusters.Features;

public class TPUtils
{
    public static Vector3 GetRoomInHeavy(Player ply)
    {
        var room = Map.GetRandomRoom(FacilityZone.HeavyContainment);
        RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(room.ApiRoom.Position);
        
        DoorVariant[] whitelistedDoorsForZone = PlayerRoles.PlayableScps.Scp106.Scp106PocketExitFinder.GetWhitelistedDoorsForZone(roomIdentifier.Zone);
        if (whitelistedDoorsForZone.Length != 0)
        {
            DoorVariant randomDoor = PlayerRoles.PlayableScps.Scp106.Scp106PocketExitFinder.GetRandomDoor(whitelistedDoorsForZone);
            float range = ((roomIdentifier.Zone == FacilityZone.Surface) ? 45f : 11f);
            if (ply.RoleBase is not IFpcRole fpc)
                goto NearestDoor;
            return PlayerRoles.PlayableScps.Scp106.Scp106PocketExitFinder.GetSafePositionForDoor(randomDoor, range, fpc.FpcModule.CharController);
        }
        NearestDoor:
        return DoorVariant.AllDoors.OrderBy(x => Vector3.Distance(ply.Position, x.transform.position)).FirstOrDefault()?.transform.position ?? ply.Position;
    }
    
}