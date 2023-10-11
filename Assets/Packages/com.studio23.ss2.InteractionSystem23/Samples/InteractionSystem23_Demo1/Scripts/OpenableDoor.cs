using System.Threading;
using com.studio23.ss2.InteractionSystem23.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.studio23.ss2.InteractionSystem23.Samples.Demo1
{
    public class OpenableDoor: OpenableBase
    {
        [FormerlySerializedAs("doorAnimTime")] public float _doorAnimTime = 1.2f;
        [FormerlySerializedAs("doorObject")] public GameObject _doorObject;
        private Vector3 _closedEulerAngles;
        private Vector3 _opendedEulerAngles;
        [SerializeField] private float _doorOpenAngle = 90;
        protected  void Awake()
        {
            _closedEulerAngles = _doorObject.transform.localRotation.eulerAngles;
            _opendedEulerAngles = _closedEulerAngles + Vector3.up * _doorOpenAngle;
        }
        
        
        protected override async UniTask DoOpenInteraction(CancellationToken token)
        {
            await AnimateDoor(_opendedEulerAngles)
                .WithCancellation(token);
        }
        
        Tween AnimateDoor(Vector3 endAngles)
        {
            return _doorObject.transform.DOLocalRotate(endAngles, _doorAnimTime)
                .SetEase(Ease.Linear);
        }

        public override void SnapToOpenState()
        {
            base.SnapToOpenState();
            _doorObject.transform.localRotation = Quaternion.Euler(_opendedEulerAngles);
        }
        
        public override void SnapToClosedState()
        {
            base.SnapToClosedState();
            _doorObject.transform.localRotation = Quaternion.Euler(_closedEulerAngles);
        }

        protected override void Initialize()
        {
            
        }

        public override string GetPromptSuffix()
        {
            return _objectName;
        }
    }
}