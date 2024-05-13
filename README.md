<h1 align="center">Interaction System</h1>
<p align="center">
<a href="https://openupm.com/packages/com.studio23.ss2.interactionsystem/"><img src="https://img.shields.io/npm/v/com.studio23.ss2.interactionsystem?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
</p>

Introducing an Interaction System package for Unity.

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
   - [Getting Started](#Getting-Started)
3. [Extensions](#Extensions)

## Installation

### Install via Git URL

You can also use the "Install from Git URL" option from Unity Package Manager to install the package.
```
https://github.com/Studio-23-xyz/InteractionSystem.git#upm
```

## Usage


### PlayerInteractionFinder
The PlayerInteractionFinder class is responsible for locating interactable objects based on the camera's position and orientation.

Example Usage:

```C#
// Attach the PlayerInteractionFinder component to a GameObject in your scene.
// The camera reference should be assigned in the inspector.
// The Interaction Layer Mask and Obstacle Layer Mask can be adjusted as needed.
// Set other parameters such as Interaction Find Distance and Interaction Sphere Cast Radius.

public class YourClass : MonoBehaviour
{
    [SerializeField] private PlayerInteractionFinder interactionFinder;

    private void Start()
    {
        // Optionally, set the main camera explicitly (if not set in the inspector).
        interactionFinder.SetCam(Camera.main);
    }

    private void Update()
    {
        List<InteractableBase> interactables = interactionFinder.FindInteractables();
        // Use the list of interactables as needed.
    }
}
```

### InteractionManager
The InteractionManager class manages the interaction stack and handles interaction events.

Example Usage:

```C#
// Attach the InteractionManager component to a GameObject in your scene.
// Set up the required references and events.

public class YourClass : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;

    private void Start()
    {
        // Subscribe to interaction events if needed.
        interactionManager.OnInteractionChainStarted += YourMethod;
        interactionManager.OnInteractionChainEnded += YourMethod;
    }

    // Other relevant methods and event handlers...
}
```

### InteractionInputManager
The InteractionInputManager class manages input actions and buttons related to interactions.

Example Usage:

```C#
// Attach the InteractionInputManager component to a GameObject in your scene.
// Set up input actions and buttons as needed.

public class YourClass : MonoBehaviour
{
    [SerializeField] private InteractionInputManager inputManager;

    private void Start()
    {
        // Subscribe to input events if needed.
        inputManager.OnInteractableConfirmed += YourMethod;
        inputManager.OnInteractableConfirmationStarted += YourMethod;
        inputManager.OnInteractableConfirmationCancelled += YourMethod;
    }

    // Other relevant methods and event handlers...
}
```

### InspectionManager
The InspectionManager class handles the inspection of interactable objects.



### InputPromptsController
Manage input prompts for interactions.

