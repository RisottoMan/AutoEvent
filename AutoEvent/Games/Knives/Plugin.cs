using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Features;

namespace AutoEvent.Games.Knives;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Knives of Death";
    public override string Description { get; set; } = "Knife players against each other on a 35hp map from cs 1.6";
    public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
    public override string CommandName { get; set; } = "knives";
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "35hp_2", 
        Position = new Vector3(0, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Knife.ogg", 
        Volume = 10
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
    private EventHandler _eventHandler { get; set; }
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler();
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        var count = 0;
        List<GameObject> spawnList = MapInfo.Map.AttachedBlocks.Where(r => r.name.Contains("Spawnpoint")).ToList();
        foreach (Player player in Player.List)
        {
            if (count % 2 == 0)
            {
                player.GiveLoadout(Config.Team1Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                player.Position = spawnList.ElementAt(0).transform.position;
            }
            else
            {
                player.GiveLoadout(Config.Team2Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                player.Position = spawnList.ElementAt(1).transform.position;
            }
            count++;

            if (player.CurrentItem == null)
            {
                player.CurrentItem = player.AddItem(ItemType.Jailbird);
            }
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
            yield return Timing.WaitForSeconds(1f);
        }   
    }

    protected override void CountdownFinished()
    {
        foreach(var wall in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Wall"))
        {
            GameObject.Destroy(wall);
        }
    }

    protected override bool IsRoundDone()
    {   
        return !(Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0 &&
               Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) > 0);
    }

    protected override void ProcessFrame()
    {
        string mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces).ToString();
        string chaosCount = Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency).ToString();
        Extensions.Broadcast(Translation.Cycle.
            Replace("{name}", Name).
            Replace("{mtfcount}", mtfCount).
            Replace("{chaoscount}", chaosCount), 1);
    }

    protected override void OnFinished()
    {
        if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
        {
            Extensions.Broadcast(Translation.ChaosWin.Replace("{name}", Name), 10);
        }
        else if (Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) == 0)
        {
            Extensions.Broadcast(Translation.MtfWin.Replace("{name}", Name), 10);
        }
    }
}