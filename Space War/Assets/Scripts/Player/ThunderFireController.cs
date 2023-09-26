using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderFireController : MonoBehaviour {

    [Tooltip("the offset between the player and the enemy")]
    private float offset;
    
    [Tooltip("use to store the other collider")]
    private Collider collider;

    void Start()
    {
        offset = 15f;
        Destroy(gameObject, 16);
        transform.localScale = new Vector3(0.2f, offset, 0.01f);
    }

    void Update() { 
        
        if (collider == null)
        {
            OnTriggerExit(collider);
        }

        //shoot the lazer
        if (GameObject.Find("firePoint") != null)
        {
            transform.position = GameObject.Find("firePoint").transform.position + new Vector3(0, offset/2, 0);
        }
       
    }

    void OnTriggerStay(Collider others)
    {
        //if collide with enemy, set the offset between player and enemy
        if (others.tag == "Enemy")
        {
            if (GameObject.Find("firePoint") != null)
            {
                offset = others.transform.position.y - GameObject.Find("firePoint").transform.position.y;
            }

            //change lazer's length by offset
            transform.localScale = new Vector3(0.2f, offset, 0.01f);
            //store the collider
            collider = others;
        }

        //if collide with small boss, set the offset between player and small boss
        if (others.tag == "LittleBoss")
        {
            if (GameObject.Find("firePoint") != null)
            {
                offset = others.transform.position.y - GameObject.Find("firePoint").transform.position.y;
            }

            //change lazer's length by offset
            transform.localScale = new Vector3(0.2f, offset, 0.01f);
            //store the collider
            collider = others;
        }

        //if collide with boss, set the offset between player and boss
        if (others.name == "BigBoss")
        {       
            if (GameObject.Find("firePoint") != null)
            {
                offset = others.transform.position.y - GameObject.Find("firePoint").transform.position.y;
            }

            //change lazer's length by offset
            transform.localScale = new Vector3(0.2f, offset, 0.01f);
            //store the collider
            collider = others;
        }
    }

    void OnTriggerExit(Collider others)
    {
        //reset the lazer if exit collider
            offset = 15f;
            transform.localScale = new Vector3(0.2f, offset, 0.01f);
    }


}
