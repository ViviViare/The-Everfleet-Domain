using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace viviviare
{
    public class Enemy_Blueprint : MonoBehaviour
    {
        #region Main Variables
        
        [Header("Enemy waypoint movement")]
        [SerializeField] protected Transform _spawnPos;
        protected List<Transform> _waypoints = new List<Transform>(); //List of all Waypoints for this enemy to follow
        public Transform _lastWaypoint; //Last waypoint the enemy moved past, used for Tower Tracking
        public int _waypointIndex; //Amount of waypoints the enemy has moved past.

        [Header("Base enemy stats")]

        //Base values for the public ones to default to
        [SerializeField] private int _baseMaxHealth = 10; 
        [SerializeField] private int _baseDamage = 0;
        [SerializeField] private float _baseMoveSpeed = 0.75f;

        [Header("Public enemy stats")]
        
        //Public values to be used
        public int _health;
        public int _damage;
        public float _moveSpeed;
        

        [Header("Miscellaneous variables")]
        [SerializeField] private Image _healthBar;

        #endregion

        private void Start()
        {
            _waypoints = Waypoints._waypoints; //Copy the waypoints from the waypoint script
            _spawnPos = GameObject.FindGameObjectWithTag("EnemySpawn").transform;
            ResetSettings(); //Initialize stats at start 
        }

        private void Update()
        {
            MoveToWaypoints();
        }

        private void MoveToWaypoints()
        {
            //Move the enemy towards the waypoint corrosponding to the current waypointIndex at the speed of this enemy type
            transform.position = Vector3.MoveTowards(transform.position, _waypoints[_waypointIndex].transform.position, _moveSpeed * Time.deltaTime);

            //If the enemy has reached the waypoint, check to see if the enemy has more waypoints to go through
            if (Vector3.Distance(transform.position, _waypoints[_waypointIndex].transform.position) < 0.1f)
            {
                if (_waypointIndex < _waypoints.Count - 1)
                {
                    //Cache the waypoint the enemy just went past for the Tower Tracking
                    _lastWaypoint = _waypoints[_waypointIndex].transform;
                    _waypointIndex++;
                }
                else
                {
                    ReachedEnd();
                }
            }
        }
        
        public void TakeDamage(int incomingDamage)
        {
            _health -= incomingDamage;
            //Set the health bar icon to be equal to the Health divided by the Max Health
            _healthBar.fillAmount = (float)_health / (float)_baseMaxHealth;
            if (_health <= 0) //Check to see if the enemy has died
            {
                ResetSettings();
                EnemyWaves._livingEnemies--;
                PoolManager.Despawn(gameObject);
            }
        }

        private void ResetSettings()
        {
            _health = _baseMaxHealth;
            _moveSpeed = _baseMoveSpeed;
            _damage = _baseDamage;

            _waypointIndex = 0;
            _lastWaypoint = _spawnPos;

            _healthBar.fillAmount = 1;
        }

        private void ReachedEnd()
        {
            ResetSettings();
            EnemyWaves._livingEnemies--;
            PoolManager.Despawn(gameObject);
            Player_Stats._i.TakeDamage(_health); //Player takes damage equal to the remaining health of the enemy
            
        }

        

    }
}
