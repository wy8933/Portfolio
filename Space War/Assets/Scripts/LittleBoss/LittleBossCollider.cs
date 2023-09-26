using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleBossCollider : MonoBehaviour {
    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;

    [Tooltip("hp of the small boss")]
    private int blood = 100;

    [Tooltip("The small boss game object"), SerializeField]
    private GameObject smallBoss;
	// Update is called once per frame
	void Update () {
        //died when the hp is less than or equals to 0
        if (blood <= 0)
        {
            //destroy the small boss
            Instantiate(explode, this.transform.position, Quaternion.identity);
            Destroy(smallBoss);
        }
	}

    void OnTriggerEnter(Collider others)
    {
        //Deduct the player's hp by one if collide with the player
        if(others.tag == "Player")
        {
            Status.status.lifeModify(-1);
            Instantiate(explode, this.transform.position,Quaternion.identity);
            Destroy(smallBoss);
        }

        //Deduct the small boss's hp if collide with bullet
        if (others.tag == "bullet")
        {
            blood -= 50;
            Destroy(others.gameObject);
        }

        //kill the small boss if collide the the lazer
        if (others.tag == "ThunderFireLight")
        {
            blood =0;
        }

    }
}
