using System.Threading;
using com.bdeshi.helpers.Utility;
using Studio23.SS2.InteractionSystem23.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.InteractionSystem23.Core
{
    public class InspectionManager:MonoBehaviourSingletonPersistent<InspectionManager>
    {
        [SerializeField] private bool _isDebug = false;
        public Transform InspectionObjectParent;
        public Transform Player;

        [SerializeField] private PlayerInteractionFinder _subInteractionFinder;
        
        private Transform _examinationObject;
        private Transform _ogParent;
        private Vector3 _ogOffset;
        private Quaternion _ogOrientation;
        
        private bool _isDragging = false;
        private bool _wantsToCancel = false;
        private bool _isInspecting = false;
        [Header("Examination Controls")]
        [SerializeField] private float _examinationSensitivity = 69;
        [SerializeField] private float _moveSpeed = 2;
        [SerializeField] private float _zoomSpeed = 8/120f;
        private Camera _cam;

        protected override void initialize()
        {
            _subInteractionFinder = GetComponentInChildren<PlayerInteractionFinder>();
        }

        private void Start()
        {
            InteractionInputManager.Instance.InspectionDragButton.addPerformedCallback(gameObject, HandleInspectDragPerformed);
            InteractionInputManager.Instance.InspectionDragButton.addCancelledCallback(gameObject, HandleInspectDragCancelled);
            InteractionInputManager.Instance.InteractCancelButton.addPerformedCallback(gameObject, HandleInspectionCancelled);
            InteractionInputManager.Instance.InspectResetButton.addPerformedCallback(gameObject, HandleInspectResetPerformed);
        }

        private void HandleInspectResetPerformed()
        {
            if (_isInspecting && _examinationObject != null)
            {
                _examinationObject.localPosition = Vector3.zero;
            }
        }

        private void HandleInspectionCancelled()
        {
            _wantsToCancel = true;
            _isInspecting = false;
        }

        private void HandleInspectDragCancelled()
        {
            _isDragging = false;
        }

        private void HandleInspectDragPerformed()
        {
            _isDragging = true;
        }

        void FindPlayer()
        {
            if (Player == null)
            {
                Player = GameObject.FindWithTag("Player").transform;
            }
        }
        
        private async UniTask ExaminationTask(InspectableBase inspectable, CancellationToken token)
        {
            InteractionManager.Instance.Dlog("examine TASK start");
            while (!_wantsToCancel)
            {
                UpdateDrag(inspectable, _examinationObject);
                UpdateInspectableMove(inspectable);
                FindSubInteraction();
                await UniTask.Yield(token);
            }
            Debug.Log("examine TASK end");
        }
        
        public void Dlog(string message, UnityEngine.Object context = null)
        {
            #if UNITY_EDITOR
                if (_isDebug)
                {
                    Debug.Log(message, context);
                }
            #endif
            
        }

        private void UpdateInspectableMove(InspectableBase inspectableBase)
        {
            
            if (InteractionInputManager.Instance.InspectionMoveInput != Vector2.zero || 
                InteractionInputManager.Instance.InspectionZoomInput != 0)
            {
                Vector3 moveAmount = InteractionInputManager.Instance.InspectionMoveInput;
                var zoomAmount = InteractionInputManager.Instance.InspectionZoomInput;
                moveAmount = _cam.transform.right * moveAmount.x 
                             + _cam.transform.up * moveAmount.y
                             ;
                moveAmount += _cam.transform.forward * zoomAmount * _zoomSpeed;
                
                moveAmount *= _moveSpeed * Time.deltaTime;
                

                _examinationObject.position += moveAmount;
                //clamp the maximum displacement aka localPosition
                var examinationObjectLocalPos = _examinationObject.transform.localPosition;
                _examinationObject.localPosition = new Vector3(
                    Mathf.Clamp(examinationObjectLocalPos.x,
                        -inspectableBase.InspectionTargetMaxOffset,
                        inspectableBase.InspectionTargetMaxOffset),
                    Mathf.Clamp(examinationObjectLocalPos.y, 
                        -inspectableBase.InspectionTargetMaxOffset, 
                        inspectableBase.InspectionTargetMaxOffset),
                    Mathf.Clamp(examinationObjectLocalPos.z, 
                        -inspectableBase.InspectionTargetMaxZoom,
                        inspectableBase.InspectionTargetMaxZoom)
                ); 
            }
        }

        private void UnMoveInspectableForInspection(InspectableBase inspectable)
        {
            _examinationObject.parent = _ogParent;
            _examinationObject.localPosition = _ogOffset;
            _examinationObject.localRotation = _ogOrientation;
            _examinationObject.localScale = inspectable.OriginalInspectionTargetScale;
        }

        private void MoveInspectableForInspection(InspectableBase inspectable)
        {
            _ogParent = _examinationObject.transform.parent;
            _ogOffset = _examinationObject.localPosition;
            _ogOrientation = _examinationObject.localRotation;
            
            
            _examinationObject.gameObject.SetActive(true);
            _examinationObject.parent = InspectionObjectParent;
            _examinationObject.localPosition = Vector3.zero;
            _examinationObject.localRotation = Quaternion.identity;
            _examinationObject.localScale = inspectable.InspectionScale;
        }

        public void HandleInteractionInitialize(InspectableBase inspectable)
        {
            FindPlayer();
            FindCamera();
            _wantsToCancel = false;
            _isDragging = false;
            _isInspecting = true;
            
            InspectionObjectParent.transform.parent = Player;
            InspectionObjectParent.localPosition = inspectable.InspectionOffset;

            
            //#TODO fix the render texture issue and update this
            Dlog("start inspection " + inspectable);
            _examinationObject = inspectable.GetInspectionTarget();
            MoveInspectableForInspection(inspectable);
        }

        private void FindCamera()
        {
            _cam = Camera.main;
        }

        public void HandleInspectablePaused(InspectableBase inspectable)
        {
            _isInspecting = false;
            _wantsToCancel = false;
            _isDragging = false;
        }
        
        public void HandleInspectableCompleted(InspectableBase inspectable)
        {
            _isInspecting = false;
            Dlog("end inspection " + inspectable);
            InspectionObjectParent.transform.parent = this.transform;
            UnMoveInspectableForInspection(inspectable);
        }

        public void HandleInspectableResumed(InspectableBase inspectable)
        {
            _isInspecting = true;
            _wantsToCancel = false;
            _isDragging = false;
        }
        private void FindSubInteraction()
        {
            if(_subInteractionFinder == null)
                return;
            _subInteractionFinder.transform.position = Player.position;
            var interactions = _subInteractionFinder.FindInteractables();
            InteractionManager.Instance.ShowNewInteractables(interactions);
        }

        private void UpdateDrag(InspectableBase inspectableBase, Transform spawnedExaminationObject)
        {
            Vector2 dragDelta = InteractionInputManager.Instance.InspectDragDelta;
            float dragSensitivity = _examinationSensitivity * inspectableBase.InspectionRotationSensitivity;
            if (_isDragging && dragDelta != Vector2.zero)
            {
                // Scale the drag delta by the rotation speed
                float rotationAmountX = -dragDelta.x * dragSensitivity * Time.deltaTime;
                float rotationAmountY = dragDelta.y * dragSensitivity * Time.deltaTime;

                // Rotate the object relative to its current rotation
                //#TODO if we do camera stacking, then space.world would not be an issue.
                // but we can do that only if we fix the rendertexture issue
                spawnedExaminationObject.RotateAround(spawnedExaminationObject.position, _cam.transform.up, rotationAmountX);
                spawnedExaminationObject.RotateAround(spawnedExaminationObject.position, _cam.transform.right, rotationAmountY);
            }
        }

        public async UniTask ShowInspectable(InspectableBase inspectable, CancellationToken token)
        {
            await ExaminationTask(inspectable, token);
        }
    }
}