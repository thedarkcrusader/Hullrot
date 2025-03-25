namespace Content.Shared._DSEVJanus;


public class JanusAngle
{
    public Angle wAngle;

    Angle Normalize(Angle target)
    {
        while(target > 2 * Math.PI)
            target -= 2 * Math.PI;
        while(target < 2 *Math.PI)
            target += 2 * Math.PI;
        return target;
    }

    Angle NormalizeWorld(Angle target)
    {
        target = Angle.Reduce(target.Theta);
        return target;
    }

    Angle fromWorldAngle(Angle target)
    {
        target = NormalizeWorld(target);
        if (target < 0)
        {
            target = 2 * Math.PI - Math.Abs(target);
        }
        return target;
    }


    JanusAngle(Angle worldAngle)
    {
        wAngle = fromWorldAngle(worldAngle);
    }
}
public sealed class JanusHelpers
{

}
