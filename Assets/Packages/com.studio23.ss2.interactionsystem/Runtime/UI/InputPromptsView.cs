using System.Collections.Generic;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Core;
using Studio23.SS2.InteractionSystem.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public class InputPromptsView:InputPromptsViewBase
    {
        [SerializeField] private Transform _promptsHolder;
        [SerializeField] private InteractButtonPromptViewBase _togglePromptView;
        [SerializeField] private InteractButtonPromptViewBase _pickupPromptView;
        [SerializeField] private InteractButtonPromptViewBase _inspectPromptView;
        private Dictionary<InputButtonSlot, InteractButtonPromptViewBase> _promptsMap;

        public override InteractButtonPromptViewBase GetPromptForButton(InputButtonSlot buttonSlot)
        {
            return _promptsMap[buttonSlot];
        }

        public override void UpdatePromptsView(InputPromptsModel model)
        {
            foreach (var kv in _promptsMap)
            {
                var promptView = _promptsMap[kv.Key];
                var prevInteractableUnderButton = promptView.CurInteractable;

                if (model.TryGetRegisteredInteractable(kv.Key, out var interactable))
                {
                    promptView.gameObject.SetActive(true);
                    promptView.showInteractable(interactable);
                }
                else
                {
                    promptView.gameObject.SetActive(false);
                }
                UpdateCurInteractableUnderButton(promptView,prevInteractableUnderButton, interactable);
            }
        }

        private void UpdateCurInteractableUnderButton(InteractButtonPromptViewBase promptView, InteractableBase prevInteractable, InteractableBase curInteractable)
        {
            if (curInteractable != prevInteractable)
            {
                if (prevInteractable != null)
                {
                    prevInteractable.HandleHoveredEnd();
                }
                
                if (curInteractable != null)
                {
                    curInteractable.HandleHoveredStart();
                }
                
                promptView.showInteractable(curInteractable);
            }
        }

        private void Start()
        {
            _promptsMap = new Dictionary<InputButtonSlot, InteractButtonPromptViewBase>
            {
                [InteractionManager.Instance.InputHandler.InspectButton] = _inspectPromptView,
                [InteractionManager.Instance.InputHandler.PickupButton] = _pickupPromptView,
                [InteractionManager.Instance.InputHandler.ToggleButton] = _togglePromptView
            };
            foreach (var buttonPrompt in _promptsMap.Values)
            {
                buttonPrompt.gameObject.SetActive(false);
            }
        }
        
        public override void HidePrompts()
        {
            gameObject.SetActive(false);
        }

        public override void ShowPrompts()
        {
            gameObject.SetActive(true);
        }
    }
}