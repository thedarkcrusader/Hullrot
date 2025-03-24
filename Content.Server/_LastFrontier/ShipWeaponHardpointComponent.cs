namespace Content.Server._LastFrontier;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponHardpointComponent : Component
{
    public EntityUid? anchoredWeapon = null;
    public float turnRate = 0.5f;
}
