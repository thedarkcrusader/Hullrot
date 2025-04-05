using Robust.Shared.GameStates;
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
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShipWeaponSafeUseComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<AnglePair> safeAngles = new ();

}
