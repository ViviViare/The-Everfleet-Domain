using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBehaviour : MonoBehaviour
{
    
    public float _panSpeed = 20f;

    private void Update()
    {

        //Move camera using the W.A.S.D keys

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * _panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * _panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * _panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * _panSpeed * Time.deltaTime, Space.World);
        }
        
    }
}
