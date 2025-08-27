using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
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
    [Dependency] private readonly SharedHullrotProjectileSystem _projectileSystem = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    private ISawmill _sawmill = default!;

    private const string ammoChamberContainerName = "Hullrot_Ammo_Chamber";
    public bool getProjectile(EntityUid shooter, Entity<HullrotGunComponent> gun, Entity<HullrotBulletComponent> bullet,[NotNullWhen(true)] out Entity<HullrotProjectileComponent>? outputComp)
    {
        outputComp = null;
        EntityUid projectile = Spawn(bullet.Comp.projectileEntity.ToString(), MapCoordinates.Nullspace);
        if (!TryComp<HullrotProjectileComponent>(projectile, out var projectileComp))
            return false;
        projectileComp.firedFrom = gun.Owner;
        projectileComp.shotBy = shooter;
        projectileComp.initialMovement = new Vector2(bullet.Comp.Speed * gun.Comp.SpeedMultiplier, bullet.Comp.Speed * gun.Comp.SpeedMultiplier);
        projectileComp.initialPosition = new EntityCoordinates(gun.Owner, 0, 0);
        outputComp = (projectile, projectileComp);
        return true;
    }

    public bool getProjectileChambered(EntityUid shooter, Entity<HullrotGunComponent> gun,[NotNullWhen(true)] out Entity<HullrotProjectileComponent>? outputComp)
    {
        outputComp = null;
        if (!gun.Comp.ammoProvider.getAmmo(out var chambered, out var slot))
            return false;
        if (!TryComp<HullrotBulletComponent>(chambered, out var bulletComp))
            return false;
        EntityUid projectile;
        if(!IsClientSide()
            projectile = bulletComp.projectileEntity.ToString(), MapCoordinates.Nullspace);
        _itemSlotsSystem.TryEject(gun, slot, null, out var ejected);
        var projectileComp = EnsureComp<HullrotProjectileComponent>(projectile);
        projectileComp.firedFrom = gun.Owner;
        projectileComp.shotBy = shooter;
        projectileComp.initialMovement = new Vector2(bulletComp.Speed * gun.Comp.SpeedMultiplier, bulletComp.Speed * gun.Comp.SpeedMultiplier);
        outputComp = (projectile, projectileComp);
        return true;
    }

    public void fireGun(EntityUid shooter, Entity<HullrotGunComponent> gun, EntityCoordinates shootingFrom, Vector2 targetPos)
    {
        if(!getProjectileChambered(shooter, gun, out var projectileNullable))
            return;
        var map = _transformSystem.GetMapId(gun.Owner);
        MapCoordinates mapCoords = new MapCoordinates(_transformSystem.GetWorldPosition(gun.Owner) + targetPos, map);
        Entity<HullrotProjectileComponent> projectile = projectileNullable.Value;
        projectile.Comp.initialPosition = shootingFrom;
        if (_mapManager.TryFindGridAt(mapCoords, out var gridUid, out var gridComp))
        {
            Vector2i tileIndices = _mapSystem.CoordinatesToTile(gridUid, gridComp, mapCoords);
            projectile.Comp.aimedPosition = new EntityCoordinates(gridUid, tileIndices);
        }
        else
        {
            projectile.Comp.aimedPosition = _transformSystem.ToCoordinates(mapCoords);
        }
        _projectileSystem.queueProjectile(projectile);
    }

    public bool tryGetProvider(EntityUid from,[NotNullWhen(true)] out HullrotGunProviderComponent? provider)
    {
        provider = null;
        if (TryComp<HullrotGunAmmoChamberComponent>(from, out var chambered))
        {
            provider = chambered;
            return true;
        }
        if (TryComp<HullrotGunAmmoMagazineChamberComponent>(from, out var magazine))
        {
            provider = magazine;
            return true;
        }
        return false;
    }

    public void onGunInitialized(Entity<HullrotGunComponent> gun, ref ComponentInit args)
    {
        if (!tryGetProvider(gun.Owner, out var provider))
        {
            _sawmill.Error($"Gun prototype {MetaData(gun.Owner).EntityPrototype} does not have any GunAmmo components to fetch ammo from!");
            return;
        }
        gun.Comp.ammoProvider = provider;
    }

    public void onChamberInitialized(Entity<HullrotGunAmmoChamberComponent> chamber, ref ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(chamber.Owner, ammoChamberContainerName, chamber.Comp.bulletSlot);
    }

    public override void Initialize()
    {
        SubscribeLocalEvent<HullrotGunComponent, ComponentInit>(onGunInitialized);
        SubscribeLocalEvent<HullrotGunAmmoChamberComponent, ComponentInit>(onChamberInitialized);
        _sawmill = _logManager.GetSawmill("HullrotGunSystem");
    }
}
