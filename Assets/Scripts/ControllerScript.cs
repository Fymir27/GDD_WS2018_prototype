using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton - only add this once per scene!
/// </summary>
public class ControllerScript : MonoBehaviour {

    public static ControllerScript Instance;

    public Button GameOverButton;
    public GameObject Player;

    public bool GameOver = false;

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // call this when player has lost
    public void OnGameOver()
    {
        GameOver = true;

        // show gameOverButton
        GameOverButton.gameObject.SetActive(true);

        // disable Player
        Player.SetActive(false);

    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
