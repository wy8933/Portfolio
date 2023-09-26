using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
    [Tooltip("the explostion effect of the bullet"),SerializeField]
    private GameObject explode;

	void Start () {
        //destroy the bullet after 3 seconds
        Destroy(gameObject, 3);
	}
	
	void Update () {
        //bullet movement
        transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * 10);
	}

    //Deduct the player's hp when collide and create explosion effect
    void OnTriggerEnter(Collider others)
    {
        if(others.tag == "Player")
        {
            Instantiate(explode, transform.position, Quaternion.identity);
            Status.status.lifeModify(-1);
            Destroy(gameObject);
            
        }
    }
}
