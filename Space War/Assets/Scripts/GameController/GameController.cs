using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    [Tooltip("Text to show the game is over"), SerializeField]
    public Text GameOver;

    [Tooltip("The time of the game being paused")]
    private float pauseTime;

	// Use this for initialization
	void Start () {
        pauseTime = 0;
        Time.timeScale = 1;
    }
	
	// Update is called once per frame
	void Update () {
        //show the canvas when the player is dead
        if (Status.status.life <= 0)
        {
            pauseTime += Time.deltaTime;
            if (pauseTime >= 0.4f)
            {
                GameOver.gameObject.SetActive(true);

                //pause the game
                Time.timeScale = 0;

                //restart game
                if (Input.GetKeyDown(KeyCode.R)){
                    SceneManager.LoadScene("Plane");
                }
            }
        }


	}

    /// <summary>
    /// restart the game
    /// </summary>
    public void ReStratGame()
    {
        SceneManager.LoadScene("plane");
    }

    /// <summary>
    /// quit the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
