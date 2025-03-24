using Robust.Shared.GameStates;


namespace Content.Shared._LastFrontier;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShipWeaponComponent : Component
{
    public List<EntityUid> linkedConsoles = new();
    [AutoNetworkedField]
    public EntityUid? anchoredTo = null;

}
