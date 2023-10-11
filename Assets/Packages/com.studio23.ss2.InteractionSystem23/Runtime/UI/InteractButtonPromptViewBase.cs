using com.studio23.ss2.InteractionSystem23.Abstract;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.UI
{
    public abstract class InteractButtonPromptViewBase : MonoBehaviour
    {
        public abstract void SetPromptText(InteractableBase interactableBase);
        public abstract void ToggleProgressSlider(bool shouldShowSlider);
        public abstract void SetProgress(float progress);
    }
}