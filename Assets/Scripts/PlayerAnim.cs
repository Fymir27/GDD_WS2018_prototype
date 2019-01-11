using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("isMoving", true);
        }
        /*else if (anim.GetBool("isMoving") == true && Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("jump");
            Vector3 myScale = transform.localScale;
            myScale.x *= -1;
            transform.localScale = myScale;
        }
        else if (rigidBody.velocity.y < 0)
        {
            anim.SetBool("landing", true);
        }*/
        else
        {
            anim.ResetTrigger("jump");
            anim.SetBool("landing", false);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("isMoving", false);
        }
    }
}
