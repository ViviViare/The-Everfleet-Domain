using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace viviviare
{
    public class Tower_Stats_UI : MonoBehaviour
    {
        public TextMeshProUGUI _name;

        [Header("Current Stats")]
        public TextMeshProUGUI _damage;
        public TextMeshProUGUI _currency;
        public TextMeshProUGUI _rate;
        public TextMeshProUGUI _range;

        [Header("Upgraded Stats")]
        public TextMeshProUGUI _damageNEW;
        public TextMeshProUGUI _currencyNEW;
        public TextMeshProUGUI _rateNEW;
        public TextMeshProUGUI _rangeNEW;

        [Header("Targetting")]
        public TextMeshProUGUI _currentTargetText;
        public Button _nextTarget;
        public TextMeshProUGUI _nextTargetText;
        public Button _previousTarget;
        public TextMeshProUGUI _previousTargetText;

    }
}
