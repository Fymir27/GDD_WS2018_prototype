using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeScript : MonoBehaviour
{
    public float volumeSet;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioListener.volume = volumeSet;
            PlayerPrefs.SetFloat("volume", volumeSet);
        }
    }
}
