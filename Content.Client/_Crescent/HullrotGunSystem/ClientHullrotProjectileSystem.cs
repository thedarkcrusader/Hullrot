using Content.Shared._Crescent.HullrotGunSystem;
using Robust.Client.GameObjects;


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

    }

    public override void processProjectiles(float deltaTime)
    {

    }
}
