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
    public TimeSpan Timer { get; set; }

    public string text = $"" +
    $"███████╗██╗  ██╗ █████╗ ██████╗ ██╗  ██╗" +
    $"██╔════╝██║  ██║██╔══██╗██╔══██╗██║ ██╔╝" +
    $"█████╗  ███████║███████║██████╔╝█████╔╝" +
    $"██╔══╝  ██╔══██║██╔══██║██╔══██╗██╔═██╗" +
    $"███████╗██║  ██║██║  ██║██║  ██║██║  ██╗" +
    $"╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝";
    public void Init(Plugin plugin)
    {

    }

    public void Update()
    {
        foreach(Player player in Player.GetPlayers())
        {
            player.ReceiveHint(text, 1);
        }
    }

    public void Deinit()
    {

    }
}
