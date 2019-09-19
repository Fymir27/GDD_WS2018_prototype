using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourPhysics : MonoBehaviour {

    enum PlayerStatus
    {
        Airborne,
        StuckLeft,
        StuckRight,
        StuckTop,
        StuckBottom
    }

    [SerializeField]
    Vector2 crawlDirection;
    Vector2 prevCrawlDirection;

    [SerializeField]
    Vector2 jumpDirection;

    [SerializeField]

    //Dictionary<CrawlDirection, Vector3> movement;
    public bool phaseSwitch = true; // is on when player status changes until he collides with wall again and sticks

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    [SerializeField]
    PlayerStatus status;
    //[SerializeField]
    //CrawlDirection crawlDirection;

    bool nudge = true;

    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float jumpForceX;
    [SerializeField]
    float jumpForceY;

    [SerializeField]
    bool buttonWasPressed = false;

    List<Vector2> directionsClockwise = new List<Vector2>()
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };

    List<Vector2> directionsCounterclockwise = new List<Vector2>()
    {
        Vector2.up,
        Vector2.left,
        Vector2.down,
        Vector2.right
    };

    private Animator animator;
    private int frameCounter;
    public Vector2 PlayerSize;
    public Vector3 PlayerExtents;
    public Vector3 PlayerCenter
    {
        get
        {
            return GetComponent<BoxCollider2D>().bounds.center;
        }
    }

    public Vector2 CrawlDirection
    {
        get
        {
            return crawlDirection;
        }

        set
        {
            prevCrawlDirection = crawlDirection;
            crawlDirection = value;
        }
    }

    // Use this for initialization
    void Start () {
        PlayerExtents = GetComponent<BoxCollider2D>().bounds.extents + new Vector3(0,0);
        PlayerSize = GetComponent<BoxCollider2D>().bounds.size;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        rigidbody.AddForce(new Vector2(-200, 100));
        animator = GetComponent<Animator>();
        frameCounter = 15;
        Vector3 myScale = transform.localScale;
        if(myScale.x < -1)
        {
            myScale.x *= -1;
            transform.localScale = myScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        IsPlayerGrounded(Physics2D.gravity.normalized);

        if (Input.GetKey(KeyCode.Space) || Input.touches.GetLength(0) > 0)
        {
            if (status != PlayerStatus.Airborne)
            {
                buttonWasPressed = true;
            }

           

            Vector2 direction = Vector2.zero;

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
                    if(!IsPlayerGrounded(Vector2.left))
                    {
                        CrawlDirection = GetNextDirection(CrawlDirection, false);

                        //rigidbody.velocity = Vector2.zero;
                        Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
                    }
                    break;

                case PlayerStatus.StuckRight:
                    if (!IsPlayerGrounded(Vector2.right))
                    {               
                        CrawlDirection = GetNextDirection(CrawlDirection, true);

                        //transform.position += new Vector3(CrawlDirection.x, CrawlDirection.y);
                        //rigidbody.velocity = Vector2.zero;                       
                        Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
                    }
                    break;

                case PlayerStatus.StuckTop:
                    if (!IsPlayerGrounded(Vector2.up))
                    {
                        if (CrawlDirection == Vector2.right)
                        {
                            //Debug.Log("TOP -> LEFT");
                            CrawlDirection = Vector2.up;
                            rigidbody.velocity = Vector2.zero;
                            Physics2D.gravity = Vector2.left * Physics2D.gravity.magnitude;
                        }
                        else if (CrawlDirection == Vector2.left)
                        {
                            //Debug.Log("TOP -> RIGHT");
                            CrawlDirection = Vector2.up;
                            rigidbody.velocity = Vector2.zero;
                            Physics2D.gravity = Vector2.right * Physics2D.gravity.magnitude;
                        }
                    }
                    break;

                case PlayerStatus.StuckBottom:
                    if (!IsPlayerGrounded(Vector2.down))
                    {
                        if (CrawlDirection == Vector2.right)
                        {
                            //Debug.Log("TOP -> LEFT");
                            CrawlDirection = Vector2.down;
                            rigidbody.velocity = Vector2.zero;
                            Physics2D.gravity = Vector2.left * Physics2D.gravity.magnitude;
                        }
                        else if (CrawlDirection == Vector2.left)
                        {
                            //Debug.Log("TOP -> RIGHT");
                            CrawlDirection = Vector2.down;
                            rigidbody.velocity = Vector2.zero;
                            Physics2D.gravity = Vector2.right * Physics2D.gravity.magnitude;
                        }
                    }
                    break;
            }

            if (status != PlayerStatus.Airborne)
            {
                animator.SetBool("isMoving", true);
                ControllerScript.Instance.PlaySound(Sound.Walk);
                if (CrawlDirection.x == 0)
                {
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x, movementSpeed * CrawlDirection.y);
                }
                else
                {
                    rigidbody.velocity = new Vector2(movementSpeed * CrawlDirection.x, rigidbody.velocity.y);
                }
            }


        }
        else if (buttonWasPressed)
        {
            frameCounter = 8;
            ControllerScript.Instance.PlaySound(Sound.Jump);
            Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
            rigidbody.velocity = Vector2.zero; // just to control the jump better
            rigidbody.AddForce(CrawlDirection * jumpForceY + jumpDirection * jumpForceX);
            buttonWasPressed = false;
            status = PlayerStatus.Airborne;
            animator.SetBool("jumping", true);
        }

        if(animator.GetBool("jumping") == true && frameCounter == 0)
        {
            Vector3 myScale = transform.localScale;
            myScale.x *= -1;
            transform.localScale = myScale;
            frameCounter = -1;
            animator.SetBool("isMoving", false);
        }
        else
        {
            frameCounter -= 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(status != PlayerStatus.Airborne)
        {
            return;
        }
        ControllerScript.Instance.PlaySound(Sound.Land);
        animator.SetBool("jumping", false);

        var contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);

        float collisionNormalX = Mathf.Round(contactPoints[0].normal.x);
        float collisionNormalY = Mathf.Round(contactPoints[0].normal.y);

        var collisionNormal = new Vector2(collisionNormalX, collisionNormalY);

        if (collisionNormal == Vector2.right)
        {
            status = PlayerStatus.StuckLeft;
            CrawlDirection = Vector2.up; //rigidbody.velocity.y > 0 ? Vector2.up : Vector2.down;
            jumpDirection = Vector2.right;
            Physics2D.gravity = Vector2.left * Physics2D.gravity.magnitude;
        }
        else if (collisionNormal == Vector2.left)
        {
            status = PlayerStatus.StuckRight;
            CrawlDirection = Vector2.up; //rigidbody.velocity.y > 0 ? Vector2.up : Vector2.down;
            jumpDirection = Vector2.left;
            Physics2D.gravity = Vector2.right * Physics2D.gravity.magnitude;

        }
        else if (collisionNormal == Vector2.up)
        {
            status = PlayerStatus.StuckBottom;
            CrawlDirection = rigidbody.velocity.x > 0 ? Vector2.right : Vector2.left;
            jumpDirection = Vector2.up;
            Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;

        }
        else if (collisionNormal == Vector2.down)
        {
            status = PlayerStatus.StuckTop;
            CrawlDirection = rigidbody.velocity.x > 0 ? Vector2.left : Vector2.right;
            jumpDirection = Vector2.up;
            Physics2D.gravity = Vector2.up * Physics2D.gravity.magnitude;
        }

        rigidbody.velocity = Vector2.zero;
    }

    private Vector2 GetNextDirection(Vector2 curDir, bool clockwise)
    {
        List<Vector2> directions = clockwise ? directionsClockwise : directionsCounterclockwise;

        int curIndex = directions.IndexOf(curDir);

        int newIndex = (curIndex + 1) % 4; // there's 4 directions

        Vector2 result =  directions[newIndex];

        //Debug.Log("Next Direction: " + result);

        return result;
    }

    private bool IsPlayerGrounded(Vector2 dir)
    {
        float rayDistance = 1f;

        Vector3 rayStart;
        Vector3 rayStart2;
        Vector3 boxStart;
        Vector2 boxSize = new Vector2(PlayerExtents.x * 2, rayDistance);
        float boxAngle = 0;

        RaycastHit2D hit;
        RaycastHit2D hit2;

        if (dir == Vector2.up)
        {
            rayStart = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
            boxStart = PlayerCenter + new Vector3(0, PlayerExtents.y);
            
        }
        else if (dir == Vector2.right)
        {
            rayStart = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
            boxStart = PlayerCenter + new Vector3(PlayerExtents.x, 0);
        }
        else if (dir == Vector2.down)
        {
            rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
            boxStart = PlayerCenter + new Vector3(0, -PlayerExtents.y);
        }
        else if (dir == Vector2.left)
        {
            rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
            boxStart = PlayerCenter + new Vector3(-PlayerExtents.x, 0);
        }
        else
        {
            Debug.Log("IsPlayerGrounded() -> Invalid direction! " + dir);
            return true;
        }

        var boxHit = Physics2D.BoxCast(PlayerCenter, collider.bounds.size, 0, dir, rayDistance);

        hit = Physics2D.Raycast(rayStart, dir, rayDistance);
        hit2 = Physics2D.Raycast(rayStart2, dir, rayDistance);

        Debug.DrawRay(rayStart, dir, Color.red, 0, false);
        Debug.DrawRay(rayStart2, dir, Color.red, 0, false);

        bool result = boxHit.collider != null;//hit.collider != null || hit2.collider != null;

        if(result == false)
        {
            Debug.Log("Player not grounded! " + dir);
        }
        /*else
        {
            if(hit.collider.gameObject.name.Equals("Player") || hit2.collider.gameObject.name.Equals("Player"))
            {
                Debug.Log("Ray hit Player!");
            }
        }*/
        return result;
    }
}
