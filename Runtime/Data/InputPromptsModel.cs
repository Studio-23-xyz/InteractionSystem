using System;
using System.Collections.Generic;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Abstract;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Data
{
    public class InputPromptsModel:MonoBehaviour
    {
        public event Action<InputPromptsModel> OnPromptsChanged;
        private Dictionary<InputButtonSlot, InteractableBase> _registeredInteractables = new Dictionary<InputButtonSlot, InteractableBase>();
        public IEnumerable<InteractableBase> RegisteredInteractables => _registeredInteractables.Values;
        
        public bool TryGetRegisteredInteractable(InputButtonSlot button, out InteractableBase interactableBase)
        {
            return _registeredInteractables.TryGetValue(button, out interactableBase);
        }

        public void SetInputPrompts(List<InteractableBase> prompts)
        {
            _registeredInteractables.Clear();
            foreach (var interactable in prompts)
            {
                _registeredInteractables[interactable.InputButton] = interactable;
            }
            OnPromptsChanged?.Invoke(this);
        }

        [ContextMenu("Print Input promtps")]
        public void LogInputPrompts()
        {
            Debug.Log($"InputPrompts x {_registeredInteractables.Count}", this);
            foreach ((var Button, var registeredInteractable) in _registeredInteractables)
            {
                Debug.Log($"{Button.ButtonName}: {registeredInteractable}");
            }
        }

        public void ClearPrompts()
        {
            _registeredInteractables.Clear();
            OnPromptsChanged?.Invoke(this);
        }
    }
}