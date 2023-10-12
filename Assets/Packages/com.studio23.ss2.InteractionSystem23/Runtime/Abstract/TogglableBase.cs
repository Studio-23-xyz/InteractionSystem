using System.Threading;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Abstract
{
    public abstract class TogglableBase : InteractableBase
    {
        [FormerlySerializedAs("objectName")] public string _objectName ="Switch";

        [FormerlySerializedAs("onDeactivated")] 
        [SerializeField]
        protected UnityEvent _onDeactivated;
        [FormerlySerializedAs("onActivated")] 
        [SerializeField] 
        private UnityEvent _onActivated;
        
        [FormerlySerializedAs("isActive")] 
        [SerializeField] 
        protected bool _isActive = false;
        public bool IsActive => _isActive;
        public UnityEvent OnActivated => _onActivated;
        public UnityEvent OnDeactivated => _onDeactivated;

        [SerializeField] private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;
        
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.ToggleButton;
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
        
        public override async UniTask DoNormalInteraction(CancellationToken token)
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