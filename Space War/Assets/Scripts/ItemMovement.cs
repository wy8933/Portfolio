using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour {
    [Tooltip("The speed that object moves"), SerializeField] 
    private float speed;
	void Update () {
        //rotate and move the object
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 90);
        transform.position += (new Vector3(0,speed,0)*Time.deltaTime);
	}
}
