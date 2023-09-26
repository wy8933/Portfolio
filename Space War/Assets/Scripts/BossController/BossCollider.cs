using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Boss collision
/// </summary>
public class BossCollider : MonoBehaviour {
    [Tooltip("The part of the boss Collider"),SerializeField]
    private GameObject bossCollider;

    [Tooltip("Explosion effect when boss is damaged"), SerializeField]
    private GameObject explosionPrefab;

    [Tooltip("The boss game object"), SerializeField]
    private GameObject boss;

    [Tooltip("The controller that control as the AI of the boss"),SerializeField]
    private BossController bossController;

    void Update () {
        //Boss died when it's hp reaches 0
        if (bossController.bossBlood <= 0)
        {
            Destroy(boss);
            //It's lazer should also disapear
            if (GameObject.Find("FireLight") != null)
            {
                Destroy(GameObject.Find("FireLight"));
            }
            //player gain score for killing the boss
            Status.status.score += 200;
        }
	}

    void OnTriggerEnter(Collider others)
    {
        if (others.tag == "bullet")
        {
            //destroy the bullet and create explosion
            Destroy(others);
            Instantiate(explosionPrefab, others.transform.position, Quaternion.identity);
            bossController.bossBlood -= 100;
        }
        if (others.tag == "Player")
        {
            //kill player if they collide with the boss
            Status.status.life = 0;
            //create the explosion effect
            Instantiate(explosionPrefab, others.transform.position, Quaternion.identity);
        }
        if (others.tag == "ThunderFireLight")
        {
            //deduct hp when hit by the player's lazer
            bossController.bossBlood -= 300;
        }
    }
}
