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
        public static event Action<SwingingJailbirdEventArgs> SwingingJailbird;
        public static event Action<ChargingJailbirdEventArgs> ChargingJailbird;
        public static event Action<ShotEventArgs> Shot;
        public static event Action<PickUpItemArgs> PickUpItem;
        public static event Action<SearchPickUpItemArgs> SearchPickUpItem;
        public static event Action<PlayerJumpArgs> PlayerJump;
        public static event Action<UsingStaminaArgs> UsingStamina;
        public static void OnDropAmmo(DropAmmoArgs ev) => DropAmmo?.Invoke(ev);
        public static void OnDropItem(DropItemArgs ev) => DropItem?.Invoke(ev);
        public static void OnPlaceTantrum(PlaceTantrumArgs ev) => PlaceTantrum?.Invoke(ev);
        public static void OnPlayerDamage(PlayerDamageArgs ev) => PlayerDamage?.Invoke(ev);
        public static void OnPlayerDying(PlayerDyingArgs ev) => PlayerDying?.Invoke(ev);
        public static void OnHandCuff(HandCuffArgs ev) => HandCuff?.Invoke(ev);
        public static void OnLockerInteract(LockerInteractArgs ev) => LockerInteract?.Invoke(ev);
        public static void OnPlayerNoclip(PlayerNoclipArgs ev) => PlayerNoclip?.Invoke(ev);
        public static void OnSwingingJailbird(SwingingJailbirdEventArgs ev) => SwingingJailbird?.Invoke(ev);
        public static void OnChargingJailbird(ChargingJailbirdEventArgs ev) => ChargingJailbird?.Invoke(ev);
        public static void OnShot(ShotEventArgs ev) => Shot?.Invoke(ev);
        public static void OnPickUpItem(PickUpItemArgs ev) => PickUpItem?.Invoke(ev);
        public static void OnSearchPickUpItem(SearchPickUpItemArgs ev) => SearchPickUpItem?.Invoke(ev);
        public static void OnPlayerJump(PlayerJumpArgs ev) => PlayerJump?.Invoke(ev);
        public static void OnUsingStamina(UsingStaminaArgs ev) => UsingStamina?.Invoke(ev);
    }
}
