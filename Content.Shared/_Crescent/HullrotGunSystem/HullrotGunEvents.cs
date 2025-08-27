using Robust.Shared.Map;
using Robust.Shared.Serialization;


namespace Content.Shared._Crescent.HullrotGunSystem;

[Serializable, NetSerializable]
public class ClientSideGunFiredEvent : EntityEventArgs
{
    public NetEntity gun;
    public NetEntity shooter;
    public NetCoordinates shotFrom;
    public NetCoordinates aimedPosition;
}
