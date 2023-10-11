using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.studio23.ss2.InteractionSystem23.Abstract
{
    public abstract class InputPromptsControllerBase : MonoBehaviour
    {
        public UnityEvent<InteractableBase> OnInteractableConfirmed;
        public UnityEvent<InteractableBase> OnInteractableConfirmationStarted;
        public UnityEvent<InteractableBase> OnInteractableConfirmationCancelled;

        public abstract void SetInteractables(List<InteractableBase> interactables);
    }
}