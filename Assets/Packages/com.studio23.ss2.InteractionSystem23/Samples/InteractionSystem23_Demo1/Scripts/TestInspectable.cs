using com.studio23.ss2.InteractionSystem23.Abstract;
using com.studio23.ss2.InteractionSystem23.Core;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.Samples.Demo1
{
    public class TestInspectable:InspectableBase
    {
        Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
        }

        protected override void Initialize()
        {
            
        }

        protected override void HandleInteractionInitialize()
        {
            _collider.gameObject.SetActive(false);
            _inspectionTarget.gameObject.SetActive(false);
            
            InspectionManager.Instance.HandleInteractionInitialize(this);
        }

        protected override void HandleInteractionPause()
        {
            InspectionManager.Instance.HandleInspectablePaused(this);
        }

        protected override void HandleInteractionResumed()
        {
            InspectionManager.Instance.HandleInspectableResumed(this);
        }

        protected override void HandleInteractionCompleted()
        {
            _collider.gameObject.SetActive(true);
            _inspectionTarget.gameObject.SetActive(true);
            InspectionManager.Instance.HandleInspectableCompleted(this);
        }

        public override string GetPromptSuffix()
        {
            return _objectName;
        }
    }
}