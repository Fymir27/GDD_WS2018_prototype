using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool isPlayer = collision.gameObject.CompareTag("Player");

        if (isPlayer)
        {
            ControllerScript.Instance.PlaySound(Sound.Die);
            ControllerScript.Instance.OnGameOver();
        }
    }
}
