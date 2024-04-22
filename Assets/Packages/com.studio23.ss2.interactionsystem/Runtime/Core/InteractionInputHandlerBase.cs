using Bdeshi.Helpers.Input;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Core
{
    public class InteractionInputHandlerBase : MonoBehaviour
    {
        [SerializeField] protected Vector2 _inspectDragDelta;
        [SerializeField] protected Vector2 _inspectionMoveInput;
        [SerializeField] protected float _inspectionZoomInput;
        public Vector2 InspectDragDelta => _inspectDragDelta;
        public Vector2 InspectionMoveInput => _inspectionMoveInput;
        public float InspectionZoomInput => _inspectionZoomInput;
        public InputButtonSlot InspectButton { get;  protected set;}  = new InputButtonSlot("InspectButton");
        public InputButtonSlot InspectionDragButton { get; protected set; }  = new InputButtonSlot("InspectionDragButton");
        public InputButtonSlot ToggleButton { get; protected set; }  = new InputButtonSlot("ToggleButton");
        public InputButtonSlot PickupButton { get; protected set; }  = new InputButtonSlot("InteractButton");
        public InputButtonSlot InteractCancelButton { get; protected set; }  = new InputButtonSlot("InteractCancelButton");
        public InputButtonSlot InspectResetButton { get; protected set; }  = new InputButtonSlot("InspectResetButton");
    }
}