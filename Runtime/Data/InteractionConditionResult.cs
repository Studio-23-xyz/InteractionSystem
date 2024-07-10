namespace Studio23.SS2.InteractionSystem.Data
{
    public enum InteractionConditionResult
    {
        Show,//should show the interactionPrompt normally
        Disable,//cannot interact with it, but show a disabled prompt
        Hide,//won't even show a prompt
        Passthrough,//move to next condition if one exists. Otherwise, hide
    }
}