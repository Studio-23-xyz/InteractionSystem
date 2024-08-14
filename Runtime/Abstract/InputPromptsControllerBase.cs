using System;
using System.Collections.Generic;
using Studio23.SS2.InteractionSystem.Data;
using Studio23.SS2.InteractionSystem.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class InputPromptsControllerBase : MonoBehaviour
    {
        public abstract InputPromptsModel Model { get; }
        public abstract InputPromptsViewBase View { get; }
        
        public UnityEvent<InteractableBase> OnInteractableConfirmationStarted;
        public UnityEvent<InteractableBase> OnInteractableConfirmationCancelled;
        public event Action<InputPromptsControllerBase> OnActionPromptsChanged;

        public abstract void SetInteractables(List<InteractableBase> interactables);
        public abstract void ClearInteractables();
    }
}