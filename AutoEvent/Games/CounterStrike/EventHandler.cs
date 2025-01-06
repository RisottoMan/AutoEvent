using System;
using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace AutoEvent.Games.CounterStrike;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        if (ev.Pickup.Info.ItemId != ItemType.SCP018)
            return;

        if (_plugin.BombState == BombState.NoPlanted)
        {
            if (ev.Player.IsCHI)
            {
                _plugin.BombState = BombState.Planted;
                _plugin.RoundTime = new TimeSpan(0, 0, 35);
                _plugin.BombObject.transform.position = ev.Pickup.Position + new Vector3(0, -0.25f, 0);
                
                Extensions.PlayAudio("BombPlanted.ogg", 5, false);
                ev.Player.ShowHint(_plugin.Translation.YouPlanted);
            }
        }
        else if (_plugin.BombState == BombState.Planted)
        {
            if (ev.Player.IsNTF && Vector3.Distance(ev.Player.Position, _plugin.BombObject.transform.position) < 3)
            {
                _plugin.BombState = BombState.Defused;
                ev.Player.ShowHint(_plugin.Translation.YouDefused);
            }
        }

        ev.IsAllowed = false;
    }
}