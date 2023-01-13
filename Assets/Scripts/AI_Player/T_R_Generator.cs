using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viviviare
{
    public class T_R_Generator : Tower_Blueprint
    {
        //Variables
        [Header("Currency Generation")]
        public int _currencyToGenerate; //Amount of currency to generate
        public float _currencyDelay; //Time between new currency generation
        [SerializeField] private GameObject _generationParticles;
        private float _particleDecay = 5f;


        private void Start()
        {
            //Run the code in FindNewTarget() after the amount of seconds _detectionRate is
            InvokeRepeating("CreateCurrency", _currencyDelay, _currencyDelay);
            _uiStats = _uiStatsCanvas.GetComponent<Tower_Stats_UI>();
        }

        private void Update()
        {
            //Only run CheckUI if the tower has been placed
            if (_placed) CheckUI();
        }

        private void CreateCurrency()
        {
            if (!_placed) return;
            //Increase player currency by the amount this tower generates
            Player_Stats._i._currency += _currencyToGenerate;
            Player_Stats._i.UpdateUI();
            //Create currency particles for player feedback
            StartCoroutine(SpawnCurrencyParticles());
        }

        private IEnumerator SpawnCurrencyParticles()
        {
            //Grab a reference to the newly spawned particles, using Pooling to not use unnecessary Instantiate() or Destroy()
            GameObject newParticles = PoolManager.Spawn(_generationParticles, transform.position + Vector3.up, Quaternion.identity);
            //Rotate the new particles to face upwards
            newParticles.transform.eulerAngles += new Vector3(-90,0,0);
            //After the defined time has passed, despawn the particles 
            yield return new WaitForSeconds(_particleDecay);
            PoolManager.Despawn(newParticles);

        }

        protected override void UpdateStatUI()
        {
            _uiStats._currency.text = "Gold: " + _currencyToGenerate;
            _uiStats._rate.text = "Rate: " + _currencyDelay;
        }

        public override void HideRangeUI()
        {
            //The abstract class HideRangeUI() is used by T_C_Shooter, but defined in Tower_Blueprint.
            //T_R_Generate needs an override for these two methods since it also inherits from Tower_Blueprint 
            return;
        }

        public override void ShowRangeUI()
        {
            return;
        }
    
    
    }
}
