using System.Threading;
using com.bdeshi.helpers.Input;
using Studio23.SS2.InteractionSystem23.Core;
using Studio23.SS2.InteractionSystem23.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Studio23.SS2.InteractionSystem23.Abstract
{
    public abstract class ClosableBase: InteractableBase
    {
        [SerializeField] private bool _isOpen = true;
        public string _objectName ="Only Closable";

        protected abstract UniTask DoCloseInteraction(CancellationToken token);
        public UnityEvent OnClosed;
        [SerializeField] private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.ToggleButton;
        
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

        public override async UniTask DoNormalInteraction(CancellationToken token)
        {
            if (_isOpen)
            {
                await DoCloseInteraction(token);
            }
        }

        protected override InteractionConditionResult EvaluateInteractionConditionsInternal(PlayerInteractionFinder finder)
        {
            if (!_isOpen)
                return InteractionConditionResult.Hide;
            return base.EvaluateInteractionConditionsInternal(finder);
        }

        public override string GetPromptPrefix() => "Close";
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