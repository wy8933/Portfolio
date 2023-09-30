using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    [Tooltip("Button to restart the game"),SerializeField]
    public Button restartButton;
    [Tooltip("Button to quit the game"), SerializeField]
    public Button quitButton;
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
                //show the text and button
                restartButton.gameObject.SetActive(true);
                quitButton.gameObject.SetActive(true);
                GameOver.gameObject.SetActive(true);

                //pause the game
                Time.timeScale = 0;
            }
        }


	}

    /// <summary>
    /// restart the game
    /// </summary>
    public void ReStratGame()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// quit the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
