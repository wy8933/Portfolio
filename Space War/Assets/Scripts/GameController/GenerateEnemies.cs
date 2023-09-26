using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemies : MonoBehaviour {
    [Tooltip("Gameobject of the small boss"),SerializeField]
    private GameObject smallBoss;
    [Tooltip("Gameobject of the normal enemy"), SerializeField]
    private GameObject enemyPrefab;
    [Tooltip("Gameobject of the boss"), SerializeField] 
    private GameObject boss;
    
    //record time
    float enemyTimer = 0;
    float bossTimer = 0;

    [Tooltip("switch that determine if there is boss or not")]
    private int _switch = 1;
    
	void Update () {
        //update the time
        enemyTimer += Time.deltaTime;
        bossTimer += Time.deltaTime;

        //spawn enemy at random spot
        if (enemyTimer >= 0.5f)
        {
            enemyTimer = 0;
            float x = Random.Range(-4.5f, 5f);
            Instantiate(enemyPrefab, new Vector3(x, 7, -0.5f), Quaternion.identity);
        }

        //spawn boss
        if (bossTimer >= 5 && _switch == 1)
        {
            _switch = 0;
            Instantiate(boss, new Vector3(0, 9.05f, -0.5f), this.transform.rotation);
        }

        //boss出来后拥有群机突袭技能
        if (bossTimer >= 10 && _switch == 0)
        {
            //when time is bigger than 10，check if the boss is dead, if yes，reset to keep spawn boss
            if (GameObject.Find("BigBoss(Clone)") == null)
            {
                _switch = 1;
                bossTimer = 0;
            }else
            {
                //spawn 5 small bosses
                Instantiate(smallBoss, new Vector3(0, 4.27f, -0.5f), this.transform.rotation);
                Instantiate(smallBoss, new Vector3(-2.02f, 4.91f, -0.5f), this.transform.rotation);
                Instantiate(smallBoss, new Vector3(2.02f, 4.91f, -0.5f), this.transform.rotation);
                Instantiate(smallBoss, new Vector3(3.62f, 5.71f, -0.5f), this.transform.rotation);
                Instantiate(smallBoss, new Vector3(-3.62f, 5.71f, -0.5f), this.transform.rotation);
                bossTimer = 5;
            }

        }
	}
}
