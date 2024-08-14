using System.Threading;
using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class InspectableBase : InteractableBase
    {
        [SerializeField] 
        protected string _objectName = "Shinpachi's shinpachis";
        [SerializeField] 
        protected Transform _inspectionTarget;
        [SerializeField] 
        protected float _inspectionRotationSensitivity = 1;
        [SerializeField] 
        protected Vector3 _inspectionPosOffset = new Vector3(0,0,0);
        [SerializeField] 
        protected Vector3 _inspectionRotOffset = new Vector3(0,0,0);

        [SerializeField] float _inspectionScaleMultiplier = 1;
        [SerializeField] Vector3 _inspectionTargetScale = Vector3.one;
        [SerializeField] float _inspectionTargetMaxOffset = 3;
        [SerializeField] bool _canBeInterrupted = true;
        [SerializeField] float _inspectionTargetMaxZoom = 1;

        public Vector3 OriginalInspectionTargetScale => _inspectionTargetScale;
        public Vector3 InspectionScale => _inspectionTargetScale * _inspectionScaleMultiplier;
        public Vector3 InspectionPosOffset => _inspectionPosOffset;
        public Vector3 InspectionRotOffset => _inspectionRotOffset;
        public float InspectionTargetMaxOffset => _inspectionTargetMaxOffset;
        public float InspectionTargetMaxZoom => _inspectionTargetMaxZoom;
        public float InspectionRotationSensitivity => _inspectionRotationSensitivity;
        public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.InspectButton;
        public override bool CanBeInterrupted => _canBeInterrupted;
        public virtual bool CanExitInspection => true;
        public virtual bool ForceExitInspection => false;
        public override string GetPromptPrefix() => "Inspect";

        protected virtual void Awake()
        {
            _inspectionTargetScale = _inspectionTarget.localScale;
        }

        public Transform GetInspectionTarget()
        {
            return _inspectionTarget;
        }


        protected override void HandleInteractionStarted()
        {
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
            Debug.Log("completed "  + this, this);
            _inspectionTarget.gameObject.SetActive(true);
            InspectionManager.Instance.HandleInspectableCompleted(this);
        }

        public override async UniTask DoInteraction(CancellationToken token)
        {
            await InspectionManager.Instance.ShowInspectable(this, token);
        }
        
        public virtual void HandleRotation(bool isRotationActive)
        {
            
        }
        

        public void ToggleCamStacking(bool shouldCamStackingBeActive)
        {
            if (shouldCamStackingBeActive)
            {
                InspectionManager.Instance.ActivateInspectionCamStacking();
            }
            else
            {
                InspectionManager.Instance.DeactivateInspectionCamStacking();
            }
        }
    }
}