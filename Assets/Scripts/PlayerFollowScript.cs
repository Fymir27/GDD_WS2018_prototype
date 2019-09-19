using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowScript : MonoBehaviour {

    public enum CameraMode
    {
        Scrolling,
        Falling,
        Following
    }

    public Transform PlayerTransform;

    public static PlayerFollowScript Instance;

    public CameraMode Mode;

    

    //public bool fullFollow = false;

    float smoothTime = 0.05f;
    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        Instance = this;
        Mode = CameraMode.Scrolling;
    }

    private void Update()
    {
        if (PlayerTransform == null)
        {
            Debug.Log("Player Transform null!");
        }

        Vector3 target = Vector3.zero;

        float x = 0;
        if(LevelGeneration.ActiveLevel != null)
            x = LevelGeneration.ActiveLevel.transform.position.x;

        switch (Mode)
        {
            case CameraMode.Scrolling:                
                target = new Vector3(x, Mathf.Max(transform.position.y, PlayerTransform.position.y));
                break;

            case CameraMode.Falling:
                target = new Vector3(transform.position.x, PlayerTransform.position.y);
                break;

            case CameraMode.Following:
                target = PlayerTransform.position;
                break;
        }

        float distance = (target - transform.position).magnitude;

        bool smooth = false;

        if (!smooth)
        {
            transform.position = target + Vector3.back * 10;
        }
        else
        {

            if (distance < 0.5)
            {
                transform.position = target + Vector3.back * 10;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime) + Vector3.back * 10;
            }
        }



    }
}