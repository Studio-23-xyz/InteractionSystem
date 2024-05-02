using Studio23.SS2.InteractionSystem.Core;
using Studio23.SS2.InteractionSystem.Samples.Demo1;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1.Scripts
{
    public class TestPlayerController:MonoBehaviour
    {
        private PlayerInteractionFinder _interactionFinder;
        private TestCharacterController _controller;
        [SerializeField] private bool _isPaused = false;

        private void HandleInteractionStart()
        {
            _isPaused = true;
            _controller.Toggle(false);
        }

        private void HandleInteractionEnded()
        {
            _isPaused = false;
            _controller.Toggle(true);
        }
        
        private void LookForInteractables()
        {
            var interactables = _interactionFinder.FindInteractables();
            InteractionManager.Instance.ShowNewInteractables(interactables);
        }

        private void Update()
        {
            if (_isPaused)
            {
                return;
            }

            LookForInteractables();
        }

        private void Awake()
        {
            _interactionFinder = GetComponent<PlayerInteractionFinder>();
            _controller = GetComponent<TestCharacterController>();
        }
        
        private void Start()
        {
            InteractionManager.Instance.OnInteractionChainStarted += HandleInteractionStart;
            InteractionManager.Instance.OnInteractionChainEnded += HandleInteractionEnded;
        }

        private void OnDisable()
        {
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.OnInteractionChainStarted -= HandleInteractionStart;
                InteractionManager.Instance.OnInteractionChainEnded -= HandleInteractionEnded;
            }
        }
    }
    
}