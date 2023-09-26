using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//激光跟随boss发射点移动
public class FireLightMove : MonoBehaviour {
	void Start () {
        //destroy the object after 1 second
        Destroy(gameObject, 1);
	}
	
	void Update () {
        //make the lazer trace the shooting point
        if (GameObject.Find("FireLightPoint") != null)
        {
            transform.position = GameObject.Find("FireLightPoint").transform.position + new Vector3(0, -6.25f, 0);
        }
	}
}
