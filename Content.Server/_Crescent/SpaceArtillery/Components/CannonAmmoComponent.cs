namespace Content.Server._Crescent.SpaceArtillery;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CannonAmmoComponent : Component
{
    // a total load above this speed will cause the ammo to fragment
    [DataField]
    public float MaxSpeed = 400;
    // Extra Penetration added by bullet speed.
    [DataField]
    public float SpeedToPenetrationRatio = 0.8f;
    [DataField]
    public int FragmentCount = 10;
    [DataField]
    public float FragmentDamageMultiplier = 0.4f;
    [DataField]
    public float FragmentPenMultiplier = 0.1f;
}
