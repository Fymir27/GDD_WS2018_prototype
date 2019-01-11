using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum Sound
{
    Walk,
    Jump,
    Land,
    Die,
    Powerup
}

/// <summary>
/// Singleton - only add this once per scene!
/// </summary>
public class ControllerScript : MonoBehaviour {

    public static ControllerScript Instance;

    public Button GameOverButton;
    public GameObject Player;

    public AudioSource audioWalking;
    public AudioSource audioOther;

    public AudioClip jump;
    public AudioClip land;
    public AudioClip die;
    public AudioClip powerup;

    public bool GameOver = false;

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.R))
        {
            RestartScene();
        }
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

    public void PlaySound(Sound sound)
    {
        if(sound == Sound.Walk)
        {
            if (!audioWalking.isPlaying)
            {
                audioWalking.Play();
            }
        }
        else
        {
            switch (sound)
            {
                case Sound.Walk:
                    break;

                case Sound.Jump:
                    audioOther.clip = jump;
                    break;

                case Sound.Land:
                    if(audioOther.clip != die)
                        audioOther.clip = land;
                    break;

                case Sound.Die:
                    audioOther.clip = die;
                    break;

                case Sound.Powerup:
                    audioOther.clip = powerup;
                    break;
            }
            audioWalking.Pause();
            audioOther.Play();
        }
    }
}
