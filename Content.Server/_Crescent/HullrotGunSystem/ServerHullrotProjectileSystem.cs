using Content.Shared._Crescent.HullrotGunSystem;


namespace Content.Server._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public sealed class ServerHullrotProjectileSystem : SharedHullrotProjectileSystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {

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
