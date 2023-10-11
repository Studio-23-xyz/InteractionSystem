using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Data;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.UI
{
    public abstract class InputPromptsViewBase : MonoBehaviour
    {
        public abstract InteractButtonPromptViewBase GetPromptForButton(InputButtonSlot buttonSlot);
        public abstract void UpdatePromptsView(InputPromptsModel model);
        public abstract void HidePrompts();
        public abstract void ShowPrompts();
    }
}