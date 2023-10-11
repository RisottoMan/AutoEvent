using AutoEvent.Events.EventArgs;
using System;

namespace AutoEvent.Events.Handlers
{
    internal class Players
    {
        public static event Action<DropAmmoArgs> DropAmmo;
        public static event Action<DropItemArgs> DropItem;
        public static event Action<PlaceTantrumArgs> PlaceTantrum;
        public static event Action<PlayerDamageArgs> PlayerDamage;
        public static event Action<PlayerDyingArgs> PlayerDying;
        public static event Action<HandCuffArgs> HandCuff;
        public static event Action<LockerInteractArgs> LockerInteract;
        public static event Action<PlayerNoclipArgs> PlayerNoclip;
        public static void OnDropAmmo(DropAmmoArgs ev) => DropAmmo?.Invoke(ev);
        public static void OnDropItem(DropItemArgs ev) => DropItem?.Invoke(ev);
        public static void OnPlaceTantrum(PlaceTantrumArgs ev) => PlaceTantrum?.Invoke(ev);
        public static void OnPlayerDamage(PlayerDamageArgs ev) => PlayerDamage?.Invoke(ev);
        public static void OnPlayerDying(PlayerDyingArgs ev) => PlayerDying?.Invoke(ev);
        public static void OnHandCuff(HandCuffArgs ev) => HandCuff?.Invoke(ev);
        public static void OnLockerInteract(LockerInteractArgs ev) => LockerInteract?.Invoke(ev);
        public static void OnPlayerNoclip(PlayerNoclipArgs ev) => PlayerNoclip?.Invoke(ev);
    }
}
