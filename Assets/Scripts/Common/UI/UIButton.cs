using UnityEngine;
using UnityEngine.UI;
using System;

namespace Common.UI
{
    public class UIButton : Button
    {
        [SerializeField] private bool _interactableOnAwake = true;
        
        protected override void Awake()
        {
            base.Awake();
            interactable = _interactableOnAwake;
        }

        public void SetInteractable(bool interactable)
        {
            this.interactable = interactable;
        }

        public bool IsInteractable()
        {
            return interactable;
        }

        public void SetClickCallback(Action callback)
        {
            onClick.RemoveAllListeners();
            onClick.AddListener(() => callback?.Invoke());
        }
    }
}