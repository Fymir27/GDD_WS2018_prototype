using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    enum PlayerStatus
    {
        Airborne,
        StuckLeft,
        StuckRight,
        StuckTop,
        StuckBottom
    }

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    [SerializeField]
    PlayerStatus status;

    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float jumpForceX;
    [SerializeField]
    float jumpForceY;

    [SerializeField]
    bool buttonWasPressed = false;

	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        rigidbody.AddForce(new Vector2(-100, 0));
	}
	
	// Update is called once per frame
	void Update ()
    {      
        if(Input.GetKey(KeyCode.Space) || Input.touches.GetLength(0) > 0)
        {
            Vector3 movement = Vector2.zero;
            switch (status)
            {
                case PlayerStatus.Airborne:
                    break;

                case PlayerStatus.StuckLeft:
                    movement = new Vector2(0, movementSpeed);
                    buttonWasPressed = true;
                    break;

                case PlayerStatus.StuckRight:
                    movement = new Vector2(0, movementSpeed);
                    buttonWasPressed = true;
                    break;

                case PlayerStatus.StuckTop:
                    movement = new Vector2(-movementSpeed, 0);
                    buttonWasPressed = true;
                    break;

                case PlayerStatus.StuckBottom:
                    movement = new Vector2(movementSpeed, 0);
                    buttonWasPressed = true;
                    break;

                default:
                    break;
            }
            transform.position += movement;
        }
        else if(buttonWasPressed)
        {
            Vector2 jumpVector = Vector2.zero;

            switch (status)
            {
                case PlayerStatus.Airborne:
                    break;

                case PlayerStatus.StuckLeft:
                    jumpVector = new Vector2(jumpForceX, jumpForceY);
                    break;

                case PlayerStatus.StuckRight:
                    jumpVector = new Vector2(- jumpForceX, jumpForceY);
                    break;

                case PlayerStatus.StuckTop:
                    // just let the player drop?
                    break;

                case PlayerStatus.StuckBottom:
                    jumpVector = new Vector2(jumpForceX, jumpForceY);
                    break;

                default:
                    break;
            }

            status = PlayerStatus.Airborne;
            rigidbody.AddForce(jumpVector);
            rigidbody.gravityScale = 1;
            buttonWasPressed = false;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);

        float collisionNormalX = Mathf.Round(contactPoints[0].normal.x);
        float collisionNormalY = Mathf.Round(contactPoints[0].normal.y);

        var collisionNormal = new Vector2(collisionNormalX, collisionNormalY);

        if (collisionNormal == Vector2.right)
        {
            status = PlayerStatus.StuckLeft;
        }
        else if(collisionNormal == Vector2.left)
        {
            status = PlayerStatus.StuckRight;
        }
        else if (collisionNormal == Vector2.up)
        {
            status = PlayerStatus.StuckBottom;
        }
        else if (collisionNormal == Vector2.down)
        {
            status = PlayerStatus.StuckTop;
        }

        rigidbody.gravityScale = 0;
        rigidbody.velocity = Vector2.zero;
    }



    

}
