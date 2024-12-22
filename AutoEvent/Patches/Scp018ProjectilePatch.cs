using AutoEvent.Events;
using AutoEvent.Events.EventArgs;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace AutoEvent.Patches;
[HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.Update))]
internal static class Scp018ProjectilePatch
{
    public static void Postfix(Scp018Projectile __instance)
    {
        Scp018UpdateArgs scp018UpdateEvent = new Scp018UpdateArgs(__instance);
        Handlers.OnScp018Update(scp018UpdateEvent);
    }
}

[HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.ProcessCollision))]
internal static class Scp018ProjectileBouncePatch
{
    public static void Postfix(Scp018Projectile __instance, Collision collision)
    {
        Scp018CollisionArgs scp018CollisionEvent = new Scp018CollisionArgs(__instance);
        Handlers.OnScp018Collision(scp018CollisionEvent);
    }
}