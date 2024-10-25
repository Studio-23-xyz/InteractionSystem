using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class TestItemPickup:ItemPickupBase
    {
        [SerializeField] private bool _canBeInterrupted = false;
        [SerializeField] private string _pickUpName = "Shinpachi's shinpachis";
        [SerializeField] private LocalizedString PromptPrefix;
        [SerializeField] private LocalizedString PromptSuffix;

        public override LocalizedString GetLocalizedPromptPrefix()
        {
            return PromptPrefix;
        }

        public override LocalizedString GetLocalizedPromptSuffix()
        {
            return PromptSuffix;
        }

        public override bool CanBeInterrupted => _canBeInterrupted;
        public override string PickUpName => _pickUpName;
        
        protected override void Initialize()
        {
            
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

        public override void AddPickupToInventory()
        {
            base.AddPickupToInventory();
            TestInventory.Instance.AddItem(this.PickUpName);
        }

        public override async UniTask DoInteraction(CancellationToken token)
        {
            AddPickupToInventory();
            
            await UniTask.Yield();  
        }

    }
}