using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleBossCollider : MonoBehaviour {
    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;

    [Tooltip("hp of the small boss")]
    private int blood = 100;
	
	// Update is called once per frame
	void Update () {
        //died when the hp is less than or equals to 0
        if (blood <= 0)
        {
            //destroy the small boss
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider others)
    {
        //Deduct the player's hp by one if collide with the player
        if(others.tag == "Player")
        {
            Status.status.lifeModify(-1);
            Destroy(gameObject);
            Instantiate(explode, this.transform.position,Quaternion.identity);
        }

        //Deduct the small boss's hp if collide with bullet
        if (others.tag == "bullet")
        {
            blood -= 50;
        }

        //kill the small boss if collide the the lazer
        if (others.tag == "ThunderFireLight")
        {
            blood =0;
        }

    }
}
