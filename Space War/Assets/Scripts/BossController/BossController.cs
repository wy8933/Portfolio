using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {
    //dertermine to direction of move movement
    private bool left = true;

    [Tooltip("The game object of bullet boss shoot"),SerializeField]
    private GameObject bullet;

    [Tooltip("the game object of lazer boss shoot"), SerializeField]
    private GameObject fireLight;
    [Tooltip("the position of boss's lazer shoots"), SerializeField]
    private GameObject fireLightPoint;

    [Tooltip("The position of the boss's fires"), SerializeField]
    private GameObject[] firePos;

    //time of game progress
    private float bulletTimer = 0;
    private float lazerTimer = 0;

    [Tooltip("Boss's hp")]
    public int bossBlood = 10000;

    void FixedUpdate()
    {
        //move to specific height before start to move left and right
        if (transform.position.y >= 5.89)
        {
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * 5);
        }
        else 
        {
            //move to left
            if (transform.position.x >= -3.2 && left)
            {
                transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * 6);

                if (transform.position.x <= -3.2)
                {
                    left = false;
                }

            }
            else
            {
                //set the direction to left when reach the right end
                if (transform.position.x >= 3.2)
                {
                    left = true;
                }
                //move to right
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * 6);
            }

            //add time to the timers
            bulletTimer += Time.deltaTime;
            lazerTimer += Time.deltaTime;

            //shoots bullet every 1.5 seconds
            if (bulletTimer > 1.5f)
            {
                for (int i = 0; i < 8; i++) {
                    Instantiate(bullet, firePos[i].transform.position, Quaternion.identity);
                }
                bulletTimer = 0;
            }

            //shoot lazer every 5 seconds
            if (lazerTimer > 5)
            {
                Instantiate(fireLight, new Vector3(fireLightPoint.transform.position.x, fireLightPoint.transform.position.y, fireLightPoint.transform.position.z), Quaternion.identity);
                lazerTimer = 0;
            }
        }
  
    }
}
