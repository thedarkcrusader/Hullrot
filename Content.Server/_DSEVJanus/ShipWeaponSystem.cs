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
                var box = fixture.Shape.ComputeAABB(_physics.GetPhysicsTransform(entity, transform), 0);
                //Logger.Warning($"Calculated box bounds");
                //Logger.Warning($"Top left : {box.Left} {box.Top} , Right : {box.Right} {box.Top}");
                //Logger.Warning($"Bottom right : {box.Left} {box.Bottom} , Right : {box.Right} {box.Bottom}");
                Vector4 topPoints = new Vector4(box.Left, box.Top, box.Right, box.Top);
                Vector4 bottomPoints = new Vector4(box.Right, box.Bottom, box.Left, box.Bottom);
                Vector4 objectPos = new Vector4(worldPosition.X, worldPosition.Y, worldPosition.X, worldPosition.Y);
                //Logger.Warning($"Object pos is {worldPosition.X} {worldPosition.Y}");
                objectPos = weaponPosSubstractor - objectPos;
                //Logger.Warning($"Obtained relative pos {objectPos.X}, {objectPos.Y} ");
                //Logger.Warning($"substracted pos is {objectPos.X} {objectPos.Y}");
                topPoints -= weaponPosSubstractor;
                //Logger.Warning($"Top Points 1 : {topPoints.X}, {topPoints.Y}, 2 : {topPoints.Z}, {topPoints.W}");
                //Logger.Warning($"Top points are :  {topPoints.X} {topPoints.Y}  and {topPoints.Z} {topPoints.W}");
                bottomPoints -= weaponPosSubstractor;
                //Logger.Warning($"Bottom point 1 : {bottomPoints.X}, {bottomPoints.Y} 2 {bottomPoints.Z}, {bottomPoints.W}");
                //Logger.Warning($"Bottom points are :  {bottomPoints.X} {bottomPoints.Y}  and {bottomPoints.Z} {bottomPoints.W}");
                var angles = new List<Angle>();
                angles.Add(JanusAngle.Get(Angle.FromWorldVec(new Vector2(topPoints.X, topPoints.Y)).Reduced()));
                angles.Add(JanusAngle.Get(Angle.FromWorldVec(new Vector2(topPoints.Z, topPoints.W)).Reduced()));
                angles.Add(JanusAngle.Get(Angle.FromWorldVec(new Vector2(bottomPoints.X, bottomPoints.Y)).Reduced()));
                angles.Add(JanusAngle.Get(Angle.FromWorldVec(new Vector2(bottomPoints.Z, bottomPoints.W)).Reduced()));
                Angle biggestStart = JanusAngle.Get(Angle.Zero);
                Angle biggestEnd = JanusAngle.Get(Angle.Zero);
                Angle biggestRadius = JanusAngle.Get(Angle.Zero);
                JanusSlice biggestSlice = null;
                /*
                foreach (var angle in angles)
                {
                    Logger.Warning($"Angle : {angle.Degrees}");
                }
                */
                foreach (var angle in angles)
                {
                    foreach (var others in angles)
                    {
                        //if (JanusAngle.ClosestTurn(others, angle) < 0)
                        //    continue;
                        Logger.Warning($"Comparing {(others - angle).Degrees}");
                        if (JanusAngle.ClosestDifference(angle, others)> biggestRadius)
                        {
                            biggestRadius = JanusAngle.ClosestDifference(angle, others);
                            Logger.Warning($"$Switched to {biggestRadius.Degrees} !");
                            biggestSlice = (JanusAngle.CounterClockSlice(others, angle));
                            biggestStart = others;
                            biggestEnd = angle;
                        }
                    }
                }
                //Logger.Warning($"Obtained slice  , starting at {biggestStart.Degrees} and a radius of {biggestRadius.Degrees}");
                anglePairs.Add(new AnglePair(){first = biggestStart, second = biggestEnd});
            }

        }

        return anglePairs;
    }

    public void onHardpointAnchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponAnchoredEvent args)
    {
        var comp = EnsureComp<ShipWeaponSafeUseComponent>(args.ShipWeapon);
        comp.safeAngles = generateSafeFiringArcs(new Entity<ShipWeaponSafeUseComponent>(args.ShipWeapon, comp),
            5,
            CollisionGroup.Impassable);
        Dirty(args.ShipWeapon, comp);
    }

    public void onHardpointUnanchorWeapon(Entity<ShipWeaponHardpointComponent> owner, ref ShipWeaponUnanchoredEvent args)
    {

    }


}
