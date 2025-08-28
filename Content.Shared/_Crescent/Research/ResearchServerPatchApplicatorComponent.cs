using Robust.Shared.GameStates;

namespace Content.Shared._Crescent.Research;

// Do I need to explain this (part 2)?
[RegisterComponent]
public sealed partial class ResearchServerPatchApplicatorComponent : Component
{
    [DataField, ViewVariables]
    public TimeSpan ApplicationTime = TimeSpan.FromSeconds(10);

    [DataField, ViewVariables]
    public bool Reusable = false;
}
