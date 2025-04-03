using Robust.Shared.Serialization;


namespace Content.Server._DESVJanus;


/// <summary>
/// This is used for...
/// </summary>
/// Needed because Tuples are very dangerous apparently !!!
[Serializable, NetSerializable]
public class AnglePair
{
    [DataField]
    public Angle first;
    [DataField]
    public Angle second;
}
[RegisterComponent]
public sealed partial class ShipWeaponSafeUseComponent : Component
{
    [DataField]
    public List<AnglePair> safeAngles = new ();

}
