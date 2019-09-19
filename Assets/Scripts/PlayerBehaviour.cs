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

    enum CrawlDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    Dictionary<CrawlDirection, Vector3> movement;


    new Rigidbody2D rigidbody;
    new Collider2D collider;

    [SerializeField]
    PlayerStatus status;
    [SerializeField]
    CrawlDirection crawlDirection;

    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float jumpForceX;
    [SerializeField]
    float jumpForceY;

    [SerializeField]
    bool buttonWasPressed = false;

    public Vector3 PlayerExtents;
    public Vector3 PlayerCenter
    {
        get
        {
            return GetComponent<SpriteRenderer>().bounds.center;
        }
    }

	// Use this for initialization
	void Start ()
    {
        PlayerExtents = GetComponent<SpriteRenderer>().bounds.extents;
        //PlayerCenter = GetComponent<SpriteRenderer>().bounds.center;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        rigidbody.AddForce(new Vector2(-100, 0));

        movement = new Dictionary<CrawlDirection, Vector3>()
        {
            { CrawlDirection.Up, new Vector3(0, movementSpeed) },
            { CrawlDirection.Right, new Vector3(movementSpeed, 0) },
            { CrawlDirection.Down, new Vector3(0, -movementSpeed) },  // ?
            { CrawlDirection.Left, new Vector3(-movementSpeed, 0) }
        };
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.Space) || Input.touches.GetLength(0) > 0)
        {

            
            buttonWasPressed = true;

            Vector3 rayStart = new Vector3();
            Vector3 rayStart2 = new Vector3();
            Vector2 rayDirection = new Vector2();
            RaycastHit2D hit;
            RaycastHit2D hit2;

            switch (status)
            {
                case PlayerStatus.Airborne:
                    break;

                case PlayerStatus.StuckLeft:
                    rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
                    rayStart2 = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
                    rayDirection = Vector2.left;

                    hit = Physics2D.Raycast(rayStart, rayDirection, .2f);
                    hit2 = Physics2D.Raycast(rayStart2, rayDirection, .2f);

                    Debug.DrawRay(rayStart, rayDirection, Color.red, 0, false);
                    Debug.DrawRay(rayStart2, rayDirection, Color.red, 0, false);

                    if (hit.collider == null && hit2.collider == null)
                    {
                        if (crawlDirection == CrawlDirection.Down)
                        {
                            status = PlayerStatus.StuckTop;
                        }
                        else
                        {
                            status = PlayerStatus.StuckBottom;
                        }
                        crawlDirection = CrawlDirection.Left;

                    }
                    break;

                case PlayerStatus.StuckRight:
                    rayStart = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
                    rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
                    rayDirection = Vector2.right;

                    hit = Physics2D.Raycast(rayStart, rayDirection, .2f);
                    hit2 = Physics2D.Raycast(rayStart2, rayDirection, .2f);

                    Debug.DrawRay(rayStart, rayDirection, Color.red, 0, false);
                    Debug.DrawRay(rayStart2, rayDirection, Color.red, 0, false);

                    if (hit.collider == null && hit2.collider == null)
                    {
                        if (crawlDirection == CrawlDirection.Down)
                        {
                            status = PlayerStatus.StuckTop;                           
                        }
                        else
                        {
                            status = PlayerStatus.StuckBottom;
                        }
                        crawlDirection = CrawlDirection.Right;
                    }
                    break;

                case PlayerStatus.StuckTop:
                    rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
                    rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
                    rayDirection = Vector2.up;

                    hit = Physics2D.Raycast(rayStart, rayDirection, .2f);
                    hit2 = Physics2D.Raycast(rayStart2, rayDirection, .2f);

                    Debug.DrawRay(rayStart, rayDirection, Color.red, 0, false);
                    Debug.DrawRay(rayStart2, rayDirection, Color.red, 0, false);

                    if (hit.collider == null && hit2.collider == null)
                    {
                        if(crawlDirection == CrawlDirection.Right)
                        {
                            status = PlayerStatus.StuckLeft;
                        }
                        else
                        {
                            status = PlayerStatus.StuckRight;
                        }
                        crawlDirection = CrawlDirection.Up;
                    }
                    break;

                case PlayerStatus.StuckBottom:
                    rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
                    rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
                    rayDirection = Vector2.down;

                    hit = Physics2D.Raycast(rayStart, rayDirection, .2f);
                    hit2 = Physics2D.Raycast(rayStart2, rayDirection, .2f);

                    Debug.DrawRay(rayStart, rayDirection, Color.red, 0, false);
                    Debug.DrawRay(rayStart2, rayDirection, Color.red, 0, false);

                    if (hit.collider == null && hit2.collider == null)
                    {
                        if (crawlDirection == CrawlDirection.Right)
                        {
                            status = PlayerStatus.StuckLeft;
                        }
                        else
                        {
                            status = PlayerStatus.StuckRight;
                        }
                        crawlDirection = CrawlDirection.Down;
                    }
                    break;

                default:
                    break;
            }
            transform.position += movement[crawlDirection];
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
                    if (crawlDirection == CrawlDirection.Right)
                    {
                        jumpVector = new Vector2(jumpForceX, jumpForceY);
                    }
                    else
                    {
                        jumpVector = new Vector2(-jumpForceX, jumpForceY);
                    }
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
        if(ControllerScript.Instance.GameOver)
        {
            return;
        }

        var contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);

        float collisionNormalX = Mathf.Round(contactPoints[0].normal.x);
        float collisionNormalY = Mathf.Round(contactPoints[0].normal.y);

        var collisionNormal = new Vector2(collisionNormalX, collisionNormalY);

        if (collisionNormal == Vector2.right)
        {
            status = PlayerStatus.StuckLeft;
            crawlDirection = CrawlDirection.Up;
        }
        else if(collisionNormal == Vector2.left)
        {
            status = PlayerStatus.StuckRight;
            crawlDirection = CrawlDirection.Up;
        }
        else if (collisionNormal == Vector2.up)
        {
            status = PlayerStatus.StuckBottom;
            crawlDirection = CrawlDirection.Left;
        }
        else if (collisionNormal == Vector2.down)
        {
            status = PlayerStatus.StuckTop;
            crawlDirection = CrawlDirection.Right;
        }

        rigidbody.gravityScale = 0;
        rigidbody.velocity = Vector2.zero;
    }



    

}
