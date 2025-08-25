using Content.Shared._Crescent.HullrotGunSystem;
using Content.Shared.Interaction;


namespace Content.Client._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ClientHullrotGunSystem : SharedHullrotGunSystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<HullrotGunComponent, RangedInteractEvent>(TryUseGun);
    }

    public void onEmptyShootAttempt()
    {

    }

    public void onInvalidShootAttempt()
    {

    }

    public void TryUseGun(Entity<HullrotGunComponent> gun, ref RangedInteractEvent args)
    {
        if (gun.Comp.chambered is null)
        {
            onEmptyShootAttempt();
            return;
        }

        if (!TryComp<HullrotProjectileComponent>(gun.Comp.chambered.Value, out var chambered))
        {
            onInvalidShootAttempt();
            return;
        }

        fireGun(args.UserUid, gun, args.ClickLocation.Position);

    }
}
