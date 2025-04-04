using System.Numerics;
using Content.Shared._DSEVJanus;
using Content.Shared.Physics;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Vector4 = Robust.Shared.Maths.Vector4;


namespace Content.Server._DESVJanus;


/// <summary>
/// This handles...
/// </summary>
public sealed class ShipWeaponSystem : SharedShipWeaponSystem
{
    [Dependency] private readonly PhysicsSystem _physics = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ShipWeaponHardpointComponent,ShipWeaponAnchoredEvent>(onHardpointAnchorWeapon);
        SubscribeLocalEvent<ShipWeaponHardpointComponent,ShipWeaponUnanchoredEvent>(onHardpointUnanchorWeapon);
    }

    public List<AnglePair> generateSafeFiringArcs(Entity<ShipWeaponSafeUseComponent> weapon, float maxRange, CollisionGroup collisionMask)
    {
        List<AnglePair> anglePairs = new List<AnglePair>();
        List<JanusSlice> slices = new List<JanusSlice>();
        var weaponPosition = _transformSystem.GetWorldPosition(weapon.Owner);
        Vector4 weaponPosSubstractor = new Vector4(
            weaponPosition.X,
            weaponPosition.Y,
            weaponPosition.X,
            weaponPosition.Y);
        foreach (var entity in _lookupSystem.GetEntitiesInRange(
            weapon.Owner,
            maxRange,
           LookupFlags.Static))
        {
            if(!TryComp<FixturesComponent>(entity, out var fixtures))
                continue;
            var transform = Transform(entity);
            var worldPosition = _transformSystem.GetWorldPosition(transform);
            foreach (var (key, fixture) in fixtures.Fixtures)
            {
                if (!fixture.Hard)
                    continue;
                // Not relevant
                if ((fixture.CollisionMask & (int)collisionMask) == 0)
                    continue;
                // the child index is just a leftover for some reason - SPCR 2025
                var box = fixture.Shape.ComputeAABB(_physics.GetLocalPhysicsTransform(entity, transform), 0);
                Vector4 topPoints = new Vector4(box.Left, box.Top, box.Right, box.Top);
                Vector4 bottomPoints = new Vector4(box.Right, box.Bottom, box.Left, box.Bottom);
                Vector4 objectPos = new Vector4(worldPosition.X, worldPosition.Y, worldPosition.X, worldPosition.Y);
                //Logger.Warning($"Object pos is {worldPosition.X} {worldPosition.Y}");
                objectPos = weaponPosSubstractor - objectPos;
                //Logger.Warning($"substracted pos is {objectPos.X} {objectPos.Y}");
                topPoints = objectPos + topPoints;
                //Logger.Warning($"Top points are :  {topPoints.X} {topPoints.Y}  and {topPoints.Z} {topPoints.W}");
                bottomPoints = objectPos + bottomPoints;
                //Logger.Warning($"Bottom points are :  {bottomPoints.X} {bottomPoints.Y}  and {bottomPoints.Z} {bottomPoints.W}");
                var angles = new List<JanusAngle>();
                angles.Add(new JanusAngle(Angle.FromWorldVec(new Vector2(topPoints.X, topPoints.Y)).Reduced()));
                angles.Add(new JanusAngle(Angle.FromWorldVec(new Vector2(topPoints.Z, topPoints.W)).Reduced()));
                angles.Add(new JanusAngle(Angle.FromWorldVec(new Vector2(bottomPoints.X, bottomPoints.Y)).Reduced()));
                angles.Add(new JanusAngle(Angle.FromWorldVec(new Vector2(bottomPoints.Z, bottomPoints.W)).Reduced()));
                JanusAngle biggestStart = new JanusAngle(Angle.Zero);
                JanusAngle biggestRadius = new JanusAngle(Angle.Zero);
                foreach (var angle in angles)
                {
                    foreach (var others in angles)
                    {
                        if (angle.ClosestTurn(others) <= 0)
                            continue;
                        if (angle - others > biggestRadius)
                        {
                            Logger.Warning($"Set to new slice , starting at {biggestStart.Angle} , and a radius of {biggestRadius.Angle}" );
                            biggestRadius = angle - others;
                            biggestStart = others;
                        }
                    }
                }
                slices.Add(new JanusSlice(){Angle = biggestStart, Radius = biggestRadius});
            }

        }

        var i = 0;
        var j = 0;
        while (i < slices.Count)
        {
            j = i+1;
            while (j < slices.Count)
            {
                if (slices[i].overlap(slices[j]) != 0)
                {
                    slices[i].Merge(slices[j]);
                    slices.RemoveAt(j);
                    continue;
                }
                j++;
            }

            i++;
        }
        foreach (var slice in slices)
            anglePairs.Add(new AnglePair(){first = slice.Angle.Angle, second = (slice.Angle + slice.Radius).Angle});
        return anglePairs;
    }

    public void onHardpointAnchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponAnchoredEvent args)
    {
        var comp = EnsureComp<ShipWeaponSafeUseComponent>(args.ShipWeapon);
        comp.safeAngles = generateSafeFiringArcs(new Entity<ShipWeaponSafeUseComponent>(args.ShipWeapon, comp),
            5,
            CollisionGroup.Impassable);
    }

    public void onHardpointUnanchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponUnanchoredEvent args)
    {

    }


}
