using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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
    public EntityUid? chambered = null;
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
    public abstract bool getAmmo([NotNullWhen(true)] out EntityUid? ammo);
};

[RegisterComponent]
public partial class HullrotGunAmmoChamberComponent : HullrotGunProviderComponent
{
    public EntityUid? bullet;

    public override bool getAmmo([NotNullWhen(true)] out EntityUid? ammo)
    {
        ammo = bullet;
        return bullet is not null;
    }
}
[RegisterComponent]
public sealed partial class HullrotGunAmmoMagazineComponent : HullrotGunProviderComponent
{
    public Queue<EntityUid> bullets = new Queue<EntityUid>();
    public override bool getAmmo([NotNullWhen(true)] out EntityUid? ammo)
    {
        if (bullets.Count == 0)
        {
            ammo = null;
            return false;
        }
        ammo = bullets.Dequeue();
        return true;
    }
}

[RegisterComponent]
public sealed partial class HullrotBulletComponent : Component
{
    // meters per second.
    public float Speed = 100;
    public EntityPrototype projectileEntity;

}

