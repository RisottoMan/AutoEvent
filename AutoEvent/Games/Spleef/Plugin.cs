// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:20 PM
//    Created Date:     10/17/2023 6:20 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using AutoEvent.API.Components;
using AutoEvent.API.Enums;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.FallDown;
using AutoEvent.Games.Spleef.Configs;
using AutoEvent.Interfaces;
using CommandSystem.Commands.RemoteAdmin;
using InventorySystem.Items.Usables;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Utils.NonAllocLINQ;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Spleef;

public class Plugin : Event, IEventMap, IEventSound
{
    public override string Name { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefName;
    public override string Description { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefDescription;
    public override string Author { get; set; } = "Redforce04";
    public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefCommandName;
    public override Version Version { get; set; } = new Version(1, 0, 1);
    public SpleefTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate;

    [EventConfig]
    public SpleefConfig Config { get; set; }
    public EventHandler EventHandler { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
        { MapName = "Puzzle", Position = new Vector3(76f, 1026.5f, -43.68f), };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { SoundName = "Puzzle.ogg", Volume = 15, Loop = true };

    /// <summary>
    /// A local list of platforms that changes round to round.
    /// </summary>
    private List<GameObject> _listPlatforms;

    private List<GameObject> _colorIndicators;
    private float _spawnHeight; 

    /// <summary>
    /// All platforms in the map.
    /// </summary>
    private Dictionary<ushort, GameObject> _platforms; 
    private GameObject _lava;

    protected override void RegisterEvents()
    {
        EventHandler = new EventHandler();
        Servers.TeamRespawn += EventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += EventHandler.OnPlaceBullet;
        Servers.PlaceBlood += EventHandler.OnPlaceBlood;
        Players.DropItem += EventHandler.OnDropItem;
        Players.DropAmmo += EventHandler.OnDropAmmo;
        EventManager.RegisterEvents(EventHandler);
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(EventHandler);
        
        EventHandler = null;
    }

    protected override void OnStart()
    {
        _platforms = new Dictionary<ushort, GameObject>();
        _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
        _lava.AddComponent<LavaComponent>();
        _colorIndicators = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Cube").ToList();
        GeneratePlatforms(Config.PlatformAxisCount);
        foreach (Player player in Player.GetPlayers())
        {
            player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
        }
        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(Config.PlayerLoadouts, LoadoutFlags.IgnoreWeapons);
            ply.Position = MapInfo.Position + new Vector3(0,Config.LayerCount * 3f + 5,0);
        }
    }
    private void GeneratePlatforms(int amountPerAxis = 5)
        {
            
            float areaSizeX = 20f;
            float areaSizeY = 20f;
            float sizeX = areaSizeX / amountPerAxis;
            float sizeY = areaSizeY / amountPerAxis;
            float startPosX = -(areaSizeX/2f) + sizeX / 2f;
            float startPosY = -(areaSizeY/2f) + sizeY / 2f;
            float startPosZ = 6f;
            float breakSize = .2f;
            float sizeZ = 3f;
            _spawnHeight = 6f;
            List<SpleefPlatform> platforms = new List<SpleefPlatform>();
            for (int z = 0; z < Config.LayerCount; z++)
            {
                for (int x = 0; x < amountPerAxis; x++)
                {
                    for (int y = 0; y < amountPerAxis; y++)
                    {
                        float posX = startPosX + (sizeX * x);
                        float posY = startPosY + (sizeY * y);
                        float posZ = startPosZ + (sizeZ * z);
                        var plat = new SpleefPlatform(sizeX - breakSize, sizeY - breakSize, .3f, posX, posY, posZ);
                        platforms.Add(plat);
                        if (posZ > _spawnHeight + 2)
                        {
                            _spawnHeight = posZ + 2;
                        }
                    }
                }
            }

            var primary = MapInfo.Map.AttachedBlocks.FirstOrDefault(x => x.name == "Platform");
            foreach(var plat in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform"))
            {
                if (plat.GetInstanceID() != primary.GetInstanceID())
                    GameObject.Destroy(plat);
            }

            ushort id = 0;
            foreach (SpleefPlatform platform in platforms)
            {
                
                Vector3 position = MapInfo.Map.Position + new Vector3(platform.PositionX, platform.PositionZ ,platform.PositionY);
                var newPlatform = GameObject.Instantiate(primary, position, Quaternion.identity);
                _platforms.Add(id, newPlatform);

                newPlatform.AddComponent<DestructiblePrimitiveComponent>().DamagingPrimitive += OnDamage;
                
                var prim = newPlatform.GetComponent<PrimitiveObjectToy>() ?? newPlatform.AddComponent<PrimitiveObjectToy>();
                prim.Position = position;
                prim.NetworkPosition = position;
                prim.transform.position = position;
                prim.transform.localPosition = position;
                prim.Scale = new Vector3(platform.X , platform.Z, platform.Y);
                prim.NetworkScale = new Vector3(platform.X , platform.Z, platform.Y);
                prim.PrimitiveType = PrimitiveType.Cube;
                prim.transform.localScale = new Vector3(platform.X, platform.Z, platform.Y);
                NetworkServer.UnSpawn(newPlatform);
                NetworkServer.Spawn(newPlatform);
                id++;
            }
            GameObject.Destroy(primary);
        }

    private void OnDamage(DamagingPrimitiveArgs ev)
    {
        DebugLogger.LogDebug("Damaging Primitive.");
    }
    protected override bool IsRoundDone()
    {
        return !(Player.GetPlayers().Count(ply => ply.IsAlive) > 1) && EventTime.TotalSeconds < Config.RoundDurationInSeconds;
    }

    protected override void ProcessFrame()
    {
        
    }

    protected override void OnFinished()
    {
        
    }

    protected override void OnCleanup()
    {
        foreach (var x in this._platforms)
        {
            GameObject.Destroy(x.Value);
        }
        base.OnCleanup();
    }
}