using System.Threading;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Abstract
{
    public abstract class ClosableBase: InteractableBase
    {
        [SerializeField] private bool _isOpen = true;
        public string _objectName ="Only Closable";

        protected abstract UniTask DoCloseInteraction(CancellationToken token);
        [FormerlySerializedAs("onClosed")] [FormerlySerializedAs("OnClosed")] public UnityEvent _onClosed;
        [FormerlySerializedAs("canBeInterrupted")] [SerializeField] private bool _canBeInterrupted = false;
        public override bool CanBeInterrupted => _canBeInterrupted;
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.ToggleButton;
        
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
            _isOpen = false;
            _onClosed.Invoke();
        }

        public virtual void SnapToOpenState()
        {
            _isOpen = true;
        }
        
        
        public virtual void SnapToClosedState()
        {
            _isOpen = false;
            _onClosed.Invoke();
        }

        public override async UniTask DoInteraction(CancellationToken token)
        {
            if (_isOpen)
            {
                await DoCloseInteraction(token);
            }
        }

        public override bool CheckCanStartInteraction(PlayerInteractionFinder finder)
        {
            if (!_isOpen)
                return false;
            return base.CheckCanStartInteraction(finder);
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