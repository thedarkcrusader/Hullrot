using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;


namespace Content.Shared._Crescent.HullrotGunSystem;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class HullrotGunComponent : Component
{
    // bullet sets their own speed , gun can only influence it
    public float SpeedMultiplier = 1;

    // bullets per second
    public float FireRate = 6;
    public int shootingPosIndex = 0;
    public List<Vector2> shootingPosOffsets = new List<Vector2>();
    public HullrotGunProviderComponent ammoProvider;

    public Vector2 getShootingOffset()
    {
        if (shootingPosIndex == shootingPosOffsets.Count)
            shootingPosIndex = 0;
        return shootingPosOffsets[shootingPosIndex++];
    }
};

public abstract partial class HullrotGunProviderComponent : Component
{
    public abstract bool getAmmo([NotNullWhen(true)] out EntityUid? ammo,  out ItemSlot slot);
};

[RegisterComponent]
public partial class HullrotGunAmmoChamberComponent : HullrotGunProviderComponent
{
    /// Hullrot additions
    [DataField("bulletSlot")]
    public ItemSlot bulletSlot = new();

    public override bool getAmmo([NotNullWhen(true)] out EntityUid? ammo, out ItemSlot slot)
    {
        ammo = bulletSlot.Item;
        slot = bulletSlot;
        return bulletSlot.HasItem;
    }
}
[RegisterComponent]
public sealed partial class HullrotGunAmmoMagazineChamberComponent : HullrotGunProviderComponent
{
    [DataField("bulletSlot")]
    public ItemSlot magazineSlot = new();
    public override bool getAmmo([NotNullWhen(true)] out EntityUid? ammo,  out ItemSlot slot)
    {
        ammo = magazineSlot.Item;
        slot = magazineSlot;
        return magazineSlot.HasItem;
    }
}

[RegisterComponent]
public sealed partial class HullrotBulletComponent : Component
{
    // meters per second.
    public float Speed = 100;
    public EntityPrototype projectileEntity;
}

[RegisterComponent]
public sealed partial class HullrotMagazineComponent : Component
{
    public ItemSlot topBulletSlot = new();
    public Queue<EntityUid> loadedBullets = new();
}

