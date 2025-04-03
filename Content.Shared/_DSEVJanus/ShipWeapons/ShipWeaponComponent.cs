using Robust.Shared.GameStates;


namespace Content.Shared._DSEVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShipWeaponComponent : Component
{
    [DataField]
    public HashSet<EntityUid> linkedConsoles = new();
    [AutoNetworkedField, DataField]
    public EntityUid? anchoredTo = null;

}
