namespace com.studio23.ss2.InteractionSystem23.Data
{
    public enum InteractionConditionResult
    {
        Show,//should show the interactionPrompt normally
        Disable,//cannot interact with it, but show a disabled prompt
        Hide//won't even show a prompt
    }
}