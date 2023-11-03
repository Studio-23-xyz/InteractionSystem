using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2.InteractionSystem23.Abstract
{
    public abstract class InputPromptsControllerBase : MonoBehaviour
    {
        public UnityEvent<InteractableBase> OnInteractableConfirmed;
        public UnityEvent<InteractableBase> OnInteractableConfirmationStarted;
        public UnityEvent<InteractableBase> OnInteractableConfirmationCancelled;

        public abstract void SetInteractables(List<InteractableBase> interactables);
    }
}