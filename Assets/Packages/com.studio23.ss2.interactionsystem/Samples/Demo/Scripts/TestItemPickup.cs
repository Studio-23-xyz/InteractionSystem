using System.Threading;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class TestItemPickup:ItemPickupBase
    {
        [SerializeField] private bool _canBeInterrupted = false;
        [SerializeField] private string _pickUpName = "Shinpachi's shinpachis";

        public override bool CanBeInterrupted => _canBeInterrupted;
        public override string PickUpName => _pickUpName;
        [SerializeField] private Sprite _markerIcon;
        public override string Name => "TestItemPickup";

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

        public override async UniTask DoNormalInteraction(CancellationToken token)
        {
            AddPickupToInventory();
            
            await UniTask.Yield();  
        }

        public override async UniTask  DoDisabledInteraction(CancellationToken token)
        {
            Debug.Log("can't add " + _pickUpName + " to inventory");
            
            await UniTask.Yield();  
        }

    }
}