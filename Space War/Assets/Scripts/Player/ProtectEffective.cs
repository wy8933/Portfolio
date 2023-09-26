using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectEffective : MonoBehaviour {
    [Tooltip("the mesh for the shiled")]
    private MeshRenderer mesh;

    [Tooltip("the explostion effect of the bullet"), SerializeField]
    private GameObject explode;

    [Tooltip("The time that the shild could stay")]
    private float time;

    //the palyer's collider
    private Collider collider;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        collider = GameObject.Find("Player").GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        //make the shield follows the player
        if (GameObject.Find("Player")!=null)
        {
            transform.position = GameObject.Find("Player").transform.position;
        }

        //the time shild stay
        time += Time.deltaTime;
        if (time > 7)
        {
            Destroy(gameObject);
            collider.enabled = true;
        }
    }


    void OnTriggerEnter(Collider others)
    {   
        //destroy enemy object if trigger
        if (others.tag == "Enemy")
        {
            Destroy(others.gameObject);
            Instantiate(explode, others.transform.position, Quaternion.identity);
        }
        else if (others.tag == "EnemiesBullet")
        {
            Destroy(others.gameObject);
            Instantiate(explode, others.transform.position, Quaternion.identity);
        }
        else if (others.tag == "LittleBoss")
        {
            Destroy(others.gameObject);
            Instantiate(explode, others.transform.position, Quaternion.identity);
        }

        //if trigger with item, let the player collider handle it
        else if (others.tag == "item")
        {
            collider.SendMessage("OnTriggerEnter",others);
        }
    }
}
