using System.Threading;
using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Abstract
{
    public abstract class InspectableBase : InteractableBase
    {
        [FormerlySerializedAs("objectName")] [FormerlySerializedAs("ObjectName")] 
        public string _objectName = "Shinpachi's shinpachis";
        [FormerlySerializedAs("inspectionTarget")] [SerializeField] 
        protected Transform _inspectionTarget;
        [FormerlySerializedAs("inspectionRotationSensitivity")] [SerializeField] 
        protected float _inspectionRotationSensitivity = 1;
        [FormerlySerializedAs("inspectionOffset")] [SerializeField] 
        protected Vector3 _inspectionOffset = new Vector3(0,1.75f,1);
        
        [FormerlySerializedAs("inspectionScaleMultiplier")] 
        public float _inspectionScaleMultiplier = 1;
        [FormerlySerializedAs("originalInspectionTargetScale")] 
        [SerializeField] Vector3 _originalInspectionTargetScale;
        [FormerlySerializedAs("inspectionTargetMaxOffset")] 
        [SerializeField] float _inspectionTargetMaxOffset = 3;

        [FormerlySerializedAs("canBeInterrupted")] 
        [SerializeField] private bool _canBeInterrupted = true;

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