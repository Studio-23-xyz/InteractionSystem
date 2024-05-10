using DG.Tweening;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public class InteractionBillboardBehaviour : MonoBehaviour
    {


        [SerializeField]
        private float maxHeight = 0.3f;

        public void StartAnimating()
        {
            Vector3 initialPosition = transform.position;
            var targetPosition = initialPosition + Vector3.up * maxHeight;
            transform.DOMove(targetPosition, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            StartAnimating();
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}