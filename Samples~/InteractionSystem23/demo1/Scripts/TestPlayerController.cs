using com.studio23.ss2.InteractionSystem23.Core;
using com.studio23.ss2.InteractionSystem23.Samples.Demo1;
using UnityEngine;

namespace Packages.com.studio23.ss2.InteractionSystem23.Samples.Demo1.Scripts
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