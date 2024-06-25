using Studio23.SS2.InteractionSystem.Abstract;   
using Studio23.SS2.InteractionSystem.Core;
using Studio23.SS2.InteractionSystem.Data;
using System;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    [Serializable]
    public class TestInventoryInteractionCondition:InteractionCondition
    {
        [Tooltip("If checked, evaluate if inventory has item, else evaluate if inventory doesn't have it.")]
        [SerializeField] bool _checkIfHasItem = true;
        [SerializeField] private InteractionConditionResult _resultIfConditionPassed = InteractionConditionResult.Show;
        [SerializeField] private InteractionConditionResult _resultIfConditionFailed = InteractionConditionResult.Disable;
        [SerializeField] string _itemName = "Shinpachi's Shinpachis";
        public override InteractionConditionResult Evaluate()
        {
            bool hasItem = TestInventory.Instance.ContainsItem(_itemName);
            return hasItem? _resultIfConditionPassed: _resultIfConditionFailed;
        }
    }
}