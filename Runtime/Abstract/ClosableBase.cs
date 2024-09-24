using System.Threading;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Core;
using Studio23.SS2.InteractionSystem.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class ClosableBase: InteractableBase
    {
        [SerializeField] private bool _isOpen = true;
        public LocalizedString _objectName;
        public LocalizedString PromptPrefix;

        protected abstract UniTask DoCloseInteraction(CancellationToken token);
        public UnityEvent OnClosed;
        [SerializeField] private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;
        public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.ToggleButton;
        
        protected override void HandleInteractionStarted()
        {
            
        }

        protected override void HandleInteractionPause()
        {
            
        }

        protected override void HandleInteractionResumed()
        {
            
        }

        protected override void HandleInteractionCompleted()
        {
            _isOpen = false;
            OnClosed.Invoke();
        }

        public virtual void SnapToOpenState()
        {
            _isOpen = true;
        }
        
        
        public virtual void SnapToClosedState()
        {
            _isOpen = false;
            OnClosed.Invoke();
        }

        public override async UniTask DoInteraction(CancellationToken token)
        {
            if (_isOpen)
            {
                await DoCloseInteraction(token);
            }
        }

        protected override InteractionConditionResult EvaluateInteractionConditionsInternal()
        {
            if (!_isOpen)
                return InteractionConditionResult.Hide;
            return base.EvaluateInteractionConditionsInternal();
        }

        public override LocalizedString GetLocalizedPromptPrefix()
        {
            return PromptPrefix;
        }

        public override LocalizedString GetLocalizedPromptSuffix()
        {
            return _objectName;
        }
        
        protected override void Initialize()
        {
            if (_isOpen)
            {
                SnapToOpenState();
            }
            else
            {
                SnapToClosedState();
            }
        }
    }
}