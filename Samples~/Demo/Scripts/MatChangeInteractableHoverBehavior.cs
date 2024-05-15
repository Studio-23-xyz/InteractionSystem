using System;
using Studio23.SS2.InteractionSystem.Abstract;
using UnityEngine;

namespace  Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class MatChangeInteractableHoverBehavior:InteractableHoverBehaviorBase
    {
        private Renderer _renderer;
        public Material NormalMat;
        public Material HoverMat;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            NormalMat = _renderer.sharedMaterial;
        }

        public override void HandleHoverStarted()
        {
            _renderer.sharedMaterial = HoverMat;
        }

        public override void HandleHoverEnded()
        {
            _renderer.sharedMaterial = NormalMat;
        }
    }
}