using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class OpenableDoor: OpenableBase
    {
        public override string Name => "OpenableDoor";
        public float DoorAnimTime = 1.2f;
        public GameObject DoorObject;
        private Vector3 _closedEulerAngles;
        private Vector3 _openedEulerAngles;
        [SerializeField] private float _doorOpenAngle = 90;
        [SerializeField] private float _disabledDoorPunchAmount = .0315f;
        [SerializeField] private Sprite _markerIcon;
        
        protected override async UniTask DoOpenInteraction(CancellationToken token)
        {
            await AnimateDoor(_openedEulerAngles)
                .WithCancellation(token);
        }
        
        Tween AnimateDoor(Vector3 endAngles)
        {
            return DoorObject.transform.DOLocalRotate(endAngles, DoorAnimTime)
                .SetEase(Ease.Linear);
        }

        public override void SnapToOpenState()
        {
            base.SnapToOpenState();
            DoorObject.transform.localRotation = Quaternion.Euler(_openedEulerAngles);
        }
        
        public override void SnapToClosedState()
        {
            base.SnapToClosedState();
            DoorObject.transform.localRotation = Quaternion.Euler(_closedEulerAngles);
        }

        protected override void Initialize()
        {
            _closedEulerAngles = DoorObject.transform.localRotation.eulerAngles;
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

            return DoorObject.transform.DOShakePosition(DoorAnimTime*.5f, Vector3.one * _disabledDoorPunchAmount)
                .SetEase(Ease.OutCirc)
                .WithCancellation(token);
        }
    }
}