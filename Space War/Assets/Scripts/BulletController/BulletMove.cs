using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {
	void Start () {
		//destroy the bullet after 5 second
        Destroy(gameObject, 3);
	}
	
	void Update () {
        //player bullet movement
        transform.Translate(new Vector3(0,1,0) * Time.deltaTime * 10);
	}

    
}
