<h1 align="center">Interaction System</h1>
<p align="center">
<a href="https://openupm.com/packages/com.studio23.ss2.interactionsystem/"><img src="https://img.shields.io/npm/v/com.studio23.ss2.interactionsystem?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
</p>
"An interaction system that supports: Toggleable, Pickups, Inspectable etc. types of interactions.

## Installation

### Install via Git URL

You can also use the "Install from Git URL" option from Unity Package Manager to install the package.
```
https://github.com/Studio-23-xyz/InteractionSystem.git#upm
```
### Install from OpenUPM:
```
https://openupm.com/packages/com.studio23.ss2.interactionsystem/
```
## Usage
The samples scene contains an example setup. You will need in the scene:
1. An `InteractionManager` singleton.
2. An `InteractionPromptController` configured in the scene
3. The `InteractionPromptController` will require an `InteractionPromptModel` and `InteractionPromptView` configured in the scene.
4. A `PlayerInteractionFinder` to detect interactables in the scene.

#### Interactable state
Interactables can be in 3 states:
```
    public enum InteractionState{
        Inactive,//interaction hasn't started yet.
        Active,//runing interaction logic
        Paused,// running a sub-interaction or some other case when we don't want the interaction to run logic
    }
```
The `CurState` field returns the Interactable's current state. InteractableBase handles setting the value. So Custome Interactables inheriting from it don't need to manually set it.

#### SubInteractions
If you start an interaction while one is running, the old interaction is paused and the new interaction is started as a subinteraciton. When the subinteraction ends, the old interaction is resumed.

# Custom Interactions
You can define your own Interactable by inheriting from `InteractableBase`.
Be sure to use the cancelaltion token when overriding `DoNormalInteraction()` and `DoDisabledInteraction()` if you want subinteractions.

## Inheritance notes:

### CanBeInterrupted

If you want to push another interaction over an interaction(like dialogue during inspection),

`public abstract bool CanBeInterrupted { get; }`

should be overridden to true. Otherwise false.

### DoNormalInteraction(CancellationToken token)

The interaction ends immediately when the DoNormalInteraction returns. Write the async function that lasts as long as the interaction should last. Ex: If you play the door opening animation, you could make the async function wait until the door is played.&nbsp;

### InputButton:

Pick one of the buttons in InteractionManager.Instance.InputHandler

`public override InputButtonSlot InputButton => InteractionManager.Instance.InputHandler.ToggleButton;`

### Setup Functions

1.  `Interaction.HandleInteractionStarted()`
    
2.  `Interaction.HandleInteractionResumed()`
    
3.  `Interaction.HandleInteractionPause()`
    
4.  `Interaction.HandleInteractionCompleted()`
    
## Multiple interactions on same object depending on conditions:

Add InteractionConditions to the InteractionConditions list in the InteractableBase monobehavior. Interaction System goes through all Interactables in a given gameobject and picks the first interactable whose conditions return `InteractionConditionResult.Show`. The Interactable component order on the gameobject affects this.

Additionally if the Interactable is not enabled, it is skipped. So you can control which interactable gets picked by disabling them.

This may be overhauled in the future.

### Example:

We want to open the door if the player has the key, Otherwise we want to play a locked door rattling animation

#### Solution:

Make two interactables. One for opening. One for the locked door rattling interaction. Put both on same door object.

On the open door interaction, add a InteractionCondition that returns `InteractionConditionResult.show` if the player has the key.

No need for any conditions on the locked door interaction. It will be the &quot;default&quot; interaction. However, the component order should come after the open door interaction.

### Interaction conditions notes:

InteractionConditions return an `InteractionConditionResult` result with 3 values:

1.  Show: Shows the prompt  for the interactable when this result is returned
    
2.  Hide: Hides the prompt for the interactable when this result is returned
    
3.  Passthrough: Defers to next InteractionCondition in list when this result is returned. If this is the last InteractionCondition in the list, the final result becomes `InteractionConditionResult.Hide`  Ex: If Interaction1, Interaction2 is in list, and Interaction1 returns Passthrough
    
    1.  If interaction2 returns Show or Hide, the final result is show or hide accordingly
        
    2.  If interaction2 returns Passthrough, the final result is Hide.
        

You can write your own InteractionCondition in SS2 implementing the InteractionCondition class.

# Gotchas

1.  Be sure to use the cancellation token . Otherwise interrupt will not work
    
2.  `DoNormalInteraction()` can be called multiple times if the interaction can be interrupted.
    
    1.  Flow:
        
        1.  `Interaction.HandleInteractionStarted()`
            
        2.  `Interaction.DoNormalInteraction()`
            
        3.  Interrupt happens by starting a sub interaction
            
        4.  `Interaction.HandleInteractionPause()`
            
        5.  `SubInteraction.HandleInteractionStarted()`
            
        6.  `SubInteraction.DoNormalInteraction()`
            
        7.  Assuming the SubInteraction runs until completion
            
        8.  `SubInteraction.HandleInteractionCompleted()`
            
        9.  `Interaction.HandleInteractionResumed()`
            
        10.  `Interaction.DoNormalInteraction()`
            
        11.  Assuming this time Interaction runs until completion
            
        12.  `Interaction.HandleInteractionCompleted()`
            
    2.  Do your setup work in `HandleInteractionStarted()` and `HandleInteractionResumed()`
        
    3.  Ensure that there is no issue if DoNormalInteraction is called multiple times and can be cancelled.
        
3.  You don't have to worry about interrupt if you override `CanBeInterrupted` to false.

 
### Finding and showing Interactions
The `PlayerInteractionFinder` class raycasts through the scene to detect interactables. The `_interactionLayerMask` field controls which layers it checks. 
To show them:
```
var interactables = _interactionFinder.FindInteractables();
InteractionManager.Instance.ShowNewInteractables(interactables);
```

#### Starting interactions
InteractionPromptController handles hold confirmation and starting interation.. The InteractionManager will automatically start a confirmation once the InteractionPromptController confirms one.

#### Hold interactions
If the `_interactionHoldTime ` field on the interactable is > 0, then the InteractionPromptController will require holding the button for that amount of time.

You have to tell the InteractionManager to start the interaction:

#### Changing the text shown in the InteractionPrompt for an interactable
Override the following functions:
```
  /// <summary>
  /// Interaction prompt prefix(ex: "Inspect")
  /// </summary>
  /// <returns></returns>
  public abstract string GetPromptPrefix();
  /// <summary>
  /// Interaction prompt suffix that appears after the prompt
  /// </summary>
  /// <returns></returns>
  public abstract string GetPromptSuffix();
  /// <summary>
```

#### Custom Interaction Prompt UI
You can inherit from the `InteractionPromptView` class to customize the Prompt UI.

#### Knowing when Interaction starts/ends
InteractionManager fires the following events to tell you that
```
        /// <summary>
        /// Fired when we start the first interaction on the stack
        /// Not fired when subinteractions are started 
        /// </summary>
        public event Action OnInteractionChainStarted;
        /// <summary>
        /// Fired when we complete all the interactions on the stack
        /// Or when we cancel the interaction confirmation without anything in the stack
        /// Not fired when subinteractions are completed
        /// </summary>
        public event Action OnInteractionChainEnded;
        public bool IsRunningInteraction{get;}
```
`InteractionManager.IsRunningInteraction` can also be used to synchronously check if an interaction is running
