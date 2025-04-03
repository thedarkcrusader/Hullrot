using Content.Shared.DeviceLinking;
using Robust.Shared.Prototypes;


namespace Content.Shared._DSEVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponHardpointComponent : Component
{
    [DataField]
    public EntityUid? anchoredWeapon = null;
    [DataField]
    public float turnRate = 0.5f;
    [DataField]
    public HashSet<EntityUid> LinkedComputers = new ();
    // Prevents anyone from building a weapon console anywhere and then hijacking the ship weapons
    // and creating 20000 diplomatic blunders - SPCR 2025
    [DataField]
    public int accesCode = 0;
    [DataField]
    public ProtoId<SinkPortPrototype> On = "On";
    [DataField]
    public ProtoId<SinkPortPrototype> Off = "Off";
    [DataField]
    public ProtoId<SinkPortPrototype> FireOnce = "FireOnce";
    [DataField]
    public ProtoId<SinkPortPrototype> FireToggle = "FireToggle";
}
