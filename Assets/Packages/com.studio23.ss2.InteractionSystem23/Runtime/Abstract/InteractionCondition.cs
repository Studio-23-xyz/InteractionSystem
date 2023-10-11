using System;
using com.studio23.ss2.InteractionSystem23.Core;

namespace com.studio23.ss2.InteractionSystem23.Abstract
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
        public abstract bool Evaluate(PlayerInteractionFinder playerInteractionFinder);
    }
}