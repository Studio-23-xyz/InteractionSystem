using System;
using System.Collections.Generic;
using System.Threading;
using com.bdeshi.helpers.Utility;
using com.studio23.ss2.InteractionSystem23.Abstract;
using com.studio23.ss2.InteractionSystem23.Data;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Core
{
    public class InteractionManager:MonoBehaviourSingletonPersistent<InteractionManager>
    {
        [FormerlySerializedAs("interactionStack")] [SerializeField] 
        private List<InteractableBase> _interactionStack;
        [FormerlySerializedAs("_currentInteractable")] [FormerlySerializedAs("currentInteractable")] [SerializeField] 
        private InteractableBase _currentInteractableBase;
        [FormerlySerializedAs("inputPromptsController")] [SerializeField] 
        private InputPromptsControllerBase _inputPromptsController;
        CancellationTokenSource _subInteractionCancellationTokens;
        [CanBeNull] public InteractableBase CurrentInteractableBase => _currentInteractableBase;

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
        
        public bool IsRunningInteraction => CurrentInteractableBase != null;
        public bool _isDebug = false;

        public async UniTask DoInteraction()
        {
            OnInteractionChainStarted?.Invoke();

            while (_interactionStack.Count > 0)
            {
                //cache this so that we have the interrupted one after cancellation
                _currentInteractableBase = _interactionStack[^1];
                Dlog("Do interaction "+ _currentInteractableBase, _currentInteractableBase);
                //the state of the interactable is saved
                //if it was cancelled previously, we resume
                //else it is an interaction we are newly intializing 
                if (_currentInteractableBase.CurState == InteractionState.Paused)
                {
                    _currentInteractableBase.ResumeInteraction();
                }
                else
                {
                    _currentInteractableBase.InitializeInteraction();
                }

                var isCanceled = await _currentInteractableBase
                    .DoInteraction(_subInteractionCancellationTokens.Token)
                    .SuppressCancellationThrow();
                Dlog( _currentInteractableBase + " interation task end, Cancelled: " +  isCanceled, _currentInteractableBase);
                // check task status to handle cancellation
                if (isCanceled)
                {
                    // if the task has been cancelled
                    // we assume that a sub interaction was added
                    // cur.pause()
                    // Debug.Log("interaction pause  " + currentInteractable);
                    _currentInteractableBase.PauseInteraction();
                    RefreshCancellationToken();
                    Dlog("interaction pause " + _currentInteractableBase + " push new one " );

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
                    Dlog("interaction Stack pop " + _currentInteractableBase + " interactionStack.Count " + _interactionStack.Count);

                    _currentInteractableBase.CompleteInteraction();
                }
 
            }

            _currentInteractableBase = null;
            OnInteractionChainEnded?.Invoke();
        }

        void AddInteraction(InteractableBase newInteractableBase)
        {
            _interactionStack.Add(newInteractableBase);
            Dlog("add new interaction "+ newInteractableBase, newInteractableBase);
        }

        public async void StartNewInteraction(InteractableBase newInteractableBase)
        {
            if(_currentInteractableBase != null && !_currentInteractableBase.CanBeInterrupted)
            {
                // NOTE: This shouldn't even fire. 
                // unless someone allowed a subinteractable to proc when a non-interruptible was running
                Debug.LogWarning("Can't interrupt current interactable: "+ _currentInteractableBase, _currentInteractableBase);
                return;
            }
            Dlog(" start new interaction " + newInteractableBase);
            AddInteraction(newInteractableBase);

            if (_interactionStack.Count > 1)
            {
                Dlog("cancel()");
                _subInteractionCancellationTokens.Cancel();
            }
            else
            {
                await DoInteraction();
            }
        }
        
        protected override void initialize()
        {
            RefreshCancellationToken();
        }

        public void Dlog(string message, UnityEngine.Object context = null)
        {
            #if UNITY_EDITOR
            if (_isDebug)
            {
                Debug.Log(message, context);
            }
            #endif
            
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

        private void HandleInteractableConfirmationCompleted(InteractableBase obj)
        {
            StartNewInteraction(obj);
        }

        public void ShowNewInteractables(List<InteractableBase> interactables)
        {
            _inputPromptsController.SetInteractables(interactables);
        }
  
        private void HandleInteractableConfirmationStarted(InteractableBase obj)
        {
            if (!IsRunningInteraction)
            {
                OnInteractionChainStarted?.Invoke();
            }
        }
        
        private void HandleInteractableConfirmationCancelled(InteractableBase obj)
        {
            if (!IsRunningInteraction)
            {
                // we weren't running interactions
                // but we sent a OnInteractionChainStarted when the confirmation button hold started
                // we need to tell listeners that the interaction chain ended
                // IsRunningInteraction is true when a subinteraction is cancelled
                // so cancellations for a suninteraction aren't an issue
                OnInteractionChainEnded?.Invoke();
            }
        }
        
        private void Start()
        {
            _inputPromptsController.OnInteractableConfirmed.AddListener(HandleInteractableConfirmationCompleted); 
            _inputPromptsController.OnInteractableConfirmationStarted.AddListener(HandleInteractableConfirmationStarted);
            _inputPromptsController.OnInteractableConfirmationCancelled.AddListener(HandleInteractableConfirmationCancelled);
        }

        private void OnDestroy()
        {
            if (_subInteractionCancellationTokens != null)
            {
                _subInteractionCancellationTokens.Cancel();
                _subInteractionCancellationTokens.Dispose();
            }

            if (_inputPromptsController != null)
            {
                _inputPromptsController.OnInteractableConfirmed.RemoveListener(HandleInteractableConfirmationCompleted);
                _inputPromptsController.OnInteractableConfirmationStarted.RemoveListener(HandleInteractableConfirmationStarted);
                _inputPromptsController.OnInteractableConfirmationCancelled.RemoveListener(HandleInteractableConfirmationCancelled);
            }
        }
    }
}