using System.Threading;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using com.studio23.ss2.InteractionSystem23.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Abstract
{
    public abstract class OpenableBase: InteractableBase
    {
        [FormerlySerializedAs("isOpen")] [SerializeField] private bool _isOpen = false;
        [FormerlySerializedAs("objectName")] public string _objectName ="DarkSoulsDoor";
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.ToggleButton;
        protected abstract UniTask DoOpenInteraction(CancellationToken token);
        [FormerlySerializedAs("onOpened")] [FormerlySerializedAs("OnOpened")] 
        public UnityEvent _onOpened;
        [FormerlySerializedAs("canBeInterrupted")] [SerializeField] 
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
            _onOpened.Invoke();
        }

        public virtual void SnapToOpenState()
        {
            _isOpen = true;
            _onOpened.Invoke();
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