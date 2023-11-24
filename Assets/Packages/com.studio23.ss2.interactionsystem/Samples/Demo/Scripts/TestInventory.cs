using System.Collections.Generic;
using com.bdeshi.helpers.Utility;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Samples.Demo1
{
    public class TestInventory:MonoBehaviourSingletonPersistent<TestInventory>
    {
        HashSet<string> _items;
        protected override void initialize()
        {
            _items = new HashSet<string>();
        }

        public void AddItem(string item)
        {
            _items.Add(item);
        }

        public void RemoveItem(string item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
            }
            else
            {
                Debug.Log(item  + " not in inventory");
            }
        }

        public bool ContainsItem(string item)
        {
            return _items.Contains(item);
        }
    }
}