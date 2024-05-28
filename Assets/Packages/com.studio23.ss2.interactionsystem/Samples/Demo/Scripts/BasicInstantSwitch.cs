using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class BasicInstantSwitch:InstantSwitch
    {
        public override string Name => "BasicInstantSwitch";
        [SerializeField] Material _activeMateral;
        [SerializeField] Material _inactiveMateral;
        [SerializeField] private GameObject _matSwapTarget;
        private Renderer _renderer;
        [SerializeField] private Sprite _markerIcon;
      
        protected override void Initialize()
        {
            _renderer = _matSwapTarget.GetComponent<Renderer>();
            base.Initialize();
        }

        public override async UniTask DoDisabledInteraction(CancellationToken token)
        {
            UnityEngine.Debug.Log("can't interact with switch ");
            await UniTask.Yield();
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