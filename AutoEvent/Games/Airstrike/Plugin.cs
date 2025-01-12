using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Airstrike;
public class Plugin : Event<Config, Translation>, IEventMap, IEventSound
{
    public override string Name { get; set; } = "Airstrike Party";
    public override string Description { get; set; } = "Survive as aistrikes rain down from above.";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "airstrike";
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "DeathParty", 
        Position = new Vector3(0f, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "DeathParty.ogg", 
        Volume = 5
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    protected override FriendlyFireSettings ForceEnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Disable;
    private EventHandler _eventHandler { get; set; }
    private CoroutineHandle _grenadeCoroutineHandle;
    public int Stage { get; private set; }
    public List<GameObject> SpawnList;
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.ThrownProjectile += _eventHandler.OnThrownProjectile;
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
    }
    
    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.ThrownProjectile -= _eventHandler.OnThrownProjectile;
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        Server.FriendlyFire = true;

        SpawnList = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.Loadouts);
            player.Position = SpawnList.RandomItem().transform.position;
        }
    }

    protected override void OnStop()
    {
        Timing.CallDelayed(1.2f, () => 
        {
            if (_grenadeCoroutineHandle.IsRunning)
            {
                Timing.KillCoroutines([_grenadeCoroutineHandle]);
            }
        });
    }
    
    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int _time = 10; _time > 0; _time--)
        {
            Extensions.Broadcast($"<size=100><color=red>{_time}</color></size>", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    { 
        _grenadeCoroutineHandle = Timing.RunCoroutine(GrenadeCoroutine(), "death_grenade");
    }

    protected override void ProcessFrame()
    {
        var count = Player.List.Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript).ToString();
        var cycleTime = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
        Extensions.Broadcast(Translation.Cycle.Replace("{count}", count).Replace("{time}", cycleTime), 1);
    }

    protected override bool IsRoundDone()
    {
        int playerCount = Player.List.Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
        return !(playerCount > (Config.LastPlayerAliveWins ? 1 : 0) 
            && Stage <= Config.Rounds);
    }

    public IEnumerator<float> GrenadeCoroutine()
    { 
        Stage = 1;
        float fuse = 10f;
        float height = 20f;
        float count = 20;
        float timing = 1f;
        float scale = 4;
        float radius = MapInfo.Map.AttachedBlocks.First(x => x.name == "Arena").transform.localScale.x / 2 - 6f;

        while (Player.List.Count(r => r.IsAlive) > (Config.LastPlayerAliveWins ? 1 : 0) && Stage <= Config.Rounds)
        {
            if (KillLoop)
            {
                yield break;
            }

            DebugLogger.LogDebug($"Stage: {Stage}/{Config.Rounds}. Radius: {radius}, Scale: {scale}, Count: {count}, Timing: {timing}, Height: {height}, Fuse: {fuse}, Target: {Config.TargetPlayers}");
            
            // Not the last round.
            if (Stage != Config.Rounds)
            {
                int playerIndex = 0;
                for (int i = 0; i < count; i++)
                {

                    Vector3 pos = MapInfo.Map.Position + new Vector3(Random.Range(-radius, radius), height, Random.Range(-radius, radius));
                    // has to be re-iterated every run because a player could have been killed from the last one.
                    if (Config.TargetPlayers)
                    {
                        try
                        {
                            Player randomPlayer = Player.List.Where(x => x.Role == RoleTypeId.ClassD).ToList().RandomItem();
                            pos = randomPlayer.Position;
                            pos.y = height + MapInfo.Map.Position.y;
                        }
                        catch (Exception e)
                        {
                            DebugLogger.LogDebug("Caught an error while targeting a player.", LogLevel.Warn, true);
                            DebugLogger.LogDebug($"{e}");
                        }
                    }
                    Extensions.GrenadeSpawn(pos, scale, fuse);
                    yield return Timing.WaitForSeconds(timing);
                    playerIndex++;
                }
            }
            else // last round.
            {
                Vector3 pos = MapInfo.Map.Position + new Vector3(Random.Range(-10, 10), 20, Random.Range(-10, 10));
                Extensions.GrenadeSpawn(pos, 75, 10, 0);
            }

            yield return Timing.WaitForSeconds(15f);
            Stage++;

            // Defaults: 
            count += 30;     //20,  50,  80,  110, [ignored last round] 1
            timing -= 0.3f;  //1.0, 0.7, 0.4, 0.1, [ignored last round] 10
            height -= 5f;    //20,  15,  10,  5,   [ignored last round] 20
            fuse -= 2f;      //10,  8,   6,   4,   [ignored last round] 10
            scale -= 1;      //4,   3,   2,   1,   [ignored last round] 75
            radius += 7f;    //4,   11,  18,  25   [ignored last round] 10
        }

        DebugLogger.LogDebug("Finished Grenade Coroutine.");
        yield break;
    }

    protected override void OnFinished()
    {
        if (_grenadeCoroutineHandle.IsRunning)
        {
            KillLoop = true;
            Timing.CallDelayed(1.2f, () => {
                if (_grenadeCoroutineHandle.IsRunning)
                {
                    Timing.KillCoroutines(new CoroutineHandle[] { _grenadeCoroutineHandle });
                }
            });
        }
        var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
        int count = Player.List.Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
        if (count > 1)
        {
            Extensions.Broadcast(Translation.MorePlayer.Replace("{count}", $"{Player.List.Count(r => r.Role != RoleTypeId.ChaosConscript)}").Replace("{time}", time), 10);
        }
        else if (count == 1)
        {
            var player = Player.List.First(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
            player.Health = 1000;
            Extensions.Broadcast(Translation.OnePlayer.Replace("{winner}", player.Nickname).Replace("{time}", time), 10);
        }
        else
        {
            Extensions.Broadcast(Translation.AllDie.Replace("{time}", time), 10);
        }
    }
}