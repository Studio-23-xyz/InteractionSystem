using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class ClosableDoor: ClosableBase
    {
        [SerializeField] float _doorAnimTime = 1.2f;
        [SerializeField] GameObject _doorObject;
        [SerializeField] float _disabledDoorPunchAmount = .0315f;
        private Vector3 _closedEulerAngles;
        private Vector3 _opendedEulerAngles;

        protected  void Awake()
        {
            _closedEulerAngles = transform.rotation.eulerAngles;
            _opendedEulerAngles = _closedEulerAngles + Vector3.up * 90;
        }

        Tween AnimateDoor(Vector3 endAngles)
        {
            return _doorObject.transform.DORotate(endAngles, _doorAnimTime)
                .SetEase(Ease.Linear);
        }

        protected override async UniTask DoCloseInteraction(CancellationToken token)
        {
            await AnimateDoor(_closedEulerAngles)
                .WithCancellation(token);
        }
      
        public override void SnapToOpenState()
        {
            base.SnapToOpenState();
            _doorObject.transform.rotation = Quaternion.Euler(_opendedEulerAngles);
        }
        
        public override void SnapToClosedState()
        {
            base.SnapToClosedState();
            _doorObject.transform.rotation = Quaternion.Euler(_closedEulerAngles);
        }

        public override string GetPromptSuffix()
        {
            return _objectName;
        }

        public override UniTask DoDisabledInteraction(CancellationToken token)
        {
            Debug.Log(this + " Door is disabled " , this);
            return _doorObject.transform.DOShakePosition(_doorAnimTime *.5f, Vector3.one * _disabledDoorPunchAmount)
                .SetEase(Ease.OutCirc)
                .WithCancellation(token);
        }
    }
}