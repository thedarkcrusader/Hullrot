using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Robust.Shared.Map;


namespace Content.Shared._Crescent.HullrotGunSystem;


/// <summary>
/// This handles...
/// </summary>
public abstract class SharedHullrotGunSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    public bool getProjectile(EntityUid shooter, Entity<HullrotGunComponent> gun, Entity<HullrotBulletComponent> bullet,[NotNullWhen(true)] out Entity<HullrotProjectileComponent>? outputComp)
    {
        outputComp = null;
        EntityUid projectile = Spawn(bullet.Comp.projectileEntity.ToString(), MapCoordinates.Nullspace);
        if (!TryComp<HullrotProjectileComponent>(projectile, out var projectileComp))
            return false;
        projectileComp.firedFrom = gun.Owner;
        projectileComp.shotBy = shooter;
        projectileComp.initialMovement = new Vector2(bullet.Comp.Speed * gun.Comp.SpeedMultiplier, bullet.Comp.Speed * gun.Comp.SpeedMultiplier);
        outputComp = (projectile, projectileComp);
        return true;
    }
    public bool getProjectileChambered(EntityUid shooter, Entity<HullrotGunComponent> gun,[NotNullWhen(true)] out Entity<HullrotProjectileComponent>? outputComp)
    {
        outputComp = null;
        if (gun.Comp.chambered is null)
            return false;
        if (!TryComp<HullrotBulletComponent>(gun.Comp.chambered, out var bulletComp))
            return false;
        EntityUid projectile = Spawn(bulletComp.projectileEntity.ToString(), MapCoordinates.Nullspace);
        if (!TryComp<HullrotProjectileComponent>(projectile, out var projectileComp))
            return false;
        projectileComp.firedFrom = gun.Owner;
        projectileComp.shotBy = shooter;
        projectileComp.initialMovement = new Vector2(bulletComp.Speed * gun.Comp.SpeedMultiplier, bulletComp.Speed * gun.Comp.SpeedMultiplier);
        outputComp = (projectile, projectileComp);
        return true;
    }

    public void fireProjectile(EntityUid shooter, Entity<HullrotGunComponent> gun, Vector2 targetPos)
    {
        if(!getProjectileChambered(shooter, gun, out var projectileNullable))
            return;
        var map = _transformSystem.GetMapId(gun.Owner);
        MapCoordinates mapCoords = new MapCoordinates(_transformSystem.GetWorldPosition(gun.Owner) + targetPos, map);
        Entity<HullrotProjectileComponent> projectile = projectileNullable.Value;
        if (_mapManager.TryFindGridAt(mapCoords, out var gridUid, out var gridComp))
        {
            Vector2i tileIndices = _mapSystem.CoordinatesToTile(gridUid, gridComp, mapCoords);
            projectile.Comp.aimedPosition = new EntityCoordinates(gridUid, tileIndices);
        }
        else
        {
            projectile.Comp.aimedPosition = _transformSystem.ToCoordinates(mapCoords);
        }
    }w

    public override void Initialize()
    {

    }
}
