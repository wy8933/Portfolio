using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemiesControl : MonoBehaviour {
    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;

    [Tooltip("Enmy hp")]
    private int enemyBlood = 100;

    [Tooltip("array of item prefabs enemy can drap"), SerializeField]
    private GameObject[] itemPrefabs;

	// Use this for initialization
	void Start () {
        //destroy after 8 seconds
        Destroy(gameObject, 8);
	}
	
	void Update () {
		//move the enemy downward by world space
        transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * 4,Space.World);
        //rotate
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 90);

        //die when hp less or equals to 0
        if(enemyBlood<=0){
            Instantiate(explode, transform.position, Quaternion.identity);
            Destroy(gameObject);
            
            //add score
            Status.status.score+=10;
            
            //roll the chance to spawn item
            int number = Random.Range(0, 10);
            
            //spawn item when rolled a 6
            if (number == 6)
            {
                //roll again to decide which item
                number = Random.Range(0, 4);
                Instantiate(itemPrefabs[number], transform.position, Quaternion.identity);
            }
        }

        
	}

    //determine what to do by the collision type
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            enemyBlood = 0;
        }
        else if(other.tag == "bullet")
        {
            enemyBlood -= 50;
            Destroy(other.gameObject);
        }
        else if(other.tag == "ThunderFireLight"){
            enemyBlood =0;
        }
    }
}

