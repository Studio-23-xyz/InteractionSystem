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

#### SubInteractions
If you start an interaction while one is running, the old interaction is paused and the new interaction is started as a subinteraciton. When the subinteraction ends, the old interaction is resumed.

#### Custom Interactions
You can define your own Interactable by inheriting from `InteractableBase`.

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

#### Known Issues
When an inspectable is inspected , it can spawn inside colliders. This will be fixed when we figure out a way to overlay camera renders on HDRP.
