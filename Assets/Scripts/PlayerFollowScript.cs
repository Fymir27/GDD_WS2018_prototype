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
            float y = Mathf.Max(transform.position.y, PlayerTransform.position.y);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);

        }
        else
        {
            Debug.Log("Player Transform null!");
        }
	}
}
