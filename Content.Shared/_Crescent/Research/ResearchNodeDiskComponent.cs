// *announce sound effect*
// The enemy has taken our intelligence.

using Robust.Shared.GameStates;

namespace Content.Shared._Crescent.Research;

[RegisterComponent, NetworkedComponent]
public sealed partial class ResearchNodeDiskComponent : Component
{
    [DataField("researchPrototype"), ViewVariables]
    public string? ResearchPrototype = null;
}
