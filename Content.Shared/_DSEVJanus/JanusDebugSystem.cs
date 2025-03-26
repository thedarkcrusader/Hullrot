namespace Content.Shared._DSEVJanus;


/// <summary>
/// This handles...
/// </summary>
public sealed class JanusDebugSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem transformSystem = default!;
    public override void Update(float time)
    {
        var enumerator = EntityManager.EntityQueryEnumerator<JanusDebugComponent>();
        while (enumerator.MoveNext(out var uid, out var comp))
        {
            comp.JanusAngle = new JanusAngle(transformSystem.GetWorldRotation(uid)).Angle;
        }
    }
}
