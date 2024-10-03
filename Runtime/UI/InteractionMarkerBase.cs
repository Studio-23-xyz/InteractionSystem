using Studio23.SS2.InteractionSystem.Abstract;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public abstract class InteractionMarkerBase : MonoBehaviour
    {
        public abstract void Show(InteractableBase Interactable);
        public abstract void Hide();
    }
}