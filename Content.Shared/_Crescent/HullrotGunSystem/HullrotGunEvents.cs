using Robust.Shared.Map;


namespace Content.Shared._Crescent.HullrotGunSystem;


public class ClientSideGunFiredEvent : EntityEventArgs
{
    public NetEntity gun;
    public NetEntity shooter;
    public NetCoordinates aimedPosition;
}
