using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class TestInspectable:InspectableBase
    {
        Collider _collider;
        [SerializeField] private Sprite _markerIcon;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
        }

        protected override void Initialize()
        {
            
        }

        protected override void HandleInteractionStarted()
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

        public override async UniTask DoDisabledInteraction(CancellationToken token)
        {
            Debug.Log("can't inspect " + _objectName);

            await UniTask.Yield();
        }
    }
}