using Content.Client._Crescent.Framework;
using Content.Shared._Crescent.HullrotGunSystem;
using Content.Shared.Interaction;
using Robust.Shared.Map;


namespace Content.Client._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ClientHullrotGunSystem : SharedHullrotGunSystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HullrotGunComponent, UsingMouseDownEvent>(TryUseGun);
    }

    public void onEmptyShootAttempt()
    {

    }

    public void onInvalidShootAttempt()
    {

    }

    public void TryUseGun(Entity<HullrotGunComponent> gun, ref UsingMouseDownEvent args)
    {
        if (!gun.Comp.ammoProvider.getAmmo(out var bullet, out var itemSlot))
        {
            onEmptyShootAttempt();
            return;
        }

        if (!TryComp<HullrotBulletComponent>(bullet, out var chambered))
        {
            onInvalidShootAttempt();
            return;
        }

        fireGun(args.user, gun, args.clickCoords.Position);
        RaiseNetworkEvent(new ClientSideGunFiredEvent()
        {
            aimedPosition = GetNetCoordinates(args.clickCoords),
            gun = GetNetEntity(gun),
            shooter = GetNetEntity(args.user)
        });

    }
}
