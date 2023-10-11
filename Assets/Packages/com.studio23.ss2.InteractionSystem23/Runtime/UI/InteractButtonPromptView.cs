using com.studio23.ss2.InteractionSystem23.Abstract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.studio23.ss2.InteractionSystem23.UI
{
    public class InteractButtonPromptView:InteractButtonPromptViewBase
    {
        [SerializeField] TextMeshProUGUI _promptTMP;
        [SerializeField] private Slider _progressSlider;
        

        public override void SetPromptText(InteractableBase interactableBase)
        {
            _promptTMP.text = interactableBase.GetPromptPrefix() + ":"+ interactableBase.GetPromptSuffix();
        }
        
        public override void ToggleProgressSlider(bool shouldShowSlider)
        {
            _progressSlider.gameObject.SetActive(shouldShowSlider);
        }

        public override void SetProgress(float progress)
        {
            _progressSlider.value = progress;
        }
        
    }
}