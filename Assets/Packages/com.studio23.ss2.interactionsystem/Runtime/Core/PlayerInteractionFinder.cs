using System.Collections.Generic;
using Bdeshi.Helpers.Utility.Extensions;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.Core
{
    /// <summary>
    /// Looks for Interactables on scene based on camera position and orientation
    /// </summary>
    public class PlayerInteractionFinder: MonoBehaviour
    {
        public Vector3 InteractionFindDirection => _cam.transform.forward;
        public Vector3 InteractionRayOrigin => _cam.transform.position;
        [SerializeField] private Camera _cam;
        [SerializeField] float _interactionFindDistance = 20;
        [SerializeField] private LayerMask _interactionLayerMask;
        [SerializeField] private LayerMask _obstacleLayerMask;
        [SerializeField] private float _interactionSphereCastRadius = .25f;
        // used for nonalloc collider finding
        // Collider[] _colliderCache = new Collider[69];
        private void Start()
        {
            if (_cam == null)
            {
                SetCam(Camera.main);
            }
        }

        /// <summary>
        /// Get list of interactables.
        /// #NOTE GC alloc every call
        /// Optimize if needed
        /// </summary>
        /// <returns> a list of interactables </returns>
        public List<InteractableBase> FindInteractables()
        {
            var mask = _interactionLayerMask | _obstacleLayerMask;
            List<InteractableBase> results = new List<InteractableBase>();
            if (Physics.SphereCast(InteractionRayOrigin,  _interactionSphereCastRadius,InteractionFindDirection,
                    out var hit, _interactionFindDistance, mask, QueryTriggerInteraction.Collide))
            {
                
                // for subinteractables, interactable layer itself could be obstacle mask
                // this saves us a getcomponent call anyways
                if (_interactionLayerMask.Contains(hit))
                {
                    hit.collider.GetComponents<InteractableBase>(results);
                    for (int i = results.Count -1; i >= 0; i--)
                    {
                        var result = results[i];
                    
                        //remove all interactables that we need to hide
                        //allow interactables that return disabled and show status
                        if (result.EvaluateInteractionConditions() == InteractionConditionResult.Hide)
                        {
                            results.RemoveAt(i);
                        }
                    }
                    Debug.DrawRay(InteractionRayOrigin, InteractionFindDirection.normalized * hit.distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(InteractionRayOrigin, InteractionFindDirection.normalized * _interactionFindDistance, Color.red);

                }
            }

            return results;
        }

        public void SetCam(Camera _cam)
        {
            this._cam = _cam;
        }

        private void OnDrawGizmos()
        {
            if(_cam == null)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(InteractionRayOrigin, InteractionFindDirection * _interactionFindDistance );
        }
    }
}