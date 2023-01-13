using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viviviare
{
    public partial class EnemyWaves : MonoBehaviour
    {
        #region Variables
        //Waves
        public Wave[] _waves;
        private Wave _currentWave;
        private int _waveIndex = 0; //Index of current wave
        private float _newWaveDelay = 2f; //Time until next wave. First wave starts at initialized value.
        private float _defaultWaveDelay = 2f; //Default time until next wave if all enemies are dead

        private Transform _enemySpawnPoint;

        //Enemy status
        public static int _livingEnemies;

        //Guard Clauses
        private bool _startedWaves = false;
        private bool _allWavesEnded = false;
        private bool _waveEnded = true;

        
        #endregion

        private void Start()
        {
            _enemySpawnPoint = GameObject.FindGameObjectWithTag("EnemySpawn").transform;
            _currentWave = _waves[_waveIndex];
            _newWaveDelay = _currentWave.ThisWaveDelay;
        }

        private void Update()
        {
            //Check to see if player has:
            //1) Finished all waves
            //2) Killed all enemies
            //3) Not already got a menu UI open
            if (_allWavesEnded && _livingEnemies <= 0 && !Menu_Manager._i._isMenuUp)
            {
                Menu_Manager._i.PlayerHasWon();
                return;
            }
            //Guard clause: Do not try to spawn new waves if all waves have been completed
            else if (_allWavesEnded) return;

            
            
            
            //If all enemies are dead then start the next wave sooner
            if (_startedWaves && _livingEnemies <= 0 && _newWaveDelay > _defaultWaveDelay) 
            {
                _newWaveDelay = _defaultWaveDelay;
                _waveEnded = true;
            }
            
            //Only spawn a new wave if the previous wave has ended
            if (_waveEnded && Time.time >= _newWaveDelay)
            {
                _waveEnded = false;
                StartCoroutine(SpawnWave());
            }

        }

        private IEnumerator SpawnWave()
        {
            //Adds all of this waves enemies to the total living enemy count
            GetAllEnemiesInWave();
            
            //Iterating through every enemy type in wave
            for (int typeIndex = 0; typeIndex < _currentWave.Enemies.Count; typeIndex++)
            {
                
                for (int amountIndex = 0; amountIndex < _currentWave.Enemies[typeIndex]._amountToSpawn; amountIndex++)
                {
                    //Spawn one version of that enemy type
                    PoolManager.Spawn(_currentWave.Enemies[typeIndex]._enemyPrefab, _enemySpawnPoint.position, Quaternion.identity);
                    
                    yield return new WaitForSeconds(_currentWave.EnemySpawnDelay);
                }
            }
            _startedWaves = true;
            _waveEnded = true;
            NextWave();
        }

        private void GetAllEnemiesInWave()
        { 
            for (int typeIndex = 0; typeIndex < _currentWave.Enemies.Count; typeIndex++)
            {
                for (int amountIndex = 0; amountIndex < _currentWave.Enemies[typeIndex]._amountToSpawn; amountIndex++)
                {
                    _livingEnemies++;
                }
            }
        }

        private void NextWave()
        {
            if (_waveIndex + 1 < _waves.Length) //Iterate through to next wave
            {
                _waveIndex++;
                _currentWave = _waves[_waveIndex];
                _newWaveDelay = Time.time + _currentWave.ThisWaveDelay;
            }
            else
            {
                _allWavesEnded = true; //Toggle guard clause to stop spawning new waves
            }
        }

    }
}
