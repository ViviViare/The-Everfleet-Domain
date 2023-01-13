using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viviviare
{
    public class Projectile : MonoBehaviour
    {
        private Transform _target;
        private int _hitsPerShot;
        private float _shotSpeed;
        private float _hitDelay;
        private bool _finishedSetup;
        private int _damage;

        public void SetupProjectile (Transform newTarget, int newHitsPerAttack, float newShotSpeed, float newHitDelay, int newDamage)
        {
            _target = newTarget;
            _hitsPerShot = newHitsPerAttack;
            _shotSpeed = newShotSpeed;
            _hitDelay = newHitDelay;
            _damage = newDamage;

            _finishedSetup = true;
            
        }

        //Reset the projectiles settings back to default
        private void ResetSettings()
        {
            _target = null;
            _hitsPerShot = 0;
            _shotSpeed = 0;
            _hitDelay = 0;

            _finishedSetup = false;
        }


        private void Update()
        {
            //Guard Clause: Do not run if the projectiles settings haven't been initialized
            if (!_finishedSetup) return;
            
            //Despawn projectile if the target is inactive
            if (!_target.gameObject.activeSelf)
            {
                PoolManager.Despawn(gameObject);
                ResetSettings();
                return;
            }
            
            StartCoroutine(MoveTowardsTarget());
        }        
        private IEnumerator MoveTowardsTarget()
        {
            Vector3 direction = _target.position - transform.position;
            float distanceThisFrame = _shotSpeed * Time.deltaTime;
            
            //Check if projectile has successfully hit target
            if (direction.magnitude <= distanceThisFrame)
            {
                //Hit the enemy the amount of times the tower's _hitsPerShot allows
                int hits = 0;
                for (hits = 0; hits < _hitsPerShot; hits++)
                {
                    HitTarget();
                    yield return new WaitForSeconds(_hitDelay);
                }
                yield break;
            }

            //Move towards the target
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }
        private void HitTarget()
        {
            Enemy_Blueprint blueprint = _target.gameObject.GetComponent<Enemy_Blueprint>();
            
            blueprint.TakeDamage(_damage); //Deal damage to enemy
            PoolManager.Despawn(gameObject); //Remove projectile and place back into the projectile pool
            ResetSettings(); //Reset projectile settings for next use
        }



    }
}
