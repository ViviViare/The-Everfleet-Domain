using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace viviviare
{
    public partial class T_C_Shooter : Tower_Blueprint
    {
        #region Variables
        [Header("Enemy Detection")]
        [SerializeField] private float _detectionRate = 0.33f; //Delay of seconds the tower should detect enemies
        private string _viableEnemyType1 = "Enemy_Ground";
        private string _viableEnemyType2 = "Enemy_Air";
        public TargetType _viableTargetType = TargetType.Ground; //Enum for tracking what type of enemy this tower can hit
        public TrackingMethod _trackingMethod = TrackingMethod.First; //Enum for tracking what type of enemy to focus on
        
        //Target Types
        public GameObject _targetEnemy;
        private GameObject _firstEnemy;
        private GameObject _lastEnemy;
        private GameObject _strongestEnemy;
        private GameObject _weakestEnemy;

        [Header("Combat")]
        [SerializeField] private GameObject _attackPrefab;
        [SerializeField] private Transform _attackSpawnPoint;
        [SerializeField] private Image _attackRangeIndicator;
        public int _damage = 1;
        public float _attackRange; //Range that enemies need to be within in order to count as a valid target
        public float _attackDelay = 1f; //Time between each attack
        private float _attackTimer; //Timer to keep track of the live _attackDelay
        public int _shotsPerAttack = 1; //How many shots will be instantiated per attack
        private float _shotDelay = 0.2f; //Time between each shot being instantiated
        public int _hitsPerShot = 1; //How many hits the enemy will take from one shot
        private float _hitDelay = 0.1f; //Time between each hit taking effect
        public float _shotSpeed = 30f; //Speed that the shot will travel
        

        #endregion

        private void Start()
        {
            //Run the code in FindNewTarget() after the amount of seconds _detectionRate is
            InvokeRepeating("FindNewTarget", 0f, _detectionRate);

            _uiStats = _uiStatsCanvas.GetComponent<Tower_Stats_UI>();
            _uiStats._nextTarget.onClick.AddListener(NextTrackingMethod);
            _uiStats._previousTarget.onClick.AddListener(PreviousTrackingMethod);

            //Enable the attack range indicator while placing the tower down
            _attackRangeIndicator.enabled = true;
            //Scale the attack range indicator to be equal to the _attackRange float
            //The scale needs to be increased by (* 1.71) in order to properly equal the size of the _attackRange 
            _attackRangeIndicator.transform.localScale = new Vector2(_attackRange * 1.71f, _attackRange * 1.71f);
            
        }

        private void Update()
        {
            Detection();
            //Only run CheckUI if the tower has been placed
            if (_placed) CheckUI();
        }

        private void Detection()
        {
            //Guard clause: If there is no target, do not run.
            if (_targetEnemy != null) IsTargetInRange(ref _targetEnemy);
            if (_targetEnemy == null) return;

            //If the attack timer has reached 0, start an attack and increase the timer again
            if (_attackTimer <= 0f)
            {
                StartCoroutine(Attack());
                _attackTimer = _attackDelay / 1f;
            }

            _attackTimer -= Time.deltaTime;
        }

        #region Target acquisition
        private void FindNewTarget()
        {
            if (!_placed) return;
            //Check if targets are in range
            List<GameObject> viableEnemies = GetViableEnemies();

            float localFirstDistance = 0; //Cached distance of the enemy furthest away by Distance
            float localLastDistance = Mathf.Infinity; //Cached distance of the enemy closest to the enemy spawn by Distance
            
            int localFirstIndex = 0; //Cached distance of the enemy furthest away by Waypoint Index
            int localLastIndex = 999; //Cached distance of the enemy closest to the enemy spawn by Index

            float localHighestHealth = 0; //Cached health value (GREATEST RECORDED)
            float localLowestHealth = Mathf.Infinity; //Cached health value (LOWEST RECORDED)
            
            
            foreach (GameObject enemy in viableEnemies)
            {
                Enemy_Blueprint enemyBlueprint = enemy.GetComponent<Enemy_Blueprint>();
                
                int newIndex = enemyBlueprint._waypointIndex;
                float newDistance = Vector3.Distance(enemyBlueprint._lastWaypoint.transform.position, enemy.transform.position);

                //FIRST ENEMY CHECK

                //If the enemy's index is greater than the cached furthest index, that enemy is the new first
                //If the enemy's index is the same as the cached furthest index, and the enemy's distance is greater than the cached furthest distance, that enemy is the new first

                if (newIndex > localFirstIndex || newIndex == localFirstIndex && newDistance > localFirstDistance)
                {
                    _firstEnemy = enemy;
                    localFirstIndex = newIndex;
                    localFirstDistance = newDistance;
                }

                //If the enemy's index is lower than the cached closest index, that enemy is the new last
                //If the enemy's index is the same as the cached closest index, and the enemy's distance is lower than the cached closest distance, that enemy is the new last

                //LAST ENEMY CHECK
                if (newIndex < localLastIndex || newIndex == localLastIndex && newDistance < localLastDistance)
                {
                    _lastEnemy = enemy;
                    localLastIndex = newIndex;
                    localLastDistance = newDistance;
                }

                //If the enemy's health is greater than the cached greatest health, that enemy is the new strongest

                //STRONGEST ENEMY CHECK
                if (enemyBlueprint._health > localHighestHealth)
                {
                    _strongestEnemy = enemy;
                    localHighestHealth = enemyBlueprint._health;
                }

                //If the enemy's health is lower than the cached lowest health, that enemy is the new weakest

                //WEAKEST ENEMY CHECK
                if (enemyBlueprint._health < localLowestHealth)
                {
                    _weakestEnemy = enemy;
                    localLowestHealth = enemyBlueprint._health;
                }

            }
            
            //Check if any targets have left range
            if (_firstEnemy != null) IsTargetInRange(ref _firstEnemy);
            if (_lastEnemy != null) IsTargetInRange(ref _lastEnemy);
            if (_strongestEnemy != null) IsTargetInRange(ref _strongestEnemy);
            if (_weakestEnemy != null) IsTargetInRange(ref _weakestEnemy);
            
            //Update the target enemy
            switch (_trackingMethod)
            {
                case TrackingMethod.First:
                    _targetEnemy = _firstEnemy;
                    break;
                case TrackingMethod.Last:
                    _targetEnemy = _lastEnemy;
                    break;
                case TrackingMethod.Strongest:
                    _targetEnemy = _strongestEnemy;
                    break;
                case TrackingMethod.Weakest:
                    _targetEnemy = _weakestEnemy;
                    break;
                default:
                    _targetEnemy = _lastEnemy;
                    break;
            }
            
        }

        private void IsTargetInRange(ref GameObject targetObject)
        {
            float dist = Vector3.Distance(transform.position, targetObject.transform.position);

            if (dist > _attackRange)
            {
                targetObject = null;
            }

            if (targetObject.activeSelf == false)
            {
                targetObject = null;
            }
        }
        
        private List<GameObject> GetViableEnemies()
        {
            List<GameObject> allGroundEnemies = new List<GameObject> (GameObject.FindGameObjectsWithTag(_viableEnemyType1)); 
            List<GameObject> allAirEnemies = new List<GameObject> (GameObject.FindGameObjectsWithTag(_viableEnemyType2));

            //Loop through all enemies in the Ground enemies list and remove any that are not within range of the tower
            foreach (GameObject enemy in allGroundEnemies.ToArray())
            {
                float distanceFromTower = Vector3.Distance(transform.position, enemy.transform.position);
                
                if (distanceFromTower > _attackRange)  
                {
                    allGroundEnemies.Remove(enemy);
                }
            }

            //Loop through all enemies in the Air enemies list and remove any that are not within range of the tower
            foreach (GameObject enemy in allAirEnemies.ToArray())
            {
                float distanceFromTower = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceFromTower > _attackRange)  
                {
                    allAirEnemies.Remove(enemy);
                }
            }

            //Finally return the list of enemies that this tower should be able to access
            switch (_viableTargetType)
            {
                case TargetType.Ground:
                    return allGroundEnemies;
                case TargetType.Air: 
                    return allAirEnemies;
                case TargetType.Both:
                    List<GameObject> allEnemies = new List<GameObject> (allGroundEnemies.Concat(allAirEnemies).ToList());
                    return allEnemies;
                default:
                    return null;
            }
            
        }

        public void NextTrackingMethod()
        {
            switch (_trackingMethod)
            {
                case TrackingMethod.First:
                    _trackingMethod = TrackingMethod.Last;
                    _uiStats._currentTargetText.text = "Last"; //Next case

                    _uiStats._nextTargetText.text = "Weakest"; //Case after next case
                    _uiStats._previousTargetText.text = "First"; //Case before this case
                    break;
                case TrackingMethod.Last:
                    _trackingMethod = TrackingMethod.Weakest;
                    _uiStats._currentTargetText.text = "Weakest";

                    _uiStats._nextTargetText.text = "Strongest";
                    _uiStats._previousTargetText.text = "Last";
                    break;
                case TrackingMethod.Weakest:
                    _trackingMethod = TrackingMethod.Strongest;
                    _uiStats._currentTargetText.text = "Strongest";

                    _uiStats._nextTargetText.text = "First";
                    _uiStats._previousTargetText.text = "Weakest";
                    break;
                case TrackingMethod.Strongest:
                    _trackingMethod = TrackingMethod.First;
                    _uiStats._currentTargetText.text = "First";

                    _uiStats._nextTargetText.text = "Last";
                    _uiStats._previousTargetText.text = "Strongest";
                    break;
                default:
                    break;
            }
        }

        public void PreviousTrackingMethod()
        {
            switch (_trackingMethod)
            {
                case TrackingMethod.First:
                    _trackingMethod = TrackingMethod.Strongest;
                    _uiStats._currentTargetText.text = "Strongest"; //Previous case

                    _uiStats._nextTargetText.text = "First"; //Case after Previous case
                    _uiStats._previousTargetText.text = "Weakest"; //Case before Previous case 
                    break;
                case TrackingMethod.Last:
                    _trackingMethod = TrackingMethod.First;
                    _uiStats._currentTargetText.text = "First";

                    _uiStats._nextTargetText.text = "Last"; 
                    _uiStats._previousTargetText.text = "Strongest"; 
                    break;
                case TrackingMethod.Weakest:
                    _trackingMethod = TrackingMethod.Last;
                    _uiStats._currentTargetText.text = "Last";

                    _uiStats._nextTargetText.text = "Weakest"; 
                    _uiStats._previousTargetText.text = "First";
                    break;
                case TrackingMethod.Strongest:
                    _trackingMethod = TrackingMethod.Weakest;
                    _uiStats._currentTargetText.text = "Weakest";

                    _uiStats._nextTargetText.text = "Strongest";
                    _uiStats._previousTargetText.text = "Last";
                    break;
                default:
                    break;
            }
        }

        #endregion End Target acquisition

        private IEnumerator Attack()
        {
            int shots = 0;
            //Shoot as many shots as the _shotsPerAttack variable allows
            for (shots = 0; shots < _shotsPerAttack; shots++)
            {
                GameObject newShot = PoolManager.Spawn(_attackPrefab, _attackSpawnPoint.position, Quaternion.identity);
                //Grab a reference to the new shot's Projectile script
                Projectile shotProj = newShot.GetComponent<Projectile>();

                if (shotProj == null) yield break;
                //Pass through this turrets stats to the projectile
                shotProj.SetupProjectile(_targetEnemy.transform, _hitsPerShot, _shotSpeed, _hitDelay, _damage);

                //Before continuing the loop, wait for the duration of the _shotDelay
                yield return new WaitForSeconds(_shotDelay);
            }

            
            
        }

        protected override void UpdateStatUI()
        {
            _uiStats._damage.text = "Damage: " + _damage;
            _uiStats._rate.text = "Rate: " + _attackDelay;
            _uiStats._range.text = "Range: " + _attackRange;
        }

        public override void ShowRangeUI()
        {
            _attackRangeIndicator.enabled = true;
        }

        public override void HideRangeUI()
        {
            _attackRangeIndicator.enabled = false;
        }

        private void OnDrawGizmos()
        {
            //Display the attack range in Editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
