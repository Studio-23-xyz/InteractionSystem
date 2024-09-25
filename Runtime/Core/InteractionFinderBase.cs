using System.Collections.Generic;
using Bdeshi.Helpers.Utility.Extensions;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Core
{
    /// <summary>
    /// Looks for Interactables on scene based on camera position and orientation
    /// </summary>
    public abstract class InteractionFinderBase: MonoBehaviour
    {
        public abstract List<InteractableBase> FindInteractables();
    }
}