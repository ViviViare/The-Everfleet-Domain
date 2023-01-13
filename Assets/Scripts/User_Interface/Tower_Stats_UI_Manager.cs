using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace viviviare
{
    public class Tower_Stats_UI_Manager : MonoBehaviour
    {
        public static Tower_Stats_UI_Manager _i;

        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public bool _uiElementOpen;
        
        //Events for disabling/enableing different Tower Stat UI related colliders
        public event EventHandler _disableUICollider, _enableUICollider, _enableSphereColliders;

        public void DisableAllStatUI()
        {
            _disableUICollider?.Invoke(this, EventArgs.Empty);
        }

        public void EnableAllStatsUI()
        {
            _enableUICollider?.Invoke(this, EventArgs.Empty);
        }

        public void EnableAllSphereColliders()
        {
            _enableSphereColliders?.Invoke(this, EventArgs.Empty);
        }
        


    }
}
