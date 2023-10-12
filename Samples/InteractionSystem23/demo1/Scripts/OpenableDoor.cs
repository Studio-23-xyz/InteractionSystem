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
        private Vector3 _openedEulerAngles;
        [SerializeField] private float _doorOpenAngle = 90;
        [SerializeField] private float _disabledDoorPunchAmount = .0315f;

        protected override async UniTask DoOpenInteraction(CancellationToken token)
        {
            await AnimateDoor(_openedEulerAngles)
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
            _doorObject.transform.localRotation = Quaternion.Euler(_openedEulerAngles);
        }
        
        public override void SnapToClosedState()
        {
            base.SnapToClosedState();
            _doorObject.transform.localRotation = Quaternion.Euler(_closedEulerAngles);
        }

        protected override void Initialize()
        {
            _closedEulerAngles = _doorObject.transform.localRotation.eulerAngles;
            _openedEulerAngles = _closedEulerAngles + Vector3.up * _doorOpenAngle;
            base.Initialize();
        }

        public override string GetPromptSuffix()
        {
            return _objectName;
        }

        public override UniTask DoDisabledInteraction(CancellationToken token)
        {
            Debug.Log(this + " Door is disabled " , this);

            return _doorObject.transform.DOShakePosition(_doorAnimTime*.5f, Vector3.one * _disabledDoorPunchAmount)
                .SetEase(Ease.OutCirc)
                .WithCancellation(token);
        }
    }
}