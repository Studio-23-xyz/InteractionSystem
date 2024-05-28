using System;
using System.Collections.Generic;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Data
{
    
    public class InteractableHoverSpriteTable : ScriptableObject
    {
        public List<HoverSpriteData> Icons;


        public HoverSpriteData GetHoverSpriteData(string name)
        {
            return Icons.Find(i => i.Name == name);
        }

    }

    [Serializable]
    public class HoverSpriteData
    {
        public string Name;
        public Sprite Sprite;
    }


}