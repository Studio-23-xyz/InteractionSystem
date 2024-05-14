using System;
using System.Collections.Generic;
using Studio23.SS2.InteractionSystem.Abstract;
using Studio23.SS2.InteractionSystem.Data;
using UnityEngine;

namespace Studio23.SS2.InteractionSystem.UI
{
    public class InteractionMarkerController:MonoBehaviour
    {
        [SerializeField] protected List<InteractableBase> _interactables;
        [SerializeField] protected InteractionMarkerBase _markerBehavior;
        protected InteractableBase _curInteractable;

        protected virtual void Awake()
        {
            GetComponents(_interactables);
            if (_markerBehavior == null)
            {
                _markerBehavior = GetComponentInChildren<InteractionMarkerBase>();
            }
        }

        protected virtual void OnEnable()
        {
            UpdateIcon();
        }

        public InteractableBase GetCurActiveInteractable()
        {
            foreach (var interactable in _interactables)
            {
                if (interactable.EvaluateInteractionConditions() == InteractionConditionResult.Show)
                {
                    return interactable;
                }
            }

            return null;
        }

        public virtual void UpdateIcon()
        {
            _curInteractable = GetCurActiveInteractable();
            
            if (_curInteractable == null)
            {
                _markerBehavior.Hide();
            }
            else
            {
                _markerBehavior.Show(_curInteractable);
            }
        }
    }
}