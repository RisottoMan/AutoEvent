using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    public void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        ev.IsAllowed = false;

        // Start the animation when click on the button
        Animator animator = ev.Pickup.GameObject.GetComponentInParent<Animator>();
        if (animator != null)
        {
            animator.Play(animator.name + "action");
        }
    }
}