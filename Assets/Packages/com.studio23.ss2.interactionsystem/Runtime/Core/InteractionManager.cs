using System;
using System.Collections.Generic;
using System.Threading;
using Bdeshi.Helpers.Utility;
using BDeshi.Logging;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.InteractionSystem.Core
{
    public class InteractionManager:MonoBehaviourSingletonPersistent<InteractionManager>
    {
        [SerializeField] 
        private List<InteractableBase> _interactionStack;
        [SerializeField] 
        private InteractableBase _currentInteractable;
        [SerializeField] 
        private InputPromptsControllerBase _inputPromptsController;
        CancellationTokenSource _subInteractionCancellationTokens;
        [CanBeNull] public InteractableBase CurrentInteractable => _currentInteractable;
        /// <summary>
        /// Fired when we start the first interaction on the stack
        /// Not fired when subinteractions are started 
        /// </summary>
        public event Action OnInteractionChainStarted;
        /// <summary>
        /// Fired when we complete all the interactions on the stack
        /// Or when we cancel the interaction confirmation without anything in the stack
        /// Not fired when subinteractions are completed
        /// </summary>
        public event Action OnInteractionChainEnded;

        public event Action OnInteractionStarted;
        public event Action OnInteractionEnded;
        
        public bool IsRunningInteraction => CurrentInteractable != null;
  
        public ICategoryLogger<InteractionLogCategory> Logger => _logger; 
        [SerializeField] SerializableCategoryLogger<InteractionLogCategory> _logger = new ((InteractionLogCategory)~0); 
        
        public async UniTask DoInteraction()
        {
            OnInteractionChainStarted?.Invoke();
            
            while (_interactionStack.Count > 0)
            {
                //cache this so that we have the interrupted one after cancellation
                _currentInteractable = _interactionStack[^1];
                
                Logger.Log(InteractionLogCategory.InteractionPop, $"{_currentInteractable}", _currentInteractable);
                //the state of the interactable is saved
                //if it was cancelled previously, we resume
                //else it is an interaction we are newly intializing 
                if (_currentInteractable.CurState == InteractionState.Paused)
                {
                    Logger.Log(InteractionLogCategory.InteractionResume, $"{_currentInteractable} {_currentInteractable}", _currentInteractable);
                    _currentInteractable.ResumeInteraction();
                }
                else
                {
                    Logger.Log(InteractionLogCategory.InteractionStart, $"{_currentInteractable} {_currentInteractable}", _currentInteractable);
                    _currentInteractable.InitializeInteraction();
                }

                bool isCancelled;
                if (_currentInteractable.LastEvaluationResult == InteractionConditionResult.Show)
                {
                    isCancelled = await _currentInteractable
                        .DoNormalInteraction(_subInteractionCancellationTokens.Token)
                        .SuppressCancellationThrow();
                }
                else
                {
                    isCancelled = await _currentInteractable
                        .DoDisabledInteraction(_subInteractionCancellationTokens.Token)
                        .SuppressCancellationThrow();
                }
                
                Logger.Log(InteractionLogCategory.InteractionEnd,
                    $"{_currentInteractable} interaction task end, Cancelled: {isCancelled}", _currentInteractable);
                // check task status to handle cancellation
                if (isCancelled)
                {
                    // if the task has been cancelled
                    // we assume that a sub interaction was added
                    // cur.pause()
                    // Debug.Log("interaction pause  " + currentInteractable);
                    _currentInteractable.PauseInteraction();
                    RefreshCancellationToken();
                    Logger.Log(InteractionLogCategory.InteractionPause | InteractionLogCategory.InteractionCancelled,
                        $"interaction pause {_currentInteractable}");

                    // the new subinteraction is already at the top of the stack
                    // at the next loop, it will pick the new subinteraction and await that
                    // so no need to do anything else
                }
                else
                {
                    // if not cancelled, assume that the interaction was completed
                    if (_interactionStack.Count > 0)
                    {
                        _interactionStack.RemoveAt(_interactionStack.Count - 1);
                    }
                    Logger.Log( InteractionLogCategory.InteractionPop,
                        $"interaction Stack complete {_currentInteractable} interactionStack.Count {_interactionStack.Count}");
                    if (_currentInteractable.LastEvaluationResult == InteractionConditionResult.Show)
                    {
                        _currentInteractable.CompleteInteraction();
                    }
                }
 
            }
            _currentInteractable = null;
            OnInteractionChainEnded?.Invoke();
        }

        void AddInteraction(InteractableBase newInteractableBase)
        {
            _interactionStack.Add(newInteractableBase);
        }

        public async void StartNewInteraction(InteractableBase newInteractableBase)
        {
            if(_currentInteractable != null && !_currentInteractable.CanBeInterrupted)
            {
                // NOTE: This shouldn't even fire. 
                // unless someone allowed a subinteractable to proc when a non-interruptible was running
                Debug.LogWarning($"Can't interrupt current interactable: {_currentInteractable}", _currentInteractable);
                return;
            }
            Logger.Log(InteractionLogCategory.InteractionPush,$" Push new interaction {newInteractableBase}");
            AddInteraction(newInteractableBase);

            if (_interactionStack.Count > 1)
            {
                _subInteractionCancellationTokens.Cancel();
            }
            else
            {
                await DoInteraction();
            }
        }
        
        protected override void Initialize()
        {
            RefreshCancellationToken();
        }

        /// <summary>
        /// Creates a new cancellation token
        /// Apparently I can't reuse them after cancelling
        /// Or reset them manually
        /// </summary>
        private void RefreshCancellationToken()
        {
            if (_subInteractionCancellationTokens != null)
            {
                _subInteractionCancellationTokens.Dispose();
            }
            _subInteractionCancellationTokens = new CancellationTokenSource();
        }

    

        public void ShowNewInteractables(List<InteractableBase> interactables)
        {
            _inputPromptsController.SetInteractables(interactables);
        }

        private void OnDestroy()
        {
            if (_subInteractionCancellationTokens != null)
            {
                _subInteractionCancellationTokens.Cancel();
                _subInteractionCancellationTokens.Dispose();
            }
        }
    }
    
    [Flags]
    public enum InteractionLogCategory
    {
        InteractionPush = 1 << 0,
        InteractionPause = 1 << 1,
        InteractionResume = 1 << 2,
        InteractionPop = 1 << 3,
        InteractionChain = 1 << 4,
        InteractionStart = 1 << 5,
        InteractionEnd = 1 << 6,
        InteractionCancelled = 1 << 7,
        DoInteraction = 1 << 8,
    }
}