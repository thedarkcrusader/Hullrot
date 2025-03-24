using Content.Shared.DeviceLinking;
using Robust.Shared.Prototypes;


namespace Content.Shared._DSEVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ShipWeaponHardpointComponent : Component
{
    public EntityUid? anchoredWeapon = null;
    public float turnRate = 0.5f;
    public List<EntityUid> LinkedComputers = new List<EntityUid>();
    // Prevents anyone from building a weapon console anywhere and then hijacking the ship weapons
    // and creating 20000 diplomatic blunders - SPCR 2025
    public int accesCode = 0;
    public ProtoId<SinkPortPrototype> On = "On";
    public ProtoId<SinkPortPrototype> Off = "Off";
    public ProtoId<SinkPortPrototype> FireOnce = "FireOnce";
    public ProtoId<SinkPortPrototype> FireToggle = "FireToggle";
}
