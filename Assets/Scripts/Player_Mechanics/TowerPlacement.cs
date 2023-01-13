using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace viviviare
{
    public partial class TowerPlacement : MonoBehaviour
    {
        #region Variables
        public static TowerPlacement _i;
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
        
        [Header("Current Tower")]
        public GameObject _currentTower;
        private SpriteRenderer _currentSprite;

        [Header("Config")]
        [SerializeField] private LayerMask _colliderMask, _placementMask;
        private Camera _mainCam;
        public event EventHandler ShowPlacementRadius, HidePlacementRadius;
        private bool _canChangeColour = true;

        [Header("Tower Setup")]
        [SerializeField] [NonReorderable] private List<Tower> _towers = new List<Tower>();
        [SerializeField] private Button[] _towerButtons;
        [SerializeField] private TextMeshProUGUI[] _towerNameTexts;
        [SerializeField] private TextMeshProUGUI[] _towerAmountTexts;
        [SerializeField] private TextMeshProUGUI[] _towerCostTexts;

        private int _lastPressedTower;

        #endregion

        private void Start()
        {
            for (int i = 0; i < _towers.Count; i++)
            {
                SetupTowerSlots(i);
                UpdateTowerUI(i);
            }
            _mainCam = Camera.main;
            CheckIfCanAfford();
        }

        private void SetupTowerSlots(int towerIndex)
        {
            //Setup the tower's references
            _towers[towerIndex]._usableButton = _towerButtons[towerIndex]; 
            _towers[towerIndex]._amountTMPro = _towerAmountTexts[towerIndex];
            _towers[towerIndex]._costTMPro = _towerCostTexts[towerIndex];

            //Setup the UI
            _towerAmountTexts[towerIndex].text =  "" + _towers[towerIndex]._amount;
            _towerCostTexts[towerIndex].text =  "" + _towers[towerIndex]._cost;
            _towerNameTexts[towerIndex].text = "" + _towers[towerIndex]._name;

            //Setup the button to call the CheckIfCanPlace function for the correct tower
            _towerButtons[towerIndex].onClick.AddListener(() => CheckIfCanPlace(towerIndex));
        }
        
        private void Update()
        {
            CheckIfCanAfford();
            if (_currentTower != null) 
            {
                PlaceSelected();

                //Stop placing tower if pressing RMB
                if (Input.GetMouseButtonDown(1))
                {
                    UndoSelected(_lastPressedTower);
                }
            }
        }
        
        #region Tower Placement
        private void UndoSelected(int towerIndex)
        {
            PoolManager.Despawn(_currentTower);
            _currentTower = null;

            _towers[towerIndex]._amount++;
            UpdateTowerUI(towerIndex);
        }
        
        private void PlaceSelected()
        {
            Ray cameraRay = _mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Move tower to be wherever the cursor is.
            if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity, _colliderMask)) 
            {
                //If cursor is over an Uninteractable object then do not place the tower ontop of it
                if (hit.collider.gameObject.CompareTag("Uninteractable")) return;
                
                _currentTower.transform.position = hit.point;
                
                if (_canChangeColour) _currentSprite.color = Color.green;
            }
            
            //Ensure that there is something to hit
            if (hit.collider == null) return;

            //If mouse position is over an object that cannot be placed on, change sprite colour to red and do not do the following code.
            if (hit.collider.gameObject.CompareTag("CantPlace")) 
            {
                if (_canChangeColour) _currentSprite.color = Color.red;
                return;
            }
            
            Transform towerCircleRadius = _currentTower.transform.GetChild(1);
            SphereCollider towerCollider = towerCircleRadius.gameObject.GetComponent<SphereCollider>();
            //Show placement radius on current tower
            towerCircleRadius.gameObject.SetActive(true);

            //Because the scale of the circle is offset by 35%, to match the actual radius size visually,
            //it has to be increased to match its actual radius size.
            //Visually however, a smaller increase of 30% has a better feel to it rather than the exact size.
            float towerRadiusTrue = towerCircleRadius.localScale.x * 1.3f; 

            Vector3 towerCenter = _currentTower.gameObject.transform.position + towerCollider.center;
            towerCollider.isTrigger = true;

            if (!Physics.CheckSphere(towerCenter, towerRadiusTrue, _placementMask, QueryTriggerInteraction.Ignore) && !IsMouseOverUI())
            {
                //Has placed down a tower on a valid spot
                if (Input.GetMouseButtonDown(0) && hit.collider.gameObject != null)
                {
                    //Disable placement radius on all pre-existing towers & environment
                    HidePlacementRadius?.Invoke(this, EventArgs.Empty);

                    //Disable placement radius on newly placed tower.
                    towerCircleRadius.gameObject.SetActive(false);

                    //Hide attack range on newly placed tower.
                    _currentTower.GetComponent<Tower_Blueprint>().HideRangeUI();

                    //Delay the towers Stat UI from showing on place
                    StartCoroutine(_currentTower.GetComponent<Tower_Blueprint>().UiStartupCooldown());

                    //Update player currency
                    Player_Stats._i._currency -= _towers[_lastPressedTower]._cost;
                    Player_Stats._i.UpdateUI();

                    //Reset all variables
                    towerCollider.isTrigger = false;
                    _currentTower.GetComponent<Tower_Blueprint>()._placed = true;
                    _currentTower = null;
                    _currentSprite.color = Color.white;
                    _currentSprite = null;
                }
            }
            else
            {
                if (_canChangeColour) _currentSprite.color = Color.red;
            }
        }
        
        private void CheckIfCanPlace(int towerIndex)
        {
            //Guard Clause: Do not get a new tower if the player has a tower already selected
            if (_currentTower != null) return;

            _towers[towerIndex]._amount--;
            _lastPressedTower = towerIndex;

            UpdateTowerUI(towerIndex);
            CreateNewTower(_towers[towerIndex]._towerPrefab, towerIndex);
        }

        private void CreateNewTower(GameObject tower, int towerIndex)
        {
            //Using pooling over instantiation as towers can be removed at runtime on multiple occasions
            _currentTower = PoolManager.Spawn(tower, Vector3.zero, Quaternion.identity);
            _currentTower.GetComponent<Tower_Blueprint>()._name = _towers[towerIndex]._name;


            _currentSprite = _currentTower.GetComponentInChildren<SpriteRenderer>();
            //Enable placement radius on all pre-existing towers & environment
            ShowPlacementRadius?.Invoke(this, EventArgs.Empty); 
        }
        #endregion

        #region User Interface
        private void UpdateTowerUI(int towerIndex)
        {
            _towers[towerIndex]._amountTMPro.text = "" + _towers[towerIndex]._amount;
            switch (_towers[towerIndex]._amount)
            {
                case 0: //If player has no more towers of this type to place, player cannot place more of this tower type
                    _towers[towerIndex]._usableButton.interactable = false;
                    _towerCostTexts[towerIndex].text =  "MAX CAPACITY";
                    
                    return;
                default:
                    _towers[towerIndex]._usableButton.interactable = true;
                    _towerCostTexts[towerIndex].text =  "" + _towers[towerIndex]._cost;
                    return;
            }
        }
        
        private void CheckIfCanAfford()
        {
            for (int i = 0; i < _towers.Count; i++)
            {
                if (Player_Stats._i._currency < _towers[i]._cost)
                {
                    //Player cannot afford to place this tower
                    _towers[i]._usableButton.interactable = false;
                    _towerCostTexts[i].color = Color.red;
                }
                else if (_towers[i]._amount != 0)
                {
                    //Player can afford to place a tower and has not reached max capacity
                    _towers[i]._usableButton.interactable = true;
                    _towerCostTexts[i].color = Color.white;
                }
                else
                {
                    //Display cost text as white if player can afford the tower, but has reached max capacity
                    _towerCostTexts[i].color = Color.white;
                }

            }
        }

        private bool IsMouseOverUI()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

            //If player's cursor is only over a UI element with the UI_Ignore tag then return false
            //If the player's cursor is over a UI element, return true
            for (int i = 0; i < raycastResultList.Count; i++)
            {
                if (raycastResultList[i].gameObject.tag != "UI_Ignore")
                {
                    raycastResultList.RemoveAt(i);
                    i--;
                }
            }

            return raycastResultList.Count > 0;
        }

        #endregion

    }
}
