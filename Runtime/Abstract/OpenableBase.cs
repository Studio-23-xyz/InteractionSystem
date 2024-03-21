using System.Threading;
using Bdeshi.Helpers;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Core;
using Studio23.SS2.InteractionSystem.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class OpenableBase: InteractableBase
    {
        [SerializeField] bool _isOpen = false;
        [SerializeField] protected string _objectName ="DarkSoulsDoor";
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.ToggleButton;
        protected abstract UniTask DoOpenInteraction(CancellationToken token);

        public UnityEvent OnOpened;
        private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;

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

        public override async UniTask DoNormalInteraction(CancellationToken token)
        {
            if (!_isOpen)
            {
                await DoOpenInteraction(token);
            }
        }

        protected override InteractionConditionResult EvaluateInteractionConditionsInternal(PlayerInteractionFinder finder)
        {
            if (_isOpen)
                return InteractionConditionResult.Hide;
            return base.EvaluateInteractionConditionsInternal(finder);
        }

        public override string GetPromptPrefix() => "Open";

        public override string GetPromptSuffix() => _objectName;

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