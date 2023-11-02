using System.Threading;
using Studio23.SS2.InteractionSystem23.Abstract;
using Cysharp.Threading.Tasks;

namespace Studio23.SS2.InteractionSystem23.Samples.Demo1
{
    /// <summary>
    /// A basic switch that sends activation and deactivation events instantly
    /// </summary>
    public abstract class InstantSwitch : TogglableBase
    {
        protected override void HandleInteractionStarted()
        {
            
        }

        protected override void HandleInteractionPause()
        {
            
        }

        protected override void HandleInteractionResumed()
        {
         
        }
        protected override void HandleInteractionCompleted()
        {
            
        }
        
        protected override async UniTask DoActivateInteraction(CancellationToken cancellationToken)
        {
            await UniTask.Yield(cancellationToken);
        }

        protected override async UniTask DoDeactivateInteraction(CancellationToken cancellationToken)
        {
            await UniTask.Yield(cancellationToken);
        }
    }
}