using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    // dont touch
    [SerializeField]
    bool triggerOnlyTunnelBehaviour = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Flip()
    {
        PlayerBehaviourPhysics2.Instance.TunnelBehaviourOn = !PlayerBehaviourPhysics2.Instance.TunnelBehaviourOn;

        if(triggerOnlyTunnelBehaviour)
        {
            return;
        }

        var mode = PlayerFollowScript.Instance.Mode;

        if(mode == PlayerFollowScript.CameraMode.Following)
        {
            mode = PlayerFollowScript.CameraMode.Scrolling;
        }
        else if(mode == PlayerFollowScript.CameraMode.Scrolling)
        {
            mode = PlayerFollowScript.CameraMode.Following;
        }

        PlayerFollowScript.Instance.Mode = mode;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Flip();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Flip();
    }
}
