using System.Collections.Generic;
using AutoEvent.API.AdvancedMERTool.Objects;
using AutoEvent.API.AdvancedMERTool.Serializable;
using AutoEvent.Events.EventArgs;
using InventorySystem.Items.Pickups;

namespace AutoEvent.API.AdvancedMERTool;
public class EventManager
{
    public void OnItemSearching(SearchPickUpItemArgs ev)
    {
        List<InteractablePickupObject> list = AdvancedMERTools.Singleton.InteractablePickups.FindAll(x => x.Pickup == ev.Pickup);
        List<ItemPickupBase> removeList = new List<ItemPickupBase> { };
        foreach (InteractablePickupObject interactable in list)
        {
            if (interactable.Base.InvokeType.HasFlag(InvokeType.Searching))
            {
                interactable.RunProcess(ev.Player, ev.Pickup, out bool Remove);
                if (interactable.Base.CancelActionWhenActive)
                {
                    ev.IsAllowed = false;
                }
                if (Remove && !removeList.Contains(interactable.Pickup))
                {
                    removeList.Add(interactable.Pickup);
                }
            }
        }
        removeList.ForEach(x => x.DestroySelf());
    }
}