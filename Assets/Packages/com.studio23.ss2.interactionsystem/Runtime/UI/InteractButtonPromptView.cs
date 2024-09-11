using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio23.SS2.InteractionSystem.UI
{
    public class InteractButtonPromptView:InteractButtonPromptViewBase
    {
        [SerializeField] TextMeshProUGUI _promptTMP;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private GameObject _disabledIndicator;
        
        public override void showInteractable(InteractableBase interactableBase)
        {
            CurInteractable = interactableBase;
            if (interactableBase != null)
            {
                var localizedPromptPrefix = interactableBase.GetLocalizedPromptPrefix();
                var localizedPromptSuffix = interactableBase.GetLocalizedPromptSuffix();
                _promptTMP.text =   (localizedPromptPrefix.IsEmpty?"": localizedPromptPrefix.GetLocalizedString()) +
                                    ":"+ 
                                    (localizedPromptSuffix.IsEmpty?"": localizedPromptSuffix.GetLocalizedString());
                _disabledIndicator.gameObject.SetActive(interactableBase.LastEvaluationResult != InteractionConditionResult.Show);
            }
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