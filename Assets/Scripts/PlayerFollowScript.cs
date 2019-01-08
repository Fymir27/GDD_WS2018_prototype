using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowScript : MonoBehaviour {

    public Transform PlayerTransform;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(PlayerTransform != null)
        {
            transform.position = new Vector3(transform.position.x, PlayerTransform.position.y, transform.position.z);

        }
        else
        {
            Debug.Log("Player Transform null!");
        }
	}
}
