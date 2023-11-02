using System.Threading;
using com.bdeshi.helpers.Input;
using Studio23.SS2.InteractionSystem23.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.InteractionSystem23.Abstract
{
    public abstract class InspectableBase : InteractableBase
    {
        [SerializeField] protected string _objectName = "Shinpachi's shinpachis";
        [SerializeField] 
        protected Transform _inspectionTarget;
        [SerializeField] 
        protected float _inspectionRotationSensitivity = 1;
        [SerializeField] 
        protected Vector3 _inspectionOffset = new Vector3(0,1.75f,1);

        [SerializeField] float _inspectionScaleMultiplier = 1;
        [SerializeField] Vector3 _originalInspectionTargetScale;
        [SerializeField] float _inspectionTargetMaxOffset = 3;
        [SerializeField] bool _canBeInterrupted = true;
        [SerializeField] float _inspectionTargetMaxZoom = 1;

        public Vector3 OriginalInspectionTargetScale => _originalInspectionTargetScale;
        public Vector3 InspectionScale => _originalInspectionTargetScale * _inspectionScaleMultiplier;
        public Vector3 InspectionOffset => _inspectionOffset;
        public float InspectionTargetMaxOffset => _inspectionTargetMaxOffset;
        public float InspectionTargetMaxZoom => _inspectionTargetMaxZoom;
        public float InspectionRotationSensitivity => _inspectionRotationSensitivity;
        public override InputButtonSlot InputButton => InteractionInputManager.Instance.InspectButton;
        public override bool CanBeInterrupted => _canBeInterrupted;

        public override string GetPromptPrefix() => "Inspect";

        protected virtual void Awake()
        {
            _originalInspectionTargetScale = _inspectionTarget.localScale;
        }

        public Transform GetInspectionTarget()
        {
            return _inspectionTarget;
        }
        
        public override async UniTask DoNormalInteraction(CancellationToken token)
        {
            await InspectionManager.Instance.ShowInspectable(this, token);
        }
    }
}