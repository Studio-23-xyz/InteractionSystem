using com.bdeshi.helpers.Input;
using com.studio23.ss2.InteractionSystem23.Core;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.Abstract
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
        public override string GetPromptSuffix()
        {
            return PickUpName;
        }
        
        public virtual void AddPickupToInventory()
        {
            Debug.Log("item added to inventory" + this, this);
        }

        public override InputButtonSlot InputButton => InteractionInputManager.Instance.PickupButton;
    }
}