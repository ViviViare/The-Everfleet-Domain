using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace viviviare
{
    public class Player_Stats : MonoBehaviour
    {
        #region Variables
        public static Player_Stats _i;
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
        
        [Header("Health")]
        [SerializeField] public int _health;
        [SerializeField] private TextMeshProUGUI _healthText, _healthTextSpatial;
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private Slider _healthBarSpatial;
        
        [Header("Currency")]
        public int _currency = 100;
        [SerializeField] private int _currencyIncrease = 50;
        private float _currencyDelay = 10;

        #endregion

        private void Start()
        {
            //Increase player currency passively, without Generator towers
            InvokeRepeating("GeneratePassiveIncome", _currencyDelay, _currencyDelay);
            UpdateUI();
        }

        private void GeneratePassiveIncome()
        {
            _currency += _currencyIncrease;
            UpdateUI();
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;

            //Check if player has died
            if (_health <= 0)
            {
                _health = 0;
                Menu_Manager._i.PlayerHasLost();
            }
            UpdateUI();
        }


        public void UpdateUI()
        {
            _healthText.text = "Health: " + _health;
            _currencyText.text = "Gold: " + _currency;
        }
    }
}
