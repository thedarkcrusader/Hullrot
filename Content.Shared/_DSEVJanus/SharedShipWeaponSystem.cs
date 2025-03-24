using Content.Shared.Construction.Components;
using Content.Shared.Shuttles.UI.MapObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Serialization;


namespace Content.Shared._DSEVJanus;

[Serializable, NetSerializable]
public sealed class ShipWeaponAnchoredEvent : EntityEventArgs
{
    public required EntityUid NewHardpoint;
    public required EntityUid ShipWeapon;
}

[Serializable, NetSerializable]
public sealed class ShipWeaponUnanchoredEvent : EntityEventArgs
{
    public required EntityUid OldHardpoint;
    public required EntityUid ShipWeapon;
}

/// <summary>
/// This handles...
/// </summary>
public class SharedShipWeaponSystem : EntitySystem
{
    [Dependency] public readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] public readonly EntityLookupSystem _lookupSystem = default!;
    [Dependency] public readonly SharedMapSystem _mapSystem = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ShipWeaponHardpointComponent, AnchorAttemptEvent>(onHardpointAnchorAttempt);
        SubscribeLocalEvent<ShipWeaponHardpointComponent, UnanchorAttemptEvent>(onHardpointUnanchorAttempt);
        SubscribeLocalEvent<ShipWeaponHardpointComponent, AnchorStateChangedEvent>(onHardpointAnchorStateChanged);
        SubscribeLocalEvent<ShipWeaponComponent, AnchorAttemptEvent>(onWeaponAnchorAttempt);
        SubscribeLocalEvent<ShipWeaponComponent, AnchorStateChangedEvent>(onWeaponAnchorStateChanged);
        SubscribeLocalEvent<ShipWeaponComponent, GridInitializeEvent>(onWeaponInitialize);
    }

    private List<Entity<ShipWeaponHardpointComponent>> getAvailableHardpoints(Vector2i gridPosition, Entity<MapGridComponent> mapGrid)
    {
        List<Entity<ShipWeaponHardpointComponent>> returnList = new();
        foreach (var entity in _mapSystem.GetAnchoredEntities(mapGrid, gridPosition))
        {
            if (!TryComp<ShipWeaponHardpointComponent>(entity, out var hardComp))
                continue;
            if (hardComp.anchoredWeapon is not null)
                continue;
            returnList.Add(new Entity<ShipWeaponHardpointComponent>(entity, hardComp));
        }
        return returnList;
        _
    }

}
