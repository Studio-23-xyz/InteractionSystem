using System;
using Studio23.SS2.InteractionSystem23.Core;
using Studio23.SS2.InteractionSystem23.Data;

namespace Studio23.SS2.InteractionSystem23.Abstract
{
    [Serializable]
    public abstract class InteractionCondition
    {
        /// <summary>
        /// Returns whether this condition is true for the interaction context
        /// For now we assume interaction context is just the player interaction finder
        /// </summary>
        /// <param name="playerInteractionFinder"></param>
        /// <returns></returns>
        public abstract InteractionConditionResult Evaluate(PlayerInteractionFinder playerInteractionFinder);
    }
}