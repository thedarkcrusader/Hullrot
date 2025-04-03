using System.Buffers;
using System.Numerics;
using Content.Shared.Construction.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Shuttles.UI.MapObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using YamlDotNet.Core.Tokens;


namespace Content.Shared._DSEVJanus;

public sealed class ShipWeaponAnchoredEvent : EntityEventArgs
{
    public required EntityUid NewHardpoint;
    public required EntityUid ShipWeapon;
}

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
    [Dependency] public readonly SharedPhysicsSystem _physics = default!;
    [Dependency] public readonly SharedPowerReceiverSystem _receiving = default!;
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

    private void onHardpointAnchorAttempt(Entity<ShipWeaponHardpointComponent> owner, ref AnchorAttemptEvent args)
    {
        var grid = _transformSystem.GetGrid(owner.Owner);
        if (grid is null)
        {
            args.Cancel();
            return;
        }
        var gridComp = Comp<MapGridComponent>(grid.Value);
        if (!_transformSystem.TryGetGridTilePosition(owner.Owner, out var tilePosition, gridComp))
        {
            args.Cancel();
            return;
        }
        if (GetHardpoints(tilePosition, new Entity<MapGridComponent>(grid.Value, gridComp)).Count != 0)
        {
            args.Cancel();
        }
    }
    private void onHardpointUnanchorAttempt(Entity<ShipWeaponHardpointComponent> owner, ref UnanchorAttemptEvent args)
    {
        if(owner.Comp.anchoredWeapon is not null)
            args.Cancel();
    }

    private void onHardpointAnchorStateChanged(Entity<ShipWeaponHardpointComponent> owner, ref AnchorStateChangedEvent args)
    {
        if (args.Anchored)
            return;
        if(owner.Comp.anchoredWeapon is not null && !TerminatingOrDeleted(owner.Comp.anchoredWeapon))
            Unanchor(new Entity<ShipWeaponComponent>(owner.Comp.anchoredWeapon.Value, Comp<ShipWeaponComponent>(owner.Comp.anchoredWeapon.Value)), owner);
    }

    private HashSet<Entity<ShipWeaponHardpointComponent>> GetHardpoints(Vector2i gridPosition, Entity<MapGridComponent> mapGrid)
    {
        HashSet<Entity<ShipWeaponHardpointComponent>> returnList = new();
        foreach (var entity in _mapSystem.GetAnchoredEntities(mapGrid, gridPosition))
        {
            if (!TryComp<ShipWeaponHardpointComponent>(entity, out var hardComp))
                continue;
            returnList.Add(new Entity<ShipWeaponHardpointComponent>(entity, hardComp));
        }
        return returnList;
    }

    private void onWeaponAnchorAttempt(Entity<ShipWeaponComponent> owner, ref AnchorAttemptEvent args)
    {
        if(!AreThereAnyValidHardpoints(owner))
            args.Cancel();
    }

    private void onWeaponAnchorStateChanged(Entity<ShipWeaponComponent> owner, ref AnchorStateChangedEvent args)
    {
        if (args.Anchored)
        {
            if(!TryAnchorToAnyHardpoint(owner))
                _transformSystem.Unanchor(owner.Owner);
            return;
        }

        if (owner.Comp.anchoredTo is null)
            return;
        Unanchor(owner, new Entity<ShipWeaponHardpointComponent>(owner.Comp.anchoredTo.Value, Comp<ShipWeaponHardpointComponent>(owner.Comp.anchoredTo.Value)));
    }

    private void onWeaponInitialize(Entity<ShipWeaponComponent> owner, ref GridInitializeEvent args)
    {
        if (Transform(owner.Owner).Anchored)
        {
            if (!TryAnchorToAnyHardpoint(owner))
            {
                _transformSystem.Unanchor(owner.Owner);
            }
            Log.Error($"SharedShipWeaponSystem initialized a ship weapon called {MetaData(owner.Owner).EntityName} which had no valid hardpoint to anchor to!");
            return;
        }
    }



    private bool TryAnchor(Entity<ShipWeaponComponent> weapon, Entity<ShipWeaponHardpointComponent> anchor)
    {
        if (anchor.Comp.anchoredWeapon is not null)
            return false;
        if (weapon.Comp.anchoredTo is not null)
            return false;
        if (Transform(anchor.Owner).Anchored == false)
            return false;
        if (!TryComp(weapon.Owner, out PhysicsComponent? physics))
            return false;
        Anchor(weapon, anchor);
        return true;
    }

    private void Anchor(Entity<ShipWeaponComponent> weapon, Entity<ShipWeaponHardpointComponent> anchor)
    {
        var physics = Comp<PhysicsComponent>(weapon.Owner);
        weapon.Comp.anchoredTo = anchor.Owner;
        anchor.Comp.anchoredWeapon = weapon.Owner;
        _physics.SetLinearVelocity(weapon.Owner, Vector2.Zero, body: physics);
        _physics.SetBodyType(weapon.Owner, BodyType.Static, body: physics);
        _transformSystem.SetLocalRotation(weapon.Owner, Transform(anchor.Owner).LocalRotation);
        _transformSystem.SetParent(weapon.Owner, anchor.Owner);
        //_transformSystem.AnchorEntity(weapon.Owner);
        ShipWeaponAnchoredEvent ev = new ShipWeaponAnchoredEvent()
        {
            NewHardpoint = anchor.Owner,
            ShipWeapon = weapon.Owner
        };
        RaiseLocalEvent(anchor.Owner, ev);
    }

    private void Unanchor(Entity<ShipWeaponComponent> weapon, Entity<ShipWeaponHardpointComponent> anchor)
    {
        var physics = Comp<PhysicsComponent>(weapon.Owner);
        weapon.Comp.anchoredTo = null;
        anchor.Comp.anchoredWeapon = null;
        _physics.SetBodyType(weapon.Owner, BodyType.Dynamic, body: physics);
        _transformSystem.AttachToGridOrMap(weapon.Owner);
        _transformSystem.Unanchor(weapon.Owner);
        ShipWeaponUnanchoredEvent ev = new ShipWeaponUnanchoredEvent()
        {
            OldHardpoint = anchor.Owner,
            ShipWeapon = weapon.Owner
        };
        RaiseLocalEvent(anchor.Owner, ev);
    }

    private bool AreThereAnyValidHardpoints(Entity<ShipWeaponComponent> owner)
    {
        var grid = _transformSystem.GetGrid(owner.Owner);
        if (grid is null)
            return false;
        var gridComp = Comp<MapGridComponent>(grid.Value);
        if (!_transformSystem.TryGetGridTilePosition(owner.Owner, out var tilePosition, gridComp))
            return false;
        var hardpoints = GetHardpoints(tilePosition, new Entity<MapGridComponent>(grid.Value, gridComp));
        foreach (var hardpoint in hardpoints)
        {
            if(hardpoint.Comp.anchoredWeapon is null)
                return true;
        }
        return false;
    }
    private bool TryAnchorToAnyHardpoint(Entity<ShipWeaponComponent> owner)
    {
        var grid = _transformSystem.GetGrid(owner.Owner);
        if (grid is null)
            return false;
        var gridComp = Comp<MapGridComponent>(grid.Value);
        if (!_transformSystem.TryGetGridTilePosition(owner.Owner, out var tilePosition, gridComp))
            return false;
        var hardpoints = GetHardpoints(tilePosition, new Entity<MapGridComponent>(grid.Value, gridComp));
        foreach (var hardpoint in hardpoints)
        {
            if (TryAnchor(owner, hardpoint))
                return true;
        }
        return false;
    }

}
