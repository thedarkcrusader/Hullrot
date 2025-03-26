using System.Numerics;
using Content.Shared._DSEVJanus;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;
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

    public List<AnglePair> generateSafeFiringArcs(Entity<ShipWeaponSafeUseComponent> weapon, float maxRange, int collisionMask)
    {
        List<AnglePair> anglePairs = new List<AnglePair>();
        List<JanusSlice> slices = new List<JanusSlice>();
        foreach (var entity in _lookupSystem.GetEntitiesInRange(
            weapon.Owner,
            maxRange,
            LookupFlags.Dynamic | LookupFlags.Static))
        {
            if(!TryComp<FixturesComponent>(entity, out var fixtures))
                continue;
            if (!TryComp<TransformComponent>(entity, out var transform))
                continue;
            var worldPosition = _transformSystem.GetWorldPosition(transform);
            foreach (var (key, fixture) in fixtures.Fixtures)
            {
                if (!fixture.Hard)
                    continue;
                // Not relevant
                if ((fixture.CollisionMask & collisionMask) == 0)
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

        List<JanusSlice> finalSlices = new();
        for (var i = 0; i < slices.Count; i++)
        {
            for (var j = i; j < slices.Count; j++)
            {
                if (slices[i].overlap(slices[j]) != 0)
                {
                    slices[i].Merge(slices[j]);
                }
            }
        }
        return anglePairs;
    }

    public void onHardpointAnchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponAnchoredEvent args)
    {

    }

    public void onHardpointUnanchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponUnanchoredEvent args)
    {

    }


}
