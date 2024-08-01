using System.Threading;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class TogglableBase : InteractableBase
    {
        [SerializeField] string _objectName ="Switch";
        [SerializeField]
        protected UnityEvent _onDeactivated;
        [SerializeField] 
        private UnityEvent _onActivated;
       
        [SerializeField] 
        protected bool _isActive = false;
        public bool IsActive => _isActive;
        public UnityEvent OnActivated => _onActivated;
        public UnityEvent OnDeactivated => _onDeactivated;

        [SerializeField] private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;
        
        public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.ToggleButton;
        public abstract void SnapToActivatedState();
        public abstract void SnapToDeactivatedState();
        protected abstract UniTask DoActivateInteraction(CancellationToken cancellationToken);
        protected abstract UniTask DoDeactivateInteraction(CancellationToken cancellationToken);
        
        protected override void Initialize()
        {
            if (_isActive)
            {
                SnapToActivatedState();
                _onActivated.Invoke();
            }
            else
            {
                SnapToDeactivatedState();
                _onDeactivated.Invoke();
            }
        }
        
        public async UniTask Toggle(CancellationToken cancellationToken)
        {
            _isActive = !_isActive;
            if (_isActive)
            {
                await DoActivateInteraction(cancellationToken);
                _onActivated.Invoke();
            }
            else
            {
                await DoDeactivateInteraction(cancellationToken);
                _onDeactivated.Invoke();
            }
        }
        
        public override async UniTask DoInteraction(CancellationToken token)
        {
            await Toggle(token);
        }
        
        public override string GetPromptPrefix()
        {
            return _isActive ? "Deactivate" : "Activate";
        }
        
        public override string GetPromptSuffix()
        {
            return _objectName;
        }
    }
}