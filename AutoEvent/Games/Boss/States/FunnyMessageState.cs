using PluginAPI.Core;
using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class FunnyMessageState : IBossState
{
    public string Name { get; } = "FunnyMessage";
    public string Description { get; } = "The state in which Santa creates a funny message that prevents shooting";
    public int Stage { get; } = 1;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 5);

    // Warning Evil Santa!
    public string text = $"<line-height=95%><voffset=10em><size=30%><b><i>Warning\nEvil Santa Enrage</i></b>";
    public void Init(Plugin plugin)
    {

    }

    public void Update()
    {
        foreach(Player player in Player.GetPlayers())
        {
            player.ReceiveHint(text, 0.1f);
        }
    }

    public void Deinit()
    {

    }
}
