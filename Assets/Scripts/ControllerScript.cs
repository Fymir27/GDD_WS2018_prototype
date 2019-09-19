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


    [SerializeField]
    private BoxCollider2D player;

    public static ControllerScript Instance;

    public Image GameOverMenu;
    public Image MainMenu;
    public GameObject Player;

    [SerializeField]
    LevelGeneration[] levelGenerators;

    public AudioSource audioWalking;
    public AudioSource audioOther;

    public AudioClip jump;
    public AudioClip land;
    public AudioClip die;
    public AudioClip powerup;

    public bool GameOver = true;

    [SerializeField]
    GameObject[] levelBarriers;
    [SerializeField]
    GameObject[] levelNames;

    private void Awake()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f);
        CheckForClearedLevels();
    }



    // Use this for initialization
    void Start () {
        Instance = this;
        //GameOver = true;
        //MainMenu.gameObject.SetActive(true);
        //Player.SetActive(false);
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
        CheckForClearedLevels();
        GameOver = true;
        PlayerBehaviourPhysics2.Instance.OnDeath();      
        PlaySound(Sound.Die);
        PlayerFollowScript.Instance.Mode = PlayerFollowScript.CameraMode.Falling;
        LevelGeneration.ActiveLevel.RepositionMenu();
        // show gameOverButton
        //MainMenu.gameObject.SetActive(true);
        // disable Player
        //Player.SetActive(false);
    }

    public void OnMainMenuReenter()
    {
        PlayerBehaviourPhysics2.Instance.OnRevive();      
        //GameOver = false;
        Debug.Log("Main Menu Reentered!");
    }

    public void CheckForClearedLevels()
    {
        int maxLevelReached = PlayerPrefs.GetInt("level", 0);

        for (int i = 1; i <= maxLevelReached; i++)
        {
            levelBarriers[i].SetActive(false);
            levelNames[i].SetActive(true);
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        GameOverMenu.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        MainMenu.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameOver = false;
        MainMenu.gameObject.SetActive(false);
        Player.SetActive(true);
        Player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-200, 100));
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ResetLevels()
    {
        foreach (var generator in levelGenerators)
        {
            generator.ResetCompletely();
        }
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

    public void LevelUnlocked(int level)
    {
        if (level == 0 || level > 2)
            return;

        PlayerPrefs.SetInt("level", level);
    }
}