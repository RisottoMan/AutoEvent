using Exiled.API.Features;
using InventorySystem.Items;

namespace AutoEvent.Events.EventArgs;

/// <summary>
/// Contains all information before a player swings a <see cref="Exiled.API.Features.Items.Jailbird"/>.
/// </summary>
public class SwingingJailbirdEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwingingJailbirdEventArgs"/> class.
    /// </summary>
    /// <param name="player"><inheritdoc cref="Player"/></param>
    /// <param name="swingItem">The item being swung.</param>
    /// <param name="isAllowed">Whether the item can be swung or not.</param>
    public SwingingJailbirdEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase swingItem, bool isAllowed = true)
    {
        Player = Player.Get(player);
        Item = swingItem;
        IsAllowed = isAllowed;
    }

    /// <summary>
    /// Gets the <see cref="API.Features.Player"/> who's swinging an item.
    /// </summary>
    public Player Player { get; }

    /// <summary>
    /// Gets the <see cref="API.Features.Items.Item"/> that is being swung.
    /// </summary>
    public ItemBase Item { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the item can be swung.
    /// </summary>
    public bool IsAllowed { get; set; }
}