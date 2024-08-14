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

namespace Studio23.SS2.InteractionSystem.Core
{
    public class InteractionManager:MonoBehaviourSingletonPersistent<InteractionManager>
    {
        
        [SerializeField] 
        private List<InteractableBase> _interactionStack;
        public List<InteractableBase> InteractionStack => _interactionStack;
        [SerializeField] 
        private InteractableBase _currentInteractable;
        [SerializeField] private InteractionInputHandlerBase _inputHandler;
        public InteractionInputHandlerBase InputHandler => _inputHandler;
        [SerializeField] 
        private InputPromptsControllerBase _inputPromptsController;
        public InputPromptsControllerBase InputPromptsController => _inputPromptsController;
        CancellationTokenSource _subInteractionCancellationTokens;
        [CanBeNull] public InteractableBase CurrentInteractable => _currentInteractable;

        public InteractableHoverSpriteTable InteractableIconTable;
        /// <summary>
        /// Fired when we start the first interaction on the stack
        /// Not fired when subinteractions are started 
        /// </summary>
        public event Action<InteractableBase> OnInteractionChainStarted;
        /// <summary>
        /// Fired when we complete all the interactions on the stack
        /// Or when we cancel the interaction confirmation without anything in the stack
        /// Not fired when subinteractions are completed
        /// </summary>
        public event Action<InteractableBase> OnInteractionChainEnded;

        public event Action<InteractableBase> OnInteractionStarted;
        public event Action<InteractableBase> OnInteractionEnded;
        
        public bool IsRunningInteraction => CurrentInteractable != null;
  
        public ICategoryLogger<InteractionLogCategory> Logger => _logger; 
        [SerializeField] SerializableCategoryLogger<InteractionLogCategory> _logger = new ((InteractionLogCategory)~0); 
        
        public async UniTask DoInteraction()
        {
            if (_interactionStack.Count <=0)
            {
                return;
            }
            var firstInteractableInChain = _interactionStack[^1];
            OnInteractionChainStarted?.Invoke(firstInteractableInChain);
            
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

                bool isCancelled = await _currentInteractable
                    .DoInteraction(_subInteractionCancellationTokens.Token)
                    .SuppressCancellationThrow();
                
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
                    Logger.Log( InteractionLogCategory.InteractionComplete,
                        $"interaction Stack complete {_currentInteractable} interactionStack.Count {_interactionStack.Count}");
                    var completedInteractable = _currentInteractable;
                    _currentInteractable = null;
                    completedInteractable.CompleteInteraction();
                }
 
            }
            _currentInteractable = null;
            OnInteractionChainEnded?.Invoke(firstInteractableInChain);
        }

        void AddInteraction(InteractableBase newInteractableBase)
        {
            _interactionStack.Add(newInteractableBase);
            Logger.Log( InteractionLogCategory.InteractionQueue,
                $"interaction Stack add {_currentInteractable} interactionStack.Count {_interactionStack.Count}");
        }

        public async UniTask StartNewInteraction(InteractableBase newInteractableBase)
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
    
        
        /// <summary>
        /// push on interaction stack but  keep current interaction running
        /// passed interaction will be started when the current one is done
        /// If you want to start an interaction in the OnComplete of another, this is what you use
        /// </summary>
        /// <param name="interactableBase"></param>
        public void QueueInteraction(InteractableBase interactableBase)
        {
            if (_currentInteractable == null && _interactionStack.Count <= 0)
            {
                StartNewInteraction(interactableBase).Forget();
            }
            else
            {
                AddInteraction(interactableBase);
            }
        }
        
        /// <summary>
        /// Run interaction without pushing onto stack
        /// Note: this doesn't call onInteractionChainStarted or ended
        /// </summary>
        /// <param name="interactableBase"></param>
        public void RunIndependentInteraction(InteractableBase interactableBase)
        {
            Logger.Log(InteractionLogCategory.InteractionStart, $"independent interaction: {interactableBase}", interactableBase);
            interactableBase.InitializeInteraction();
            interactableBase.DoInteraction(default);
            interactableBase.CompleteInteraction();
            Logger.Log(InteractionLogCategory.InteractionEnd,
                $"{interactableBase} interaction task end", interactableBase);
        }

        
        protected override void Initialize()
        {
            InteractableIconTable= Resources.Load<InteractableHoverSpriteTable>("InteractionSystem/InteractableHoverSpriteTable");
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
        
        public void ClearShownInteractables()
        {
            _inputPromptsController.ClearInteractables();
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
        InteractionComplete = 1 << 4,
        InteractionStart = 1 << 5,
        InteractionEnd = 1 << 6,
        InteractionCancelled = 1 << 7,
        DoInteraction = 1 << 8,
        InteractionQueue = 1 << 9,
    }
}