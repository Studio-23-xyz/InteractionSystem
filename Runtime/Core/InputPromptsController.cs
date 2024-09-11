using System.Collections.Generic;
using Bdeshi.Helpers.Utility;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using Studio23.SS2.InteractionSystem.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Core
{
    public class InputPromptsController: InputPromptsControllerBase
    {
        private InputPromptsModel _model;
        public override InputPromptsModel Model => _model;
        private InputPromptsViewBase _view;
        public override InputPromptsViewBase View => _view;
        [SerializeField] private FiniteTimer _remainingPressTimer = new FiniteTimer(-69);

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _model = GetComponent<InputPromptsModel>();
            _view = GetComponent<InputPromptsViewBase>();
            _model.OnPromptsChanged += _view.UpdatePromptsView;

            InteractionManager.Instance.InputHandler.InspectButton.AddPerformedCallback(gameObject, HandleInspectPressed);
            InteractionManager.Instance.InputHandler.PickupButton.AddPerformedCallback(gameObject, HandlePickupPressed);
            InteractionManager.Instance.InputHandler.ToggleButton.AddPerformedCallback(gameObject, HandleTogglePressed);
        }

        private async void HandleTogglePressed()
        {
            if (_model.TryGetRegisteredInteractable(
                InteractionManager.Instance.InputHandler.ToggleButton,
                    out var togglable))
            {
                await ConfirmInteraction(togglable);
            }
        }

        private async void HandlePickupPressed()
        {
            if (_model.TryGetRegisteredInteractable(
                InteractionManager.Instance.InputHandler.PickupButton,
                    out var pickupable))
            {
                await ConfirmInteraction(pickupable);
            }
        }

        private async void HandleInspectPressed()
        {
            if (_model.TryGetRegisteredInteractable(
                InteractionManager.Instance.InputHandler.InspectButton,
                    out var inspectable))
            {
                await ConfirmInteraction(inspectable);
            }
        }

        public async UniTask ConfirmInteraction(InteractableBase interactableBase)
        {
            OnInteractableConfirmationStarted?.Invoke(interactableBase);
            await DoHoldInteraction(interactableBase);

            // tell the interaction manager that an interaction has been confirmed
            if (_remainingPressTimer.isComplete)
            {
                _view.HidePrompts();
                _model.ClearPrompts();
                InteractionManager.Instance.StartNewInteraction(interactableBase);
            }
            else
            {
                OnInteractableConfirmationCancelled?.Invoke(interactableBase);
            }
        }

        private async UniTask DoHoldInteraction(InteractableBase interactableBase)
        {
            var buttonPrompt = _view.GetPromptForButton(interactableBase.InputButton);
            buttonPrompt.ToggleProgressSlider(true);
            buttonPrompt.SetProgress(0);

            _remainingPressTimer.reset(interactableBase.InteractionHoldTime);
            while (interactableBase.InputButton.IsHeld)
            {
                _remainingPressTimer.safeUpdateTimer(Time.deltaTime);
                buttonPrompt.SetProgress(_remainingPressTimer.Ratio);
                if (_remainingPressTimer.isComplete)
                {
                    break;
                }

                await UniTask.Yield();
                await UniTask.NextFrame();
            }
            buttonPrompt.SetProgress(0);
            buttonPrompt.ToggleProgressSlider(false);
        }
        public override void SetInteractables(List<InteractableBase> interactables)
        {
            _model.SetInputPrompts(interactables);
            _view.ShowPrompts();
        }

        public override void ClearInteractables()
        {
            _model.ClearPrompts();
        }
    }
}