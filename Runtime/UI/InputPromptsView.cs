using System.Collections.Generic;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using com.studio23.ss2.InteractionSystem23.Data;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.UI
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
                if (model.TryGetRegisteredInteractable(kv.Key, out var interactable))
                {
                    promptView.gameObject.SetActive(true);
                    promptView.showInteractable(interactable);
                }
                else
                {
                    promptView.gameObject.SetActive(false);
                }
            }
        }

        private void Start()
        {
            _promptsMap = new Dictionary<InputButtonSlot, InteractButtonPromptViewBase>
            {
                [InteractionInputManager.Instance.InspectButton] = _inspectPromptView,
                [InteractionInputManager.Instance.PickupButton] = _pickupPromptView,
                [InteractionInputManager.Instance.ToggleButton] = _togglePromptView
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