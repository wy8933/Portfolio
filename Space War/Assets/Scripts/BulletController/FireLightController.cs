using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//boss的激光碰撞后效果
public class FireLightController : MonoBehaviour {
    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;

    void OnTriggerEnter(Collider others)
    {
        if (others.tag == "Player")
        {
            //find the player's location and create explosion
            transform.position = GameObject.Find("Player").transform.position;
            Instantiate(explode, transform.position, Quaternion.identity);
            Status.status.lifeModify(-1);
        }
    }

 
}
