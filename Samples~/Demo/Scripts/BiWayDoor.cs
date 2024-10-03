using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class BiWayDoor:TogglableBase
    {
        public float DoorAnimTime = 1.2f;
        public GameObject DoorObject;
        private Vector3 _closedEulerAngles;
        private Vector3 _openedEulerAngles;
        [SerializeField] private float _doorOpenAngle = 90;
        [SerializeField] private float _disabledDoorPunchAmount = .0315f;

        
        protected override void Initialize()
        {
            _closedEulerAngles = transform.rotation.eulerAngles;
            _openedEulerAngles = _closedEulerAngles + Vector3.up * _doorOpenAngle;
            base.Initialize();
        }

        public override void SnapToActivatedState()
        {
            DoorObject.transform.rotation = Quaternion.Euler(_openedEulerAngles);
        }

        public override void SnapToDeactivatedState()
        {
            DoorObject.transform.rotation = Quaternion.Euler(_closedEulerAngles);
        }

        protected override void HandleInteractionStarted()
        {
            
        }

        protected override void HandleInteractionPause()
        {
            
        }

        protected override void HandleInteractionResumed()
        {
            
        }

        protected override void HandleInteractionCompleted()
        {
            
        }

        protected override async UniTask DoActivateInteraction(CancellationToken token)
        {
            //basic door opening
            await AnimateDoor(_openedEulerAngles)
                .WithCancellation(token);
        }
        
        protected override async UniTask DoDeactivateInteraction(CancellationToken token)
        {
            //basic door opening
            await AnimateDoor(_closedEulerAngles)
                .WithCancellation(token);
        }

        Tween AnimateDoor(Vector3 endAngles)
        {
            return DoorObject.transform.DORotate(endAngles, DoorAnimTime)
                .SetEase(Ease.Linear);
        }
    }
}