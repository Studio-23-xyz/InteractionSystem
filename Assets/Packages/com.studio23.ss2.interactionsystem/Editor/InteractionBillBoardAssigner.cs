using Studio23.SS2.InteractionSystem.Abstract;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Editor
{
    [CustomEditor(typeof(InteractableBase), true)]
    public class InteractionBillBoardAssigner : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InteractableBase interactable = (InteractableBase)target;

            if (GUILayout.Button("Instantiate InteractionBillBoard"))
            {
                GameObject childPrefab = Resources.Load<GameObject>("InteractionSystem/InteractionBillBoard");
                GameObject childObject = Instantiate(childPrefab);
                childObject.transform.parent = interactable.transform;
                childObject.transform.localPosition = Vector3.zero;
                childObject.transform.localRotation = Quaternion.identity;
            }
        }
    }
}