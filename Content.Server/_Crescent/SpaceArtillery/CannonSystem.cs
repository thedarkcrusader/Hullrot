using System.Linq;
using Content.Server._Crescent.SpaceArtillery;
using Content.Shared.Interaction;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using JetBrains.Annotations;


namespace Content.Server._Crescent;


/// <summary>
/// This handles...
/// </summary>
public sealed class CannonSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CannonComponent, InteractEvent>(onInteract);
        SubscribeLocalEvent<CannonComponent, GunShotEvent>(afterShoot);
        SubscribeLocalEvent<CannonComponent, ShotAttemptedEvent>(onAttempt);
    }

    public void afterShoot(Entity<CannonComponent> cannon, GunShotEvent ev)
    {
        foreach (var (Id, Shoot) in ev.Ammo)
        {
            if (TerminatingOrDeleted(Id))
                continue;
            if (!TryComp<ProjectileComponent>(Id, out var projectile))
                continue;
        }
    }

    public void onAttempt(Entity<CannonComponent> cannon, ShotAttemptedEvent ev)
    {
        if (HasPropellant(cannon) && HasAmmoLast(cannon))
            return;
        ev.Cancel();
    }

    public bool TryLoadAmmo(Entity<CannonComponent> cannon, Entity<CannonAmmoComponent> cannonAmmo, EntityUid user)
    {
        if (!HasPropellant(cannon))
            return false;
        return false;
    }

    public bool TryLoadPropellant(Entity<CannonComponent> cannon, Entity<CannonPropellantComponent> cannonPropellant, EntityUid user)
    {
        if (HasAmmoLast(cannon))
            return false;
        return false;
    }

    public bool HasPropellant(Entity<CannonComponent> cannon)
    {
        foreach (EntityUid loaded in cannon.Comp.loadedEntities)
        {
            if (!TryComp<CannonPropellantComponent>(loaded, out var cannonPropellant))
                continue;
            return true;
        }

        return false;
    }

    public bool HasAmmoLast(Entity<CannonComponent> cannon)
    {
        if (TryComp<CannonAmmoComponent>(cannon.Comp.loadedEntities.Last(), out var cannonAmmo))
            return true;
        return false;

    }

    public void onInteract(Entity<CannonComponent> cannon, ref InteractEvent args)
    {
        if (args.Handled)
            return;
        if (TryComp<CannonAmmoComponent>(args.Used, out var cannonAmmo) && TryLoadAmmo(cannon, (args.Used, cannonAmmo), args.User))
            args.Handled = true;
        else if (TryComp<CannonPropellantComponent>(args.Used, out var cannonPropellant) && TryLoadPropellant(cannon, (args.Used, cannonPropellant), args.User))
            args.Handled = true;
    }

}
