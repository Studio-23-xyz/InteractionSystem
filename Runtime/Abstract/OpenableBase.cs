using System.Threading;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
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

        protected override void HandleInteractionInitialize()
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

        public override async UniTask DoInteraction(CancellationToken token)
        {
            if (!_isOpen)
            {
                await DoOpenInteraction(token);
            }
        }

        public override bool CheckCanStartInteraction(PlayerInteractionFinder finder)
        {
            if (_isOpen)
                return false;
            return base.CheckCanStartInteraction(finder);
        }

        public override string GetPromptPrefix() => "Open";

        protected virtual void Start()
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