using com.bdeshi.helpers.Input;
using Studio23.SS2.InteractionSystem23.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem23.UI
{
    public abstract class InputPromptsViewBase : MonoBehaviour
    {
        public abstract InteractButtonPromptViewBase GetPromptForButton(InputButtonSlot buttonSlot);
        public abstract void UpdatePromptsView(InputPromptsModel model);
        public abstract void HidePrompts();
        public abstract void ShowPrompts();
    }
}