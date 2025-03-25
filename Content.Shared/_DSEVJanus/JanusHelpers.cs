using System.Runtime.CompilerServices;


namespace Content.Shared._DSEVJanus;

/// <summary>
///  All of these helpers are licensed under MIT for anyone wishing to use them outside of the Janus codebase.
///  Angles are all expected to be counter-clockwise (akin to world Angles) and so is the math around them. - SPCR
/// </summary>
public class JanusAngle
{
    public Angle Angle;
    public JanusAngle(Angle worldAngle)
    {
        worldAngle = worldAngle.Reduced();
        if(worldAngle < 0)
            worldAngle = 2*Math.PI + worldAngle.Theta;
        Angle = worldAngle;
    }

    public JanusAngle Reduced()
    {
        Angle = Angle.Reduced();
        return this;
    }

    public static JanusAngle operator +(JanusAngle a, JanusAngle b)
    {
        a.Angle += b.Angle;
        return a;
    }

    public static JanusAngle operator -(JanusAngle a, JanusAngle b)
    {
        a.Angle -= b.Angle;
        return a;
    }

    public static JanusAngle operator +(JanusAngle a, Angle b)
    {
        a.Angle += b;
        return a;
    }

    public static JanusAngle operator -(JanusAngle a, Angle b)
    {
        a.Angle += b;
        return a;
    }

    public static bool operator >(JanusAngle a, JanusAngle b)
    {
        if(a.Angle > b.Angle)
            return true;
        return false;
    }

    public static bool operator <(JanusAngle a, JanusAngle b)
    {
        if (a.Angle < b.Angle)
            return true;
        return false;
    }
    // returns wheter a rotation should be positive or negative to reach the targetAngle -1 is clock-wise , 1 is counter clock wise
    public int ClosestTurn(JanusAngle targetAngle)
    {
        if (Math.Abs((targetAngle.Angle - Angle).Reduced()) < Math.Abs((targetAngle.Angle + Angle).Reduced()))
            return -1;
        return 1;
    }

    // returns the acute angle between 2 angles. Will be positive if counter clockwise , negative other wise
    public Angle ClosestDifference(JanusAngle targetAngle)
    {
        if (ClosestTurn(targetAngle) == -1)
            return Math.Abs((targetAngle.Angle - Angle).Reduced());
        return Math.Abs((targetAngle.Angle + Angle).Reduced());
    }
}


public class JanusSlice
{
    public required JanusAngle Angle;
    public required JanusAngle Radius;
    public int overlap(JanusSlice other)
    {
        var startingAngleDiff = Angle.ClosestTurn(other.Angle);
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
                return (Angle + Radius - other.Angle).Angle.Theta;
            case -1:
                return (other.Angle + other.Radius - Angle).Angle.Theta;
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
