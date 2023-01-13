using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace viviviare
{    
    [System.Serializable]
    public class Tower
    {
        //Storage for a towers setup stats

        public string _name;
        public GameObject _towerPrefab;
        public int _amount;
        public TextMeshProUGUI _amountTMPro; //Reference to the text displaying the amount of towers the player has left to place of this kind
        public int _cost;
        public TextMeshProUGUI _costTMPro; //Reference to the text displaying the cost to produce this tower
        public Button _usableButton; //Reference to the button that will produce a new tower of this type
    }

}
