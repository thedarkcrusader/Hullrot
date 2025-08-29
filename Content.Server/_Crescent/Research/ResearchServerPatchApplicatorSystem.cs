using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Content.Shared.Interaction;
using Content.Shared.Research.Components;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared._Crescent.Research;

namespace Content.Server._Crescent.Research;

public sealed class ResearchServerPatchApplicatorSystem : EntitySystem
{

    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ResearchServerPatchApplicatorComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<ResearchServerPatchApplicatorComponent, PatchApplicatorDoAfterFinishEvent>(OnPatchApplicatorDoAfter);
    }

    private void OnAfterInteract(EntityUid uid, ResearchServerPatchApplicatorComponent component, AfterInteractEvent args)
    {
        if (!args.CanReach)
            return;

        if (!TryComp<ResearchServerComponent>(args.Target, out var server))
            return;

        if (HasComp<PatchedResearchServerComponent>(args.Target))
        {
            _popupSystem.PopupEntity(Loc.GetString("research-patcher-fail"), args.Target.Value, args.User);
            return;
        }

        DoAfterArgs doAfterArguments = new DoAfterArgs(EntityManager, args.User, component.ApplicationTime, new PatchApplicatorDoAfterFinishEvent(), uid, uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
        };

        _doAfter.TryStartDoAfter(doAfterArguments, null);
    }

    private void OnPatchApplicatorDoAfter(EntityUid uid, ResearchServerPatchApplicatorComponent component, PatchApplicatorDoAfterFinishEvent args)
    {
        if (args.Cancelled || !args.Args.Target.HasValue || !TryComp<ResearchServerComponent>(args.Args.Target.Value, out var server))
            return;

        if (HasComp<PatchedResearchServerComponent>(args.Args.Target.Value))
        {
            _popupSystem.PopupEntity(Loc.GetString("research-patcher-fail"), args.Args.Target.Value, args.Args.User);
            return;
        }

        // Apply the patch
        EntityManager.AddComponent<PatchedResearchServerComponent>(args.Args.Target.Value);

        _popupSystem.PopupEntity(Loc.GetString("research-patcher-success"), args.Args.Target.Value, args.Args.User);

        if (!component.Reusable)
            QueueDel(uid);
    }
}
