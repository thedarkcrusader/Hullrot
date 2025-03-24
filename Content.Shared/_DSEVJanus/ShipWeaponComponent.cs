using Robust.Shared.GameStates;


namespace Content.Shared._DSEVJanus;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShipWeaponComponent : Component
{
    public HashSet<EntityUid> linkedConsoles = new();
    [AutoNetworkedField]
    public EntityUid? anchoredTo = null;

}
