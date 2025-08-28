using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Crescent.Research;

// The enemy has captured our intelligence!
[RegisterComponent, NetworkedComponent]
public sealed partial class ResearchStealerDeviceComponent : Component
{
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_Crescent/Research/researchhijacker.ogg");

    // stupid fucking hack so I can have a cool audio file I love my job!!!
    public TimeSpan StealStartTime = TimeSpan.FromSeconds(30);

    // How long it takes to steal a node.

    [DataField, ViewVariables]
    public TimeSpan StealTime = TimeSpan.FromSeconds(10);

    // Alright, who backdoored the security patch this time?
    [DataField, ViewVariables]
    public bool IgnorePatchedServer = false;
}
