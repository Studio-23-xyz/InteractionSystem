using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class InteractableHoverBehaviorBase:MonoBehaviour
    {
        public abstract void HandleHoverStarted();
        public abstract void HandleHoverEnded();
    }
}