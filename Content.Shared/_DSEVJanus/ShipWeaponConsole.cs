namespace Content.Shared._DSEVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponConsole : Component
{
    public HashSet<EntityUid> linkedHardpoints = new ();

    public Dictionary<string, HashSet<EntityUid>> weaponGroups = new();

    // A limit to how far the turrets can be. anything above won't be controllable
    public float rangeCap = 5000f;
}
