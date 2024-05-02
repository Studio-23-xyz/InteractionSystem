using System.Collections.Generic;
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
    public abstract class InteractableBase:MonoBehaviour
    {
        [SerializeField] InteractionState _curState;
        [SerializeField] private InteractionConditionResult _lastEvaluationResult = InteractionConditionResult.Show;
        /// <summary>
        /// If interactionHoldTime <= 0, no hold interaction
        /// </summary>
        [SerializeField]  
        float _interactionHoldTime = 0;
        public UnityEvent<InteractableBase> OnInteractionStarted;
        public UnityEvent<InteractableBase> OnInteractionPaused;
        public UnityEvent<InteractableBase> OnInteractionResumed;
        public UnityEvent<InteractableBase> OnInteractionCompleted;

        [SerializeReference, SerializeReferenceButton]
        protected List<InteractionCondition> _interactionConditions = new List<InteractionCondition>();
        public List<InteractionCondition> InteractionConditions => _interactionConditions;
        
        public abstract InputButtonSlot InputButton { get; }
        public InteractionState CurState => _curState;
        /// <summary>
        /// The InteractionConditionResult from last call of EvaluateInteractionConditions()
        /// </summary>
        public InteractionConditionResult LastEvaluationResult => _lastEvaluationResult;
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
        public abstract UniTask DoNormalInteraction(CancellationToken token);
        public abstract UniTask DoDisabledInteraction(CancellationToken token);

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
        
        /// <summary>
        /// External code should call this to evaluate the interaction conditions
        /// Subclasses should override the internal version of this.
        /// </summary>
        /// <param name="interactionFinder"></param>
        /// <returns></returns>
        public InteractionConditionResult EvaluateInteractionConditions(PlayerInteractionFinder interactionFinder)
        {
            _lastEvaluationResult = EvaluateInteractionConditionsInternal(interactionFinder);
            return _lastEvaluationResult;
        }
        
        /// <summary>
        /// Subclasses should override this to return their condition eval result
        /// </summary>
        /// <param name="interactionFinder"></param>
        /// <returns></returns>
        protected virtual InteractionConditionResult EvaluateInteractionConditionsInternal(PlayerInteractionFinder interactionFinder)
        {
            if (_curState != InteractionState.Inactive && CanBeInterrupted)
                return InteractionConditionResult.Hide;
            //don't show if component is disabled
            if (!enabled)
                return InteractionConditionResult.Hide;
            
            var result = InteractionConditionResult.Show;
            // If any condition says to hide or disable
            // stop evaluating and just return that
            foreach (var condition in _interactionConditions)
            {
                result = condition.Evaluate(interactionFinder);
                if (result != InteractionConditionResult.Passthrough)
                    return result;
            }

            if (result == InteractionConditionResult.Passthrough)
            {
                return InteractionConditionResult.Hide;
            }
            return result;
        }
            
        private void Start()
        {
            Initialize();
        }
    }
}