using System.Runtime.CompilerServices;


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

    // returns the acute angle between 2 angles. Will be positive if counter clockwise , negative other wise
    public static Angle ClosestDifference(Angle target, Angle starting)
    {
        return (target - starting).Reduced();
    }

    public static Angle PositiveDifference(Angle target, Angle starting)
    {
        var angle = ClosestDifference(target, starting);
        return angle * (angle < 0 ? -1 : 1);
    }
}


public class JanusSlice
{
    public required Angle Angle;
    public required Angle Radius;
    public int overlap(JanusSlice other)
    {
        var startingAngleDiff = JanusAngle.ClosestTurn(Angle, other.Angle);
        if (startingAngleDiff == -1)
        {
            if ((other.Angle + other.Radius).Reduced() > Angle)
                return -1;
            if ((Angle + Radius) > other.Angle)
                return 1;
        }
        else
        {
            if((Angle + Radius).Reduced() > other.Angle)
                return 1;
            if (other.Angle + other.Radius > Angle)
                return -1;
        }
        return 0;
    }
    // returns how many angles one of the radius is inside the other.
    public double overlapDifference(JanusSlice other)
    {
        var direction = overlap(other);
        switch (direction)
        {
            case 1:
                return (Angle + Radius - other.Angle).Theta;
            case -1:
                return (other.Angle + other.Radius - Angle).Theta;
        }

        return 0;
    }
    // this will merge the 2 slices, if they aren't overlapping it'll just add the radius of the other to the first!
    public JanusSlice Merge(JanusSlice other)
    {
        if(overlap(other) == 0)
            Radius = Radius + other.Radius;
        else
            Radius = Radius + other.Radius - overlapDifference(other);
        return this;
    }
}
