namespace Content.Server._DESVJanus;


/// <summary>
/// This is used for...
/// </summary>
/// Needed because Tuples are very dangerous apparently !!!
public class AnglePair
{
    public Angle first;
    public Angle second;
}
[RegisterComponent]
public sealed partial class ShipWeaponSafeUseComponent : Component
{
    public class AnglePair
    {
        public Angle first;
        public Angle second;
    }
    public List<AnglePair> safeAngles = new ();

}
