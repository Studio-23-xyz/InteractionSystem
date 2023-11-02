using System.Collections.Generic;
using com.bdeshi.helpers.Utility.Extensions;
using Studio23.SS2.InteractionSystem23.Abstract;
using Studio23.SS2.InteractionSystem23.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.InteractionSystem23.Core
{
    /// <summary>
    /// Looks for Interactables on scene based on camera position and orientation
    /// </summary>
    public class PlayerInteractionFinder: MonoBehaviour
    {
        public Vector3 InteractionFindDirection => _cam.transform.forward;
        public Vector3 InteractionRayOrigin => _cam.transform.position;
        [SerializeField] private Camera _cam;
        [FormerlySerializedAs("interactionFindDistance")] [SerializeField] float _interactionFindDistance = 20;
        [FormerlySerializedAs("interactionLayerMask")] [SerializeField] private LayerMask _interactionLayerMask;
        [FormerlySerializedAs("obstacleLayerMask")] [SerializeField] private LayerMask _obstacleLayerMask;

        [FormerlySerializedAs("interactionSphereCastRadius")] [SerializeField] private float _interactionSphereCastRadius = .25f;
        // used for nonalloc collider finding
        // Collider[] _colliderCache = new Collider[69];
        private void Start()
        {
            SetCam(Camera.main);
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
                        if (result.EvaluateInteractionConditions(this) == InteractionConditionResult.Hide)
                        {
                            results.RemoveAt(i);
                        }
                    }
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