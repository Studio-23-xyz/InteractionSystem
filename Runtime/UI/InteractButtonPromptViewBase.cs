using Studio23.SS2.InteractionSystem.Abstract;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public abstract class InteractButtonPromptViewBase : MonoBehaviour
    {
        public InteractableBase CurInteractable { get; protected set; }
        public abstract void showInteractable(InteractableBase interactableBase);
        public abstract void ToggleProgressSlider(bool shouldShowSlider);
        public abstract void SetProgress(float progress);
    }
}