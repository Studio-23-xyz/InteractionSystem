using Bdeshi.Helpers.Input;
using Studio23.SS2.InteractionSystem.Core;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Abstract
{
    public abstract class ItemPickupBase : InteractableBase
    {
        public abstract string PickUpName { get; }

        protected override void HandleInteractionCompleted()
        {
              // gameObject.SetActive(false);
              Destroy(gameObject);
        }

        public override string GetPromptPrefix() => "Pickup";

        public override string GetPromptSuffix() => PickUpName;
        
        public virtual void AddPickupToInventory()
        {
            Debug.Log("item added to inventory" + this, this);
        }

        public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.PickupButton;
    }
}