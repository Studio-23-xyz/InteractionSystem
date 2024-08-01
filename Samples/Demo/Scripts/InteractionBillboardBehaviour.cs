using DG.Tweening;
using Studio23.SS2.InteractionSystem.Abstract;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public class InteractionBillboardBehaviour : InteractionMarkerBase
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float maxHeight = 0.3f;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void StartAnimating()
        {
            var initialLocalPosition = transform.localPosition;
            var targetLocalPosition = initialLocalPosition + Vector3.up * maxHeight;
            transform.DOLocalMove(targetLocalPosition, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        private void OnEnable()
        {
            StartAnimating();
        }

        private void OnDisable()
        {
            transform.DOKill();
        }

        public override void Show(InteractableBase Interactable)
        {
            gameObject.SetActive(true);
            _spriteRenderer.sprite = Interactable.HoverIcon;
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}