using Content.Server.Players.RateLimiting;
using Content.Shared._Crescent.HullrotGunSystem;
using Content.Shared.Players.RateLimiting;
using Robust.Shared.Map;


namespace Content.Server._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ServerHullrotGunSystem : SharedHullrotGunSystem
{
    [Dependency] private readonly PlayerRateLimitManager _playerRateLimitManager = default!;
    public override void Initialize()
    {
        SubscribeNetworkEvent<ClientSideGunFiredEvent>(OnClientFireGun);
    }

    public void OnClientFireGun(ClientSideGunFiredEvent args)
    {
        EntityUid gun = GetEntity(args.gun);
        EntityUid shooter = GetEntity(args.shooter);
        EntityCoordinates targetCoordinates = GetCoordinates(args.aimedPosition);
        if (!TryComp<HullrotGunComponent>(gun, out var gunComp))
            return;
        fireGun(shooter, (gun, gunComp), targetCoordinates.Position);
    }
}
