using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleBossController : MonoBehaviour {

	void Start () {
        //destroy the gameobject after 3 second
		Destroy(gameObject, 3);
	}
	
	void Update () {
        //move the gameobject downward
		transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * 9);
	}
}

