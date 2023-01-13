using UnityEngine;
using System.Collections.Generic;

namespace viviviare
{
    [CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Waves", order = 1)]
    public class Wave : ScriptableObject
    {
        [field: SerializeField]
        [field: NonReorderable] public List<Enemy> Enemies {get; private set;} //Read-only

        [field: SerializeField]
        public float ThisWaveDelay {get; private set;} //Time until this wave spawns

        [field: SerializeField]
        public float EnemySpawnDelay {get; private set;} //Time between each enemy spawning

    }
    
}
