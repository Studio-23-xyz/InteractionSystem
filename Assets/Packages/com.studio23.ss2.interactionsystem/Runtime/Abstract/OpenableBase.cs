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
    public abstract class OpenableBase: InteractableBase
    {
        [SerializeField] bool _isOpen = false;
        public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.ToggleButton;
        protected abstract UniTask DoOpenInteraction(CancellationToken token);

        public UnityEvent OnOpened;
        private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;

        public LocalizedString ObjectName;
        public LocalizedString PromptPrefix;

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
            _isOpen = true;
            OnOpened.Invoke();
        }

        public virtual void SnapToOpenState()
        {
            _isOpen = true;
            OnOpened.Invoke();
        }
        
        
        public virtual void SnapToClosedState()
        {
            _isOpen = false;
        }

        public override async UniTask DoInteraction(CancellationToken token)
        {
            if (!_isOpen)
            {
                await DoOpenInteraction(token);
            }
        }

        protected override InteractionConditionResult EvaluateInteractionConditionsInternal()
        {
            if (_isOpen)
                return InteractionConditionResult.Hide;
            return base.EvaluateInteractionConditionsInternal();
        }

        public override LocalizedString GetLocalizedPromptPrefix()
        {
            return PromptPrefix;
        }

        public override LocalizedString GetLocalizedPromptSuffix()
        {
            return ObjectName;
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