using Studio23.SS2.InteractionSystem.Core;
using UnityEditor;

namespace Studio23.SS2.InteractionSystem.Utilities
{
    #if UNITY_EDITOR
    /// <summary>
    /// Unity playmode with no domain reload/scene reload can cause problems with static stuff
    /// But it's too nice to give up.
    /// Call cleanup funcs here for now.
    /// TODO: Make an attribute to fetch and call automatically
    /// </summary>
    [InitializeOnLoad]
    public static class PlayStateCleaner
    {
        static PlayStateCleaner()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        static void ModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingPlayMode)
            {
                InteractionInputManager.PlayModeExitCleanUp();
            }
        }
    }
    #endif
}