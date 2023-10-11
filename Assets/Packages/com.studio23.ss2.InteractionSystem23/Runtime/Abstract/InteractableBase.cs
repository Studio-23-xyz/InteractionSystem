using System.Collections.Generic;
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
    public abstract class InteractableBase:MonoBehaviour
    {
        [SerializeField] InteractionState _curState;
        /// <summary>
        /// If interactionHoldTime <= 0, no hold interaction
        /// </summary>
        [SerializeField]  
        float _interactionHoldTime = 0;
        [FormerlySerializedAs("_onInteractionStarted")] [FormerlySerializedAs("onInteractionStarted")]
        public UnityEvent<InteractableBase> OnInteractionStarted;
        [FormerlySerializedAs("_onInteractionPaused")] [FormerlySerializedAs("onInteractionPaused")]
        public UnityEvent<InteractableBase> OnInteractionPaused;
        [FormerlySerializedAs("_onInteractionResumed")] [FormerlySerializedAs("onInteractionResumed")]
        public UnityEvent<InteractableBase> OnInteractionResumed;
        [FormerlySerializedAs("_onInteractionCompleted")] [FormerlySerializedAs("onInteractionCompleted")]
        public UnityEvent<InteractableBase> OnInteractionCompleted;

        [SerializeReference]
        [SerializeReferenceButton]
        private List<InteractionCondition> _interactionConditions = new List<InteractionCondition>();
        
        public abstract InputButtonSlot InputButton { get; }
        public InteractionState CurState => _curState;
        //#TODO InteractionHoldTime usage
        public float InteractionHoldTime => _interactionHoldTime;
        
        
        /// <summary>
        /// Called on start()
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// Called before the interaction task is started
        /// </summary>
        protected abstract void HandleInteractionStarted();
        /// <summary>
        /// Called when an interaction is paused
        /// </summary>
        protected abstract void HandleInteractionPause();
        /// <summary>
        /// Called when a paused interaction is resumed
        /// </summary>
        protected abstract void HandleInteractionResumed();
        /// <summary>
        /// Called when an interaction is completed
        /// </summary>
        protected abstract void HandleInteractionCompleted();
        /// <summary>
        /// Interaction prompt prefix(ex: "Inspect")
        /// </summary>
        /// <returns></returns>
        public abstract string GetPromptPrefix();
        /// <summary>
        /// Interaction prompt suffix that appears after the prompt
        /// </summary>
        /// <returns></returns>
        public abstract string GetPromptSuffix();
        /// <summary>
        /// Prevents another subinteraction from starting when this is running
        /// </summary>
        public abstract bool CanBeInterrupted { get; }
        /// <summary>
        /// The actual async interaction task
        /// The interactable should assume that it can be interrupted any time during interaction
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract UniTask DoInteraction(CancellationToken token);

        public void InitializeInteraction()
        {
            _curState = InteractionState.Active;
            HandleInteractionStarted();

            OnInteractionStarted?.Invoke(this);
        }
        public void PauseInteraction()
        {
            _curState = InteractionState.Paused;
            HandleInteractionPause();
            OnInteractionPaused?.Invoke(this);
        }
        
        public void CompleteInteraction()
        {
            _curState = InteractionState.Inactive;
            HandleInteractionCompleted();
            OnInteractionCompleted?.Invoke(this);
        }

        public void ResumeInteraction()
        {
            _curState = InteractionState.Active;
            HandleInteractionResumed();
            OnInteractionResumed?.Invoke(this);
        }
        
        public virtual bool CheckCanStartInteraction(PlayerInteractionFinder interactionFinder)
        {
            if (_curState != InteractionState.Inactive && CanBeInterrupted)
                return false;
            foreach (var condition in _interactionConditions)
            {
                if (!condition.Evaluate(interactionFinder))
                    return false;
            }
            return true;
        }
            
        private void Start()
        {
            Initialize();
        }
    }
}