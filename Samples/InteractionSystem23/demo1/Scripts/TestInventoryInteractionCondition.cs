using com.studio23.ss2.InteractionSystem23.Abstract;
using com.studio23.ss2.InteractionSystem23.Core;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.Samples.Demo1
{
    public class TestInventoryInteractionCondition:InteractionCondition
    {
        [Tooltip("If checked, evaluate if inventory has item, else evaluate if inventory doesn't have it.")]
        [SerializeField] bool _checkIfHasItem = true;
        [SerializeField] string _itemName = "Shinpachi's Shinpachis";
        public override bool Evaluate(PlayerInteractionFinder playerInteractionFinder)
        {
            bool hasItem = TestInventory.Instance.ContainsItem(_itemName);
            return hasItem == _checkIfHasItem;
        }
    }
}