using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viviviare
{
    public class Waypoints : MonoBehaviour
    {
        public static List<Transform> _waypoints = new List<Transform>();

        private void Start()
        {
            //Iterate through all children and add to the _waypoints list
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag == "Waypoints") 
                {
                    _waypoints.Add(transform.GetChild(i));
                }

            }
        }

        private void OnDrawGizmos()
        {
            //Iterate through all children and draw a wiresphere in the editor above the gameobjects position
            foreach (Transform t in transform)
            {
                Gizmos.color = Color.blue;
                if (t.tag == "EnemySpawn") Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(t.position + Vector3.up , 0.5f);

            }

            //Iterate through all the children and draw a line between each one, showing the Enemy Path more clearly in the editor
            Gizmos.color = Color.blue;
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(transform.GetChild(i).position + Vector3.up, transform.GetChild(i + 1).position + Vector3.up);
            }
        }
        


    }
}
