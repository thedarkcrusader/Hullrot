using Content.Client._Crescent.Framework;
using Content.Client.Projectiles;
using Content.Shared._Crescent.HullrotGunSystem;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Shared.Physics.Components;


namespace Content.Client._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ClientHullrotProjectileSystem : SharedHullrotProjectileSystem
{
    [Dependency] private readonly AnimationPlayerSystem _player = default!;

    public Dictionary<int, Entity<HullrotProjectileComponent>> predictedProjectiles = new Dictionary<int, Entity<HullrotProjectileComponent>>();
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void projectileQueued(Entity<HullrotProjectileComponent> projectile)
    {

    }

    public override void processProjectiles(float deltaTime)
    {
        foreach (var projectile in FireNextTick)
        {
            _transform.SetCoordinates(projectile.Owner, projectile.Comp.initialPosition);
            _physics.SetLinearVelocity(projectile.Owner, projectile.Comp.initialMovement);
        }
    }
}
