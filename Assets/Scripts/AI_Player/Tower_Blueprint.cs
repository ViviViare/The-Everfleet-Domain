using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace viviviare
{
    public abstract class Tower_Blueprint : MonoBehaviour
    {
        [Header("Tower Placement")]
        public float _placementRadius; //Required radius a tower needs to be placed / Space used by the tower
        private float _radiusDivision = 1.35f; //Circle indicator spawns 35%~ larger than the float radius, this var is to offset that
        private GameObject _placementCircle; //Gameobject to show the amount of space a tower needs to be placed
        public bool _placed; //Guard clause for inheritance script
        
        [Header("Stats UI")]
        
        [SerializeField] protected Canvas _uiStatsCanvas;
        protected Tower_Stats_UI _uiStats;
        protected bool _uiIsOpen;
        protected bool _uiPlacedDelay; //Guard clause to stop the UI opening the moment a tower is placed

        [Header("Global Stats")]
        public string _name;

        private void Awake()
        {
            _placementCircle = transform.GetChild(1).gameObject; //second position is always the circle object
            _placementCircle.transform.localScale = new Vector3((_placementRadius / _radiusDivision), (_placementRadius / _radiusDivision), 0f);
            TowerPlacement._i.ShowPlacementRadius += ShowPlacementRadius; //Subscribe to the event ShowPlacementRadius 
        }

        public void ShowPlacementRadius(object sender, EventArgs e)
        {
            _placementCircle.SetActive(true);
            
            TowerPlacement._i.ShowPlacementRadius -= ShowPlacementRadius; //Unsubscribe to the event ShowPlacementRadius so that the Placement Radius can be shown twice
            TowerPlacement._i.HidePlacementRadius += HidePlacementRadius; //Subscribe to the event HidePlacementRadius so that the Placement Radius can be hidden 
        }

        private void HidePlacementRadius(object sender, EventArgs e)
        {
            _placementCircle.SetActive(false);

            TowerPlacement._i.HidePlacementRadius -= HidePlacementRadius;
            TowerPlacement._i.ShowPlacementRadius += ShowPlacementRadius;
        }

        protected void CheckUI()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
               
                //Show the range indicator for combat towers if the cursor is hovering above them
                if (hitInfo.collider == gameObject.GetComponent<SphereCollider>())
                {
                    ShowRangeUI();
                }
                else if (!_uiIsOpen)
                {
                    HideRangeUI();
                }

                //Guard Cluase: Do not run rest of code unless RMB has been pressed
                if (!Input.GetMouseButtonDown(0)) return;

                //Run if the player has clicked on either: The tower or The tower's stat UI
                if (hitInfo.collider == gameObject.GetComponent<SphereCollider>() || hitInfo.collider == _uiStatsCanvas.GetComponent<BoxCollider>())
                {
                    //Do not run if there is already a UI open or the tower has been recently placed
                    if (_uiIsOpen || !_uiPlacedDelay) return;
                    Tower_Stats_UI_Manager._i._disableUICollider -= DisableStatsUI;
                    Tower_Stats_UI_Manager._i._enableUICollider += EnableStatsUI;

                    Tower_Stats_UI_Manager._i.DisableAllStatUI();
                    Tower_Stats_UI_Manager._i.EnableAllStatsUI();

                    Tower_Stats_UI_Manager._i._uiElementOpen = true;

                    _uiIsOpen = true;
                    _uiStatsCanvas.enabled = true;

                    _uiStats._name.text = _name;
                    UpdateStatUI();
                    ShowRangeUI();
                }
                else if (_uiIsOpen) //Clicked on something else & the UI is open
                {
                    Tower_Stats_UI_Manager._i._disableUICollider += DisableStatsUI;
                    Tower_Stats_UI_Manager._i._enableUICollider -= EnableStatsUI;
                    Tower_Stats_UI_Manager._i._enableSphereColliders += EnableSphereCollider;

                    Tower_Stats_UI_Manager._i.DisableAllStatUI();

                    if (Tower_Stats_UI_Manager._i._uiElementOpen)
                    {
                        Tower_Stats_UI_Manager._i.EnableAllSphereColliders();
                    }

                    _uiIsOpen = false;
                    _uiStatsCanvas.enabled = false;
                    HideRangeUI();
                }

            }
        }

        public IEnumerator UiStartupCooldown()
        {
            _uiPlacedDelay = false;
            yield return new WaitForSeconds(0.5f);
            _uiPlacedDelay = true;
        }

        private void DisableStatsUI(object sender, EventArgs e)
        {
            _uiStatsCanvas.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        private void EnableStatsUI(object sender, EventArgs e)
        {
            _uiStatsCanvas.GetComponent<BoxCollider>().enabled = true;
        }

        private void EnableSphereCollider(object sender, EventArgs e)
        {
            Tower_Stats_UI_Manager._i._uiElementOpen = false;
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }

        protected abstract void UpdateStatUI();
        public abstract void HideRangeUI();
        public abstract void ShowRangeUI();
    }
}
