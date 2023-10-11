using System.Collections.Generic;
using com.bdeshi.helpers.Input;
using com.bdeshi.helpers.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.studio23.ss2.InteractionSystem23.Core
{
    public class InteractionInputManager : MonoBehaviourSingletonPersistent<InteractionInputManager>
    {
        [SerializeField] private InputActionAsset _inputActionMap;
        // TODO: FOR gamepad this needs to be multiplied by time.deltatime
        [SerializeField]Vector2 _inspectDragDelta; 
        public Vector2 InspectDragDelta => _inspectDragDelta;
        [SerializeField]private Vector2 _inspectionMoveInput;
        public Vector2 InspectionMoveInput => _inspectionMoveInput;
        [SerializeField]private float _inspectionZoomInput;
        public float InspectionZoomInput => _inspectionZoomInput;
        
        [SerializeField] private InputActionReference _inspectDragDeltaAction;
        [SerializeField] private InputActionReference _inspectAction;
        [SerializeField] private InputActionReference _inspectDragAction;
        [SerializeField] private InputActionReference _interactCancelAction;
        [SerializeField] private InputActionReference _toggleAction;
        [SerializeField] private InputActionReference _pickupAction;
        [SerializeField] private InputActionReference _inspectMoveAction;
        [SerializeField] private InputActionReference _inspectZoomAction;
        [SerializeField] private InputActionReference _inspectResetAction;
        [SerializeField] InputActionReference  _debug1Action;
        
        public List<InputButtonSlot> Buttons;
        public InputButtonSlot InspectButton { get;  private set;}
        public InputButtonSlot InspectionDragButton { get; private set; }
        public InputButtonSlot ToggleButton { get; private set; }
        public InputButtonSlot PickupButton { get; private set; }
        public InputButtonSlot InteractCancelButton { get; private set; }
        public InputButtonSlot InspectResetButton { get; private set; }
        public InputButtonSlot Debug1Button { get; private set; }

        protected override void initialize()
        {
            Buttons = new List<InputButtonSlot>();
            InspectButton = new InputButtonSlot("InspectButton");
            InspectionDragButton = new InputButtonSlot("InspectionDragButton");
            ToggleButton = new InputButtonSlot(  "ToggleButton");
            PickupButton = new InputButtonSlot("InteractButton");
            InteractCancelButton = new InputButtonSlot("InteractCancelButton");
            InspectResetButton = new InputButtonSlot("InspectResetButton");
            Debug1Button = new InputButtonSlot("Debug1Button");
        }

        public static Vector3 ConvertVecCamRelative(Camera cam, Vector3 dir)
        {
            return Quaternion.AngleAxis(cam.transform.rotation.eulerAngles.y, Vector3.up) * dir;
        }

        void OnEnable()
        {
            BindAll();
        }

        private void BindAll()
        {
            if (_inputActionMap == null)
                return;

            _inputActionMap.Enable();

            Buttons.Clear();
            Bind(InspectButton, _inspectAction);
            Bind(ToggleButton, _toggleAction);
            Bind(PickupButton, _pickupAction);
            Bind(InspectionDragButton, _inspectDragAction);
            Bind(InteractCancelButton, _interactCancelAction);
            Bind(InspectResetButton, _inspectResetAction);

#if UNITY_EDITOR
            Bind(Debug1Button, _debug1Action);
#endif

            _inspectMoveAction.action.performed += OnMovePerformed;
            _inspectMoveAction.action.canceled += OnMoveCancelled;
            _inspectZoomAction.action.performed += OnZoomPerformed;
            _inspectZoomAction.action.canceled += OnZoomCancelled;
            _inspectDragDeltaAction.action.performed += OnLookPerformed;
            _inspectDragDeltaAction.action.canceled += OnLookCancelled;
        }

        private void OnZoomCancelled(InputAction.CallbackContext obj)
        {
            _inspectionZoomInput = 0;
        }

        private void OnZoomPerformed(InputAction.CallbackContext obj)
        {
            _inspectionZoomInput = obj.ReadValue<float>();
        }

        private void OnMoveCancelled(InputAction.CallbackContext obj)
        {
            _inspectionMoveInput = Vector2.zero;
        }

        private void OnMovePerformed(InputAction.CallbackContext obj)
        {
            _inspectionMoveInput = obj.ReadValue<Vector2>();
        }

        void Bind(InputButtonSlot slot, InputActionReference action)
        {
            if(action == null)
            {
                Debug.LogWarning("action null");
                return;
            }
            slot.bind(action);
            Buttons.Add(slot);
        }
        
        void OnDisable()
        {
            UnBindAll();
        }

        private void UnBindAll()
        {
            if (_inputActionMap == null)
                return;

            foreach (var inputButtonSlot in Buttons)
            {
                inputButtonSlot.unBind();
            }

            _inspectMoveAction.action.performed -= OnMovePerformed;
            _inspectMoveAction.action.canceled -= OnMoveCancelled;
            _inspectDragDeltaAction.action.performed -= OnLookPerformed;
            _inspectDragDeltaAction.action.canceled -= OnLookCancelled;
            _inputActionMap.Disable();
        }

        private void OnLookCancelled(InputAction.CallbackContext obj)
        {
            _inspectDragDelta = Vector2.zero;
        }

        private void OnLookPerformed(InputAction.CallbackContext obj)
        {
            _inspectDragDelta = obj.ReadValue<Vector2>();
        }
        
        public static void PlayModeExitCleanUp()
        {
            if(Instance == null)
                return;
            foreach (var inputButtonSlot in Instance.Buttons)
            {
                inputButtonSlot.cleanup();
            }
        }
    }
}
