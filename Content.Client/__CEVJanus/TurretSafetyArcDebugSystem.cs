using System.Collections.Generic;
using System.Numerics;
using Content.Server._DESVJanus;
using Content.Shared._DSEVJanus;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Utility;
using Robust.Client.GameObjects;


namespace Content.Client.__CEVJanus
{
    public sealed class TurretSafetyArcDebugSystem : EntitySystem
    {
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly IOverlayManager _overlayManager = default!;
        [Dependency] private readonly TransformSystem _transform = default!;
        [Dependency] private readonly SharedMapSystem _map = default!;

        private TurretSafetyArcsOverlay? _overlay;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;

                if (_enabled)
                {
                    DebugTools.Assert(_overlay == null);
                    _overlay = new TurretSafetyArcsOverlay(
                        EntityManager,
                        _eyeManager,
                        _mapManager,
                        _transform,
                        _map);

                    _overlayManager.AddOverlay(_overlay);
                }
                else
                {
                    _overlayManager.RemoveOverlay(_overlay!);
                    _overlay = null;
                }
            }
        }

        private bool _enabled;

        private sealed class TurretSafetyArcsOverlay : Overlay
        {
            private readonly IEntityManager _entityManager;
            private readonly IEyeManager _eyeManager;
            private readonly IMapManager _mapManager;
            private readonly SharedTransformSystem _transformSystem;
            private readonly SharedMapSystem _mapSystem;

            public override OverlaySpace Space => OverlaySpace.WorldSpace;

            public TurretSafetyArcsOverlay(
                IEntityManager entManager,
                IEyeManager eyeManager,
                IMapManager mapManager,
                SharedTransformSystem transformSystem,
                SharedMapSystem mapSystem
            )
            {
                _entityManager = entManager;
                _eyeManager = eyeManager;
                _mapManager = mapManager;
                _transformSystem = transformSystem;
                _mapSystem = mapSystem;
            }

            protected override void Draw(in OverlayDrawArgs args)
            {
                var handle = args.WorldHandle;
                var enumerator = _entityManager.AllEntityQueryEnumerator<ShipWeaponSafeUseComponent, TransformComponent>();
                while (enumerator.MoveNext(out var uid, out var safeComponent, out var xform))
                {
                    Vector2 from = _transformSystem.GetWorldPosition(xform);
                    foreach (var anglePair in safeComponent.safeAngles)
                    {
                        Vector2 first = from + (anglePair.first).ToWorldVec() * 3;
                        handle.DrawLine(from, first, Color.Green);
                        Vector2 second = from + (anglePair.second).ToWorldVec() * 3;
                        handle.DrawLine(from, second, Color.Red);
                        handle.DrawLine(second, (second+first)/2, Color.Black);
                        handle.DrawLine((second + first)/2, first, Color.White);

                    }
                }
            }
        }
    }
}

