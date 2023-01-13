using UnityEngine;

namespace viviviare
{
    [System.Serializable] 
    public class Enemy 
    {
        public string _name; //Name of this enemy type
        public GameObject _enemyPrefab; //Gameobject to spawn for this enemy
        public int _amountToSpawn; //Amount of this enemy to spawn
    }
    
}
