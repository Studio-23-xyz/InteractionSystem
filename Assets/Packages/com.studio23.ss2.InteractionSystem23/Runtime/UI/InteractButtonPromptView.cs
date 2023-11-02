using Studio23.SS2.InteractionSystem23.Abstract;
using Studio23.SS2.InteractionSystem23.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio23.SS2.InteractionSystem23.UI
{
    public class InteractButtonPromptView:InteractButtonPromptViewBase
    {
        [SerializeField] TextMeshProUGUI _promptTMP;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private GameObject _disabledIndicator;
        

        public override void showInteractable(InteractableBase interactableBase)
        {
            _promptTMP.text = interactableBase.GetPromptPrefix() + ":"+ interactableBase.GetPromptSuffix();
            _disabledIndicator.gameObject.SetActive(interactableBase.LastEvaluationResult != InteractionConditionResult.Show);
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