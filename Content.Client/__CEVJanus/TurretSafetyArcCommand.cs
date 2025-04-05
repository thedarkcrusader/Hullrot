using Robust.Shared.Console;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


namespace Content.Client.__CEVJanus;


public sealed class TurretSafetyArcCommand : LocalizedEntityCommands
{
    [Dependency] private readonly TurretSafetyArcDebugSystem _system = default!;

    public override string Command => "showturretarc";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _system.Enabled ^= true;
    }
}

