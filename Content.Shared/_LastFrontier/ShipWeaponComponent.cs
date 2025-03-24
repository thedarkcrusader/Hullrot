namespace Content.Shared._LastFrontier;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponComponent : Component
{
    [NotSerializable]
    public List<EntityUid> linkedConsoles = new();
}
