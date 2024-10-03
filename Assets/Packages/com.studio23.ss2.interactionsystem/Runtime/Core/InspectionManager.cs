using System.Threading;
using Bdeshi.Helpers.Utility;
using BDeshi.Logging;
using Studio23.SS2.InteractionSystem.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Studio23.SS2.InteractionSystem.Core
{
    public class InspectionManager:MonoBehaviourSingletonPersistent<InspectionManager>, ISubLoggerMixin<InteractionLogCategory>
    {
        [SerializeField] private bool _isDebug = false;

        public Transform InspectionObjectParent;

        [SerializeField] private InteractionFinderBase _subInteractionFinder;
        
        private Transform _examinationObject;
        private Transform _ogParent;
        private Vector3 _ogOffset;
        private Quaternion _ogOrientation;
        
        private bool _isDragging = false;
        private bool _wantsToCancel = false;
        private bool _isInspecting = false;
        [Header("Examination Controls")]
        [SerializeField] InspectableBase _curInspectable;
        [SerializeField] private float _examinationSensitivity = 69;
        [SerializeField] private float _moveSpeed = 2;
        [SerializeField] private float _zoomSpeed = 8/120f;
        public bool ExaminationXAxisInverted = false;
        public bool ExaminationYAxisInverted = false;

        [SerializeField] Camera _mainCamera;
        [SerializeField] Camera  _inspectionCamera;
        [SerializeField] private Canvas _inspectionBackgroundCanvas;

        public UnityEvent OnInspectionStarted;
        public UnityEvent OnInspectionEnded;

        protected override void Initialize()
        {
            
        }

        private void Start()
        {
            FindMainCamera();

            InteractionManager.Instance.InputHandler.InspectionDragButton.AddPerformedCallback(gameObject, HandleInspectDragPerformed);
            InteractionManager.Instance.InputHandler.InspectionDragButton.AddCancelledCallback(gameObject, HandleInspectDragCancelled);
            InteractionManager.Instance.InputHandler.InteractCancelButton.AddPerformedCallback(gameObject, HandleInspectionCancelled);
            InteractionManager.Instance.InputHandler.InspectResetButton.AddPerformedCallback(gameObject, HandleInspectResetPerformed);
        }
        
        public void HandleInspectResetPerformed()
        {
            if (_isInspecting && _examinationObject != null)
            {
                ForceResetInspectablePosition();
            }
        }

        public void ForceResetInspectablePosition()
        {
            if (_examinationObject != null)
            {
                _examinationObject.localPosition = _curInspectable.InspectionPosOffset;
                _examinationObject.localRotation = Quaternion.Euler(_curInspectable.InspectionRotOffset);
            }
        }

        public void HandleInspectionCancelled()
        {
            _wantsToCancel = true;
            _isInspecting = false;
        }

        public void HandleInspectDragCancelled()
        {
            _isDragging = false;
        }

        public void HandleInspectDragPerformed()
        {
            _isDragging = true;
        }
        
        /// <summary>
        /// set The camera that the RT camera copies position of
        /// camera.main in most cases.
        /// </summary>
        /// <param name="cam"></param>
        public void SetGameCamera(Camera cam)
        {
            _mainCamera = cam;
        }

        
        private async UniTask ExaminationTask(InspectableBase inspectable, CancellationToken token)
        {
            Logger.Log(InteractionLogCategory.DoInteraction, $"examine {inspectable} TASK start");
            OnInspectionStarted.Invoke();
            while (true)
            {
                if((_wantsToCancel && inspectable.CanExitInspection) || inspectable.ForceExitInspection)
                {
                    break;
                }
                UpdateDrag(inspectable, _examinationObject);
                UpdateInspectableMove(inspectable);
                FindSubInteraction();
                await UniTask.Yield(token);
                await UniTask.NextFrame(token);
            }
            Logger.Log(InteractionLogCategory.DoInteraction,$"examine {inspectable} TASK end");
            OnInspectionEnded.Invoke();
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
            var inputHandler = InteractionManager.Instance.InputHandler;
            if (inputHandler.InspectionMoveInput != Vector2.zero || 
                inputHandler.InspectionZoomInput != 0)
            {
                Vector3 moveAmount = inputHandler.InspectionMoveInput;
                var zoomAmount = inputHandler.InspectionZoomInput;
                moveAmount = _inspectionCamera.transform.right * moveAmount.x 
                             + _inspectionCamera.transform.up * moveAmount.y
                             ;
                moveAmount += _inspectionCamera.transform.forward * zoomAmount * _zoomSpeed;
                
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
            Debug.Log($"unmove {_ogParent} {inspectable}", _ogParent);

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
            _examinationObject.localPosition = inspectable.InspectionPosOffset;
            _examinationObject.localRotation = Quaternion.Euler(inspectable.InspectionRotOffset);
            _examinationObject.localScale = inspectable.InspectionScale;

            //_examinationObject.parent = InspectionObjectParent;
            //_examinationObject.position = InspectionObjectParent.position;
            //_examinationObject.rotation = Quaternion.Euler(InspectionObjectParent.rotation.eulerAngles +
            //    _examinationObject.eulerAngles)
            //    ;
            _examinationObject.localScale = inspectable.InspectionScale;
        }

        public void HandleInteractionInitialize(InspectableBase inspectable)
        {
            FindMainCamera();

            _inspectionBackgroundCanvas.worldCamera = _mainCamera;
            _inspectionBackgroundCanvas.gameObject.SetActive(true);
            //urp camera stacking
            ActivateInspectionCamStacking();

            _wantsToCancel = false;
            _isDragging = false;
            _isInspecting = true;

            _curInspectable = inspectable;
            _examinationObject = inspectable.GetInspectionTarget();
            MoveInspectableForInspection(inspectable);
        }

        public void ActivateInspectionCamStacking()
        {
            var cameraData = _mainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(_inspectionCamera);
        }


        private void FindMainCamera()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;    
            }
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
            // InspectionObjectParent.transform.parent = this.transform;
            UnMoveInspectableForInspection(inspectable);
            
            _inspectionBackgroundCanvas.gameObject.SetActive(false);
            DeactivateInspectionCamStacking();

            _curInspectable = null;
        }

        public void DeactivateInspectionCamStacking()
        {
            //urp camera stacking undo
            var cameraData = _mainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Remove(_inspectionCamera);
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
            var interactions = _subInteractionFinder.FindInteractables();
            InteractionManager.Instance.ShowNewInteractables(interactions);
        }

        private void UpdateDrag(InspectableBase inspectableBase, Transform spawnedExaminationObject)
        {
            Vector2 dragDelta = InteractionManager.Instance.InputHandler.GetInspectDragDeltaThisFrame();
            float dragSensitivity = _examinationSensitivity * inspectableBase.InspectionRotationSensitivity;
            if (_isDragging && dragDelta != Vector2.zero)
            {
                // Scale the drag delta by the rotation speed
                float rotationAmountX = (ExaminationXAxisInverted ? 1:-1) * dragDelta.x * dragSensitivity * Time.deltaTime;
                float rotationAmountY = (ExaminationYAxisInverted ? 1:-1) * dragDelta.y * dragSensitivity * Time.deltaTime;

                spawnedExaminationObject.RotateAround(spawnedExaminationObject.position, _inspectionCamera.transform.up, rotationAmountX);
                spawnedExaminationObject.RotateAround(spawnedExaminationObject.position, _inspectionCamera.transform.right, rotationAmountY);
            }

            _curInspectable.HandleRotation(_isDragging);
        }

        public async UniTask ShowInspectable(InspectableBase inspectable, CancellationToken token)
        {
            await ExaminationTask(inspectable, token);
        }

        public ICategoryLogger<InteractionLogCategory> Logger => InteractionManager.Instance.Logger;
    }
}