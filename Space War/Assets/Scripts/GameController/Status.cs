using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Status : MonoBehaviour {
    [Tooltip("The point player earns by destroy enemy")]
    public int score;
    public Text scoreText;

    [Tooltip("The number of special bullet player owns")]
    public int SBNumber;
    public Text SBText;

    [Tooltip("The remaining life player owns")]
    public int life;
    public Text lifeText;

    [Tooltip("Cooldown of player taking damage")]
    private float damageCooldown;

    //make status singleton
    public static Status status;

    void Awake () {
        status = this;
    }

    /// <summary>
    /// init the variables
    /// </summary>
    private void Start()
    {
        score = 0;
        SBNumber = 3;
        life = 3;
        damageCooldown = 0;
    }

    void Update () {
        ///update the text to the screen
        scoreText.text = score.ToString();
        lifeText.text = life.ToString();
        SBText.text = SBNumber.ToString();

        damageCooldown -= Time.deltaTime;
	}

    /// <summary>
    /// modify player's life, and only deduct when cool down is over
    /// </summary>
    /// <param name="num"></param>
    public void lifeModify(int num) {

        if (num < 0 || damageCooldown <= 0)
        {
            life += num;
            damageCooldown = 0.5f;
        }
        else {
            life += num;
        }

        
    }
}
