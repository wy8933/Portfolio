using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBullet : MonoBehaviour {
    [Tooltip("the timer of special bullet to explode")]
    private float timer;

    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;
    
    [Tooltip("array of all the enemy objects")]
    private GameObject[] obj;
	
	void Update () {
        timer += Time.deltaTime;
        transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * 10);
        if (timer >= 0.3f)
        {
            Destroy(gameObject);
            Instantiate(explode);

            obj = GameObject.FindGameObjectsWithTag("Enemy");
            //destroy all enemy
            for (int i = 0; i < obj.Length; i++)
            {
                Destroy(obj[i]);
                Status.status.score += 10;
            }
            //destory all enemy bullet
            obj = GameObject.FindGameObjectsWithTag("EnemiesBullet");
            for (int i = 0; i < obj.Length; i++)
            {
                Destroy(obj[i]);
            }
            
            //destroy all the small boss
            obj = GameObject.FindGameObjectsWithTag("LittleBoss");
            for (int i = 0; i < obj.Length; i++)
            {
                Destroy(obj[i]);
            }
        }
        

	}
}
