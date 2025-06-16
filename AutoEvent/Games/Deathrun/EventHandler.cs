using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    public void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        ev.IsAllowed = false;

        DebugLogger.LogDebug("[Deathrun] click to button");
        
        // Start the animation when click on the button
        Animator animator = ev.Pickup.GameObject.GetComponentInParent<Animator>();
        if (animator != null)
        {
            DebugLogger.LogDebug($"[Deathrun] activate animation {animator.name}action");
            animator.Play(animator.name + "action");
        }
    }
}