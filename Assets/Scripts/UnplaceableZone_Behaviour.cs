using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace viviviare
{
    public class UnplaceableZone_Behaviour : MonoBehaviour
    {
        private Color _cameraDefaultColour;

        private void Start()
        {
            _cameraDefaultColour = Camera.main.backgroundColor;
            TowerPlacement._i.ShowPlacementRadius += ShowPlacementRadius;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        //Display all Environment placement radius GameObjects when event is triggered
        public void ShowPlacementRadius(object sender, EventArgs e)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            //Change background colour to red
            Camera.main.backgroundColor = Color.red;
            
            TowerPlacement._i.ShowPlacementRadius -= ShowPlacementRadius;
            TowerPlacement._i.HidePlacementRadius += HidePlacementRadius;
        }

        //Hide all Environment placement radius GameObjects when event is triggered
        private void HidePlacementRadius(object sender, EventArgs e)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            //Change background colour back to default
            Camera.main.backgroundColor = _cameraDefaultColour;

            TowerPlacement._i.HidePlacementRadius -= HidePlacementRadius;
            TowerPlacement._i.ShowPlacementRadius += ShowPlacementRadius;
        }
    }
}
