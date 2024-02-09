using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Spleef.Features;
using AutoEvent.Interfaces;
using MEC;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Version = System.Version;

namespace AutoEvent.Games.Spleef;

public class Plugin : Event, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Spleef";
    public override string Description { get; set; } = "Shoot at the platforms and don't fall into the void";
    public override string Author { get; set; } = "Redforce04 (created logic code) && KoT0XleB (modified map)";
    public override string CommandName { get; set; } = "spleef";
    public override Version Version { get; set; } = new Version(1, 0, 3);
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;

    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public EventHandler _eventHandler { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    { 
        MapName = "Spleef",
        Position = new Vector3(76f, 1026.5f, -43.68f)
    };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    {
        SoundName = "Fall_Guys_Winter_Fallympics.ogg",
        Volume = 7
    };
    private float _spawnHeight;
    private TimeSpan _remaining;
    private Dictionary<ushort, GameObject> _platforms;
    List<Loadout> _playerLoadounts;
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
        Players.Shot += _eventHandler.OnShot;
        EventManager.RegisterEvents(_eventHandler);
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(_eventHandler);
        Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
        Players.DropItem -= _eventHandler.OnDropItem;
        Players.DropAmmo -= _eventHandler.OnDropAmmo;
        Players.Shot -= _eventHandler.OnShot;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _remaining = TimeSpan.FromSeconds(Config.RoundDurationInSeconds);
        _platforms = new Dictionary<ushort, GameObject>();
        _playerLoadounts = new List<Loadout>();

        GameObject lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
        lava.AddComponent<LavaComponent>().StartComponent(this);

        GeneratePlatforms(Config.PlatformAxisCount);

        int plyCount = Player.GetPlayers().Count();
        if (plyCount <= 5)
        {
            _playerLoadounts = Config.PlayerLittleLoadouts;
        }
        else if (plyCount >= 15)
        {
            _playerLoadounts = Config.PlayerBigLoadouts;
        }
        else
        {
            _playerLoadounts = Config.PlayerNormalLoadouts;
        }

        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(_playerLoadounts, LoadoutFlags.IgnoreWeapons);
            ply.Position = MapInfo.Position + new Vector3(0, Config.LayerCount * 3f + 5, 0);
        }
    }

    private void GeneratePlatforms(int amountPerAxis = 5)
    {
        float areaSizeX = 20f;
        float areaSizeY = 20f;
        float sizeX = areaSizeX / amountPerAxis;
        float sizeY = areaSizeY / amountPerAxis;
        float startPosX = -(areaSizeX / 2f) + sizeX / 2f;
        float startPosY = -(areaSizeY / 2f) + sizeY / 2f;
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

                    Color color = Color.green;
                    switch(z)
                    {
                        case 0: color = Color.red; break;
                        case 1: color = Color.magenta; break;
                        case 2: color = Color.cyan; break;
                        case 3: color = Color.yellow; break;
                        case 4: color = Color.blue; break;
                    }

                    var plat = new SpleefPlatform(sizeX - breakSize, sizeY - breakSize, .3f, posX, posY, posZ, color);
                    platforms.Add(plat);
                    if (posZ > _spawnHeight + 2)
                    {
                        _spawnHeight = posZ + 2;
                    }
                }
            }
        }

        var primary = MapInfo.Map.AttachedBlocks.FirstOrDefault(x => x.name == "Parent-Platform");
        /*foreach (var plat in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform"))
        {
            if (plat.GetInstanceID() != primary.GetInstanceID())
                GameObject.Destroy(plat);
        }*/

        ushort id = 0;
        foreach (SpleefPlatform platform in platforms)
        {

            Vector3 position = MapInfo.Map.Position + new Vector3(platform.PositionX, platform.PositionZ, platform.PositionY);
            var newPlatform = GameObject.Instantiate(primary, position, Quaternion.identity);
            _platforms.Add(id, newPlatform);

            try
            {
                var component = newPlatform.AddComponent<FallPlatformComponent>();
                component.Init(Config.RegeneratePlatformsAfterXSeconds, Config.PlatformFallDelay, Config.PlatformHealth, 15);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Exception \n{e}");
            }

            var prim = newPlatform.GetComponent<PrimitiveObjectToy>() ?? newPlatform.AddComponent<PrimitiveObjectToy>();
            prim.NetworkMaterialColor = platform.Color;

            prim.Position = position;
            prim.NetworkPosition = position;
            prim.transform.position = position;
            prim.transform.localPosition = position;
            prim.Scale = new Vector3(platform.X, platform.Z, platform.Y);
            prim.NetworkScale = new Vector3(platform.X, platform.Z, platform.Y);
            prim.PrimitiveType = PrimitiveType.Cube;
            prim.transform.localScale = new Vector3(platform.X, platform.Z, platform.Y);
            NetworkServer.UnSpawn(newPlatform);
            NetworkServer.Spawn(newPlatform);
            id++;
        }
        GameObject.Destroy(primary);
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"{Translation.Description}\n" +
                $"{Translation.Start.Replace("{time}", $"{time}")}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    protected override void CountdownFinished()
    {
        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(_playerLoadounts, LoadoutFlags.ItemsOnly);
        }
    }

    protected override bool IsRoundDone()
    {
        _remaining -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        return !(Player.GetPlayers().Count(ply => ply.IsAlive) > 1) &&
             EventTime.TotalSeconds < Config.RoundDurationInSeconds;
    }

    protected override void ProcessFrame()
    {
        var remaining = $"{_remaining.Minutes:00}:{_remaining.Seconds:00}";
        int count = Player.GetPlayers().Count(x => x.IsAlive);
        foreach (Player ply in Player.GetPlayers())
        {
            ply.SendBroadcast(Translation.Running.
                Replace("{players}", count.ToString()).
                Replace("{remaining}", $"{remaining}"), (ushort)this.FrameDelayInSeconds);
        }
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        int count = Player.GetPlayers().Count(x => x.IsAlive);

        if (count > 1)
        {
            text = Translation.SomeSurvived;
        }
        else if (count == 1)
        {
            text = Translation.Winner.Replace("{winner}", Player.GetPlayers().First(x => x.IsAlive).Nickname);
        }
        else
        {
            text = Translation.AllDied;
        }

        Extensions.Broadcast(text, 10);
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