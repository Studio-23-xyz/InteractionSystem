using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class BasicInstantSwitch:InstantSwitch
    {
        [SerializeField] Material _activeMateral;
        [SerializeField] Material _inactiveMateral;
        [SerializeField] private GameObject _matSwapTarget;
        private Renderer _renderer;
        [SerializeField] private Sprite _markerIcon;
        public override Sprite MarkerIcon => _markerIcon;
        protected override void Initialize()
        {
            _renderer = _matSwapTarget.GetComponent<Renderer>();
            base.Initialize();
        }

        protected override async UniTask DoActivateInteraction(CancellationToken cancellationToken)
        {
            await base.DoActivateInteraction(cancellationToken);
            _renderer.sharedMaterial = _activeMateral;
        }

        protected override async UniTask DoDeactivateInteraction(CancellationToken cancellationToken)
        {
            await base.DoDeactivateInteraction(cancellationToken);
            _renderer.sharedMaterial = _inactiveMateral;
        }

        public override void SnapToActivatedState()
        {
            _renderer.sharedMaterial = _activeMateral;
        }

        public override void SnapToDeactivatedState()
        {
            _renderer.sharedMaterial = _inactiveMateral;
        }
    }
}