using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public abstract class InputPromptsViewBase : MonoBehaviour
    {
        public abstract InteractButtonPromptViewBase GetPromptForButton(InputButtonSlot buttonSlot);
        public abstract void UpdatePromptsView(InputPromptsModel model);
        public abstract void HidePrompts();
        public abstract void ShowPrompts();
    }
}