using System.Runtime.CompilerServices;
using Content.Server._DESVJanus;


namespace Content.Shared._DSEVJanus;

/// <summary>
///  All of these helpers are licensed under MIT for anyone wishing to use them outside of the Janus codebase.
///  Angles are all expected to be counter-clockwise (akin to world Angles) and so is the math around them. - SPCR
///  This is always a positive angle!! and around 0 and 2 PI
/// </summary>
public static class JanusAngle
{
    public static Angle Get(Angle worldAngle)
    {
        worldAngle = worldAngle.Reduced();
        if(worldAngle < 0)
            worldAngle = 2*Math.PI + worldAngle.Theta;
        return worldAngle;
    }
    // returns wheter a rotation should be positive or negative to reach the targetAngle -1 is clock-wise , 1 is counter clock wise
    public static int ClosestTurn(Angle target, Angle starting)
    {
        if (target == starting)
            return 0;
        var acuteAngle = ClosestDifference(target, starting);
        if(acuteAngle > 0)
            return 1;
        return -1;
    }

    // returns the acute angle between 2 angles. Will be positive if counter clockwise , negative other wise.
    public static Angle ClosestDifference(Angle starting, Angle target)
    {
        Angle acute = (target - starting).Reduced();
        if (Math.Abs(acute.Theta) > Math.PI)
        {
           // Logger.Warning($"$Converting {acute.Degrees} degrees to ${(2 * Math.PI - Math.Abs(acute.Theta)) * 180 / Math.PI} !");
            acute = new Angle(2 * Math.PI - Math.Abs(acute.Theta));
        }
    return acute;
    }
    // this ensures the  2 angles are properly put in counter-clockwise
    public static JanusSlice CounterClockSlice(Angle first, Angle second)
    {
        JanusSlice slice = new JanusSlice()
        {
            Angle = first,
            Radius = ClosestDifference(first, second),
        };

        if (slice.Radius < 0)
        {
            slice.Angle = second;
            slice.Radius = ClosestDifference(second, first);
        } // case of wrap-around
        return slice;
    }

    public static Angle PositiveDifference(Angle target, Angle starting)
    {
        var angle = ClosestDifference(target, starting);
        return angle * (angle < 0 ? -1 : 1);
    }
}

// Always counter-clock wise!
public sealed class JanusSlice
{
    public required Angle Angle;
    public required Angle Radius;

    public bool Over360()
    {
        return Angle > (Angle + Radius).Reduced();
    }
    // angle diff to 360 degree point
    public Angle ADT360()
    {
        return new Angle(2*Math.PI - Angle);
    }

    public bool Overlaps(JanusSlice other)
    {
        // special case , both overlap 0-360
        if (Over360())
        {
            if(other.Over360())
                return true;
            if (other.Angle + other.Radius > Angle)
                return true;
        }

        var startingAngleDiff = JanusAngle.ClosestTurn(Angle, other.Angle);
        if (startingAngleDiff == -1)
            return other.Overlaps(this);

        if((Angle + Radius).Reduced() > other.Angle)
            return true;
        return false;
    }

    // returns how many angles one of the radius is inside the other.
    public double OverlapDifference(JanusSlice other)
    {
        if (Over360())
        {
            
        }
        switch (OverlapDirection(other))
        {
            case 1:
                if (Over360())
                {
                    if(other.Over360())
                        return
                }
                return Math.Min(Angle - other.Angle + Radius, other.Radius);
            case -1:
                return other.OverlapDifference(this);
        }

        return 0;

    }
    // this will merge the 2 slices, if they aren't overlapping it'll just add the radius of the other to the first!
    public JanusSlice Merge(JanusSlice other)
    {

        if (other.Angle < Angle)
            return other.Merge(this);
        Radius = Radius + other.Radius - OverlapDifference(other);
        return this;
    }

    public AnglePair ConvertToAnglePair()
    {
        return new AnglePair()
        {
            first = Angle,
            second = (Angle + Radius).Reduced(),
        };
    }
}
