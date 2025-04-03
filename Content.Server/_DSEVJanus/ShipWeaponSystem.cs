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
                topPoints = objectPos - topPoints;
                bottomPoints = objectPos - bottomPoints;
                List<JanusAngle> angles = new List<JanusAngle>();
                angles.Add(new JanusAngle(new Vector2(topPoints.X, topPoints.Y).ToWorldAngle()));
                angles.Add(new JanusAngle(new Vector2(topPoints.Z, topPoints.W).ToWorldAngle()));
                angles.Add(new JanusAngle(new Vector2(bottomPoints.X, bottomPoints.Y).ToWorldAngle()));
                angles.Add(new JanusAngle(new Vector2(bottomPoints.Z, bottomPoints.W).ToWorldAngle()));
                Logger.Warning($"Angle 1 is : {angles[0].Angle}");
                Logger.Warning($"Angle 2 is : {angles[1].Angle}");
                Logger.Warning($"Angle 3 is : {angles[2].Angle}");
                Logger.Warning($"Angle 4 is : {angles[3].Angle}");
                JanusAngle biggestStart = new JanusAngle(Angle.Zero);
                JanusAngle biggestRadius = new JanusAngle(Angle.Zero);
                foreach (var angle in angles)
                {
                    foreach (var others in angles)
                    {
                        if (angle - others > biggestRadius)
                        {
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
            35,
            CollisionGroup.BulletImpassable);
    }

    public void onHardpointUnanchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponUnanchoredEvent args)
    {

    }


}
