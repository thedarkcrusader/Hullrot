using Content.Client._Crescent.Framework;
using Content.Shared._Crescent.HullrotGunSystem;
using Content.Shared.Interaction;
using Robust.Shared.Map;
using Robust.Shared.Timing;


namespace Content.Client._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ClientHullrotGunSystem : SharedHullrotGunSystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HullrotHandheldGunComponent, UsingMouseDownEvent>(TryUseGun);
    }

    public void onEmptyShootAttempt()
    {

    }

    public void onInvalidShootAttempt()
    {

    }

    public void TryUseGun(Entity<HullrotHandheldGunComponent> gun, ref UsingMouseDownEvent args)
    {
        if (!TryComp<HullrotGunComponent>(gun, out var gunComp))
            return;
        if (!gunComp.ammoProvider.getAmmo(out var bullet, out var itemSlot))
        {
            onEmptyShootAttempt();
            return;
        }

        if (!TryComp<HullrotBulletComponent>(bullet, out var chambered))
        {
            onInvalidShootAttempt();
            return;
        }
        if (!_gameTiming.IsFirstTimePredicted)
            return;
        fireGun(args.user, (gun.Owner, gunComp), new EntityCoordinates(args.user, 0, 0), args.clickCoords.Position);
        RaiseNetworkEvent(new ClientSideGunFiredEvent()
        {
            aimedPosition = GetNetCoordinates(args.clickCoords),
            shotFrom = GetNetCoordinates(new EntityCoordinates(args.user, 0, 0)),
            gun = GetNetEntity(gun),
            shooter = GetNetEntity(args.user)
        });

    }
}
