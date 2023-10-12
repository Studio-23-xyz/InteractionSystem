using com.studio23.ss2.InteractionSystem23.Abstract;
using com.studio23.ss2.InteractionSystem23.Core;
using com.studio23.ss2.InteractionSystem23.Data;
using UnityEngine;

namespace com.studio23.ss2.InteractionSystem23.Samples.Demo1
{
    public class TestInventoryInteractionCondition:InteractionCondition
    {
        [Tooltip("If checked, evaluate if inventory has item, else evaluate if inventory doesn't have it.")]
        [SerializeField] bool _checkIfHasItem = true;

        [SerializeField] private InteractionConditionResult _resultIfHasItem = InteractionConditionResult.Show;
        [SerializeField] private InteractionConditionResult _resultIfMissingItem = InteractionConditionResult.Disable;
        [SerializeField] string _itemName = "Shinpachi's Shinpachis";
        public override InteractionConditionResult Evaluate(PlayerInteractionFinder playerInteractionFinder)
        {
            bool hasItem = TestInventory.Instance.ContainsItem(_itemName);
            return hasItem? _resultIfHasItem: _resultIfMissingItem;
        }
    }
}