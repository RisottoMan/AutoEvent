using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;

namespace AutoEvent.Games.FallDown;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "FallDown";
    public override string Description { get; set; } = "All platforms are destroyed. It is necessary to survive";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "fall";
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "FallDown", 
        Position = new Vector3(0f, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Fall_Guys_Winter_Fallympics.ogg",
        Volume = 7
    };
    protected override float FrameDelayInSeconds { get; set; } = 0.9f;
    private int _platformId;
    private List<GameObject> _platforms;
    private GameObject _lava;
    private bool _noPlatformsRemainingWarning;

    protected override void OnStart()
    {
        _noPlatformsRemainingWarning = true;

        List<GameObject> spawnList = MapInfo.Map.AttachedBlocks.Where(r => r.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.Loadouts);
            player.Position = spawnList.RandomItem().transform.position;
        }

        _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
        _lava.AddComponent<LavaComponent>().StartComponent(this);
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float time = 15; time > 0; time--)
        {
            Extensions.Broadcast($"{time}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    {
        _platformId = 0;
        _platforms = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform").ToList();
        GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(x => x.name == "Wall"));
        if (Config.PlatformsHaveColorWarning)
        {
            foreach (var platform in _platforms)
            {
                platform.GetComponent<PrimitiveObject>().Primitive.Color = Color.white;
            }
        }
    }

    protected override bool IsRoundDone()
    {
        // Over 1 player is alive &&
        // over 1 platform is present. 
        return !(Player.List.Count(r => r.IsAlive) > 1 && _platforms.Count > 1);
    }
    protected override void ProcessFrame()
    {
        _platformId++;
        FrameDelayInSeconds = Config.DelayInSeconds.GetValue(_platformId, 169, 1, 0.3f);

        var count = Player.List.Count(r => r.IsAlive);
        var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
        Extensions.Broadcast(Translation.Broadcast.Replace("{name}", Name).Replace("{time}", time).Replace("{count}", $"{count}"), 1);
        
        if (_platforms.Count < 1)
        {
            if (_noPlatformsRemainingWarning)
            {
                DebugLogger.LogDebug("No platforms remaining.");
                _noPlatformsRemainingWarning = false;
            }
            return;
        }
            
        var platform = _platforms.RandomItem();
        platform.GetComponent<PrimitiveObject>().Primitive.Color = Color.red;
        if (Config.PlatformsHaveColorWarning)
        {
            Timing.CallDelayed(Config.WarningDelayInSeconds.GetValue(_platformId, 169, 0, 3), () =>
            {
                _platforms.Remove(platform);
                GameObject.Destroy(platform);
            });
        }
        else
        {
            _platforms.Remove(platform);
            GameObject.Destroy(platform);
        }

    }

    protected override void OnFinished()
    {
        if (Player.List.Count(r => r.IsAlive) == 1)
        {
            Extensions.Broadcast(Translation.Winner.Replace("{winner}", Player.List.First(r => r.IsAlive).Nickname), 10);
        }
        else
        {
            Extensions.Broadcast(Translation.Died, 10);
        }
    }
}