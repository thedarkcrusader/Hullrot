namespace Content.Server._DESVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponSafeUseComponent : Component
{
    public List<Tuple<Angle,Angle>> safeAngles = new ();

}
