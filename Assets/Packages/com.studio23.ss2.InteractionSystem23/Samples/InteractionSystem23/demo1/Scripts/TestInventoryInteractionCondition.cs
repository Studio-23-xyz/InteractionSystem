using Studio23.SS2.InteractionSystem23.Abstract;
using Studio23.SS2.InteractionSystem23.Core;
using Studio23.SS2.InteractionSystem23.Data;
using System;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem23.Samples.Demo1
{
    [Serializable]
    public class TestInventoryInteractionCondition:InteractionCondition
    {
        [Tooltip("If checked, evaluate if inventory has item, else evaluate if inventory doesn't have it.")]
        [SerializeField] bool _checkIfHasItem = true;

        [SerializeField] private InteractionConditionResult _resultIfConditionPassed = InteractionConditionResult.Show;
        [SerializeField] private InteractionConditionResult _resultIfConditionFailed = InteractionConditionResult.Disable;

        [SerializeField] string _itemName = "Shinpachi's Shinpachis";
        public override InteractionConditionResult Evaluate(PlayerInteractionFinder playerInteractionFinder)
        {
            bool hasItem = TestInventory.Instance.ContainsItem(_itemName);
            return hasItem? _resultIfConditionPassed: _resultIfConditionFailed;
        }
    }
}