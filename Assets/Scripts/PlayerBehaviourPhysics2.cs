using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourPhysics2 : MonoBehaviour
{
    enum PlayerStatus
    {
        Airborne,
        StuckLeft,
        StuckRight,
        StuckTop,
        StuckBottom
    }

    // singleton instace
    public static PlayerBehaviourPhysics2 Instance;

    [SerializeField]
    Vector2 crawlDirection;
    Vector2 prevCrawlDirection;

    [SerializeField]
    Vector2 jumpDirection;

    [SerializeField]
    Transform DeathWall;

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

    [SerializeField]
    bool clockwise;

    // when character dies
    bool falling = false;

    public bool TunnelBehaviourOn = false;

    /// <summary>
    /// Player doesn't do anything on collision a few ms after he jumped
    /// to avoid him recolliding with the wall he just jumped off,
    /// especially where two walls meet etc.
    /// </summary>
    [SerializeField]
    float sDelayAfterJump;
    float sSinceJump = 0f;

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

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        status = PlayerStatus.Airborne;
        CrawlDirection = Vector2.up;
        PlayerExtents = GetComponent<BoxCollider2D>().bounds.extents + new Vector3(0.01f, 0.01f);
        PlayerSize = GetComponent<BoxCollider2D>().bounds.size;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();      
        animator = GetComponent<Animator>();
        frameCounter = 15;

        rigidbody.AddForce(Vector2.right * jumpForceX * 0.2f);

        /*
        Vector3 myScale = transform.localScale;
        if (myScale.x < -1)
        {
            myScale.x *= -1;
            transform.localScale = myScale;
        }
        */
    }

    private void Update()
    {
        sSinceJump += Time.deltaTime;

        CalculateAndSetRotation();
    }

    private void CalculateAndSetRotation()
    {
        var scale = transform.localScale;
        scale.y = clockwise ? -.65f : .65f;
        transform.localScale = scale;

        float rotation = 0f;
        if(CrawlDirection == Vector2.left)
        {
            rotation = clockwise ? 270f : 90f;
        }
        else if(CrawlDirection == Vector2.right)
        {
            rotation = clockwise ? 90f : 270f;
        }
        else if(CrawlDirection == Vector2.up)
        {
            rotation = clockwise ? 180f : 0f;
        }
        else if(CrawlDirection == Vector2.down)
        {
            rotation = clockwise ? 0f : 180f;
        }

        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, rotation));
    }

    // Physics update
    // TODO: move input to update
    void FixedUpdate()
    {
        if (transform.position.y < DeathWall.position.y)
        {
            //ControllerScript.Instance.Reload();
            //return;
        }

        bool grounded = IsPlayerGrounded(Physics2D.gravity.normalized);

        if (Input.GetKey(KeyCode.Space) || Input.touches.GetLength(0) > 0)
        {
            if (status != PlayerStatus.Airborne && !ControllerScript.Instance.GameOver)
            {
                buttonWasPressed = true;
            }
            else
            {
                return;
            }

            if(!grounded && rigidbody.velocity.magnitude != 0)
            {
                //if(clockwise)
                    //transform.Rotate(Vector3.back, 90);
                //else
                    //transform.Rotate(Vector3.back, -90);

                jumpDirection = CrawlDirection;
                CrawlDirection = GetNextDirection(CrawlDirection, clockwise);
                Physics2D.gravity = Physics2D.gravity.magnitude * GetNextDirection(CrawlDirection, clockwise);
                rigidbody.velocity = new Vector2(0, 0);
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
            buttonWasPressed = false;

            sSinceJump = 0f;

            bool jumpHorizontally = CrawlDirection == Vector2.up || CrawlDirection == Vector2.down;

            if (ControllerScript.Instance.GameOver)
                return;

            animator.SetBool("jumping", true);

            if (jumpHorizontally)
            {
                frameCounter = 8;
                ControllerScript.Instance.PlaySound(Sound.Jump);
                Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
                rigidbody.velocity = Vector2.zero; // just to control the jump better
                rigidbody.AddForce(Vector2.up * jumpForceY + jumpDirection * jumpForceX);
                buttonWasPressed = false;
                status = PlayerStatus.Airborne;               
                if (CrawlDirection == Vector2.up)
                {
                    clockwise = !clockwise;
                }
                CrawlDirection = Vector2.up;
            }
            else
            {
                // player falls if upside down
                if (Physics2D.gravity.normalized == Vector2.up)
                {
                    Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
                    status = PlayerStatus.Airborne;
                    //clockwise = !clockwise;
                }
                else
                {
                    rigidbody.AddForce(Vector2.up * jumpForceY * 1.3f);
                    status = PlayerStatus.Airborne;
                    //CrawlDirection = -CrawlDirection;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(falling) // reached bottom of main menu
        {
            Debug.Log("Hit the ground!");
            // Reset levels
            falling = false;
            crawlDirection = Vector2.left;
            clockwise = false;
            PlayerFollowScript.Instance.Mode = PlayerFollowScript.CameraMode.Scrolling;
            ControllerScript.Instance.GameOver = false;
            status = PlayerStatus.StuckBottom;
            ControllerScript.Instance.ResetLevels();


        }

        if (ControllerScript.Instance.GameOver)
        {
            return;
        }

        if (sSinceJump <= sDelayAfterJump)
        {
            return;
        }

        var contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);

        float collisionNormalX = Mathf.Round(contactPoints[0].normal.x);
        float collisionNormalY = Mathf.Round(contactPoints[0].normal.y);

        Vector2 collisionNormal = collision.contacts[0].normal.normalized; //new Vector2(collisionNormalX, collisionNormalY);


        if (status != PlayerStatus.Airborne)
        {
            if (collisionNormal == -Physics2D.gravity.normalized)
                return;

            // to avoid getting stuck where two walls meet when crawling up
            if (collisionNormal == Vector2.down)
            {
                Debug.Log("Stuck!");
                var pos = transform.position;
                pos.x -= Physics2D.gravity.normalized.x * 0.05f;
                transform.position = pos;
            }

            if (TunnelBehaviourOn)
            {
                Physics2D.gravity = crawlDirection * Physics2D.gravity.magnitude;
                crawlDirection = GetNextDirection(crawlDirection, !clockwise);
                jumpDirection = collisionNormal;
                rigidbody.velocity = Vector2.zero;               
            }
            return;
        }

        ControllerScript.Instance.PlaySound(Sound.Land);
        animator.SetBool("jumping", false);
        animator.SetBool("isMoving", false);

        if (collisionNormal == Vector2.right)
        {
            //transform.localRotation = Quaternion.AngleAxis(0, Vector3.back);
            CrawlDirection = Vector2.up;
            clockwise = false;
            Physics2D.gravity = Vector2.left * Physics2D.gravity.magnitude;
        }
        else if (collisionNormal == Vector2.left)
        {
            //transform.localRotation = Quaternion.AngleAxis(180, Vector3.back);
            CrawlDirection = Vector2.up;
            clockwise = true;
            Physics2D.gravity = Vector2.right * Physics2D.gravity.magnitude;
        }
        else if (collisionNormal == Vector2.up)
        {
            if(rigidbody.velocity.x > 0)
            {
                CrawlDirection = Vector2.left;
                clockwise = false;
            }
            else
            {
                CrawlDirection = Vector2.right;
                clockwise = true;
            }
            Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;
        }
        else if (collisionNormal == Vector2.down)
        {
            if (crawlDirection == Vector2.right)
            {
                CrawlDirection = Vector2.left;
                clockwise = true;
            }
            else if(crawlDirection == Vector2.left)
            {
                CrawlDirection = Vector2.right;
                clockwise = false;
            }
            else if (rigidbody.velocity.x > 0)
            {
                CrawlDirection = Vector2.right;
                clockwise = false;
            }
            else
            {
                CrawlDirection = Vector2.left;
                clockwise = true;
            }
            Physics2D.gravity = Vector2.up * Physics2D.gravity.magnitude;
        }

        jumpDirection = collisionNormal;
        status = PlayerStatus.StuckBottom;
        rigidbody.velocity = Vector2.zero;
    }

    private Vector2 GetNextDirection(Vector2 curDir, bool clockwise)
    {
        List<Vector2> directions = clockwise ? directionsClockwise : directionsCounterclockwise;

        int curIndex = directions.IndexOf(curDir);

        int newIndex = (curIndex + 1) % 4; // there's 4 directions

        Vector2 result = directions[newIndex];

        //Debug.Log("Next Direction: " + result);

        return result;
    }

    private bool IsPlayerGrounded(Vector2 dir)
    {
        float rayDistance = 1f;

        Vector3 rayStart;
        Vector3 rayStart2;

        RaycastHit2D hit;
        RaycastHit2D hit2;

        if (dir == Vector2.up)
        {
            rayStart = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
        }
        else if (dir == Vector2.right)
        {
            rayStart = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, PlayerExtents.y);
        }
        else if (dir == Vector2.down)
        {
            rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(PlayerExtents.x, -PlayerExtents.y);
        }
        else if (dir == Vector2.left)
        {
            rayStart = PlayerCenter + new Vector3(-PlayerExtents.x, PlayerExtents.y);
            rayStart2 = PlayerCenter + new Vector3(-PlayerExtents.x, -PlayerExtents.y);
        }
        else
        {
            Debug.Log("IsPlayerGrounded() -> Invalid direction! " + dir);
            return true;
        }

        hit = Physics2D.Raycast(rayStart, dir, rayDistance);
        hit2 = Physics2D.Raycast(rayStart2, dir, rayDistance);

        Debug.DrawRay(rayStart, dir, Color.red, 0, false);
        Debug.DrawRay(rayStart2, dir, Color.red, 0, false);

        var boxHit = Physics2D.BoxCast(PlayerCenter, collider.bounds.size, 0, dir, rayDistance);

        bool result = boxHit.collider != null; // hit.collider != null || hit2.collider != null;

        /*else
        {
            if(hit.collider.gameObject.name.Equals("Player") || hit2.collider.gameObject.name.Equals("Player"))
            {
                Debug.Log("Ray hit Player!");
            }
        }*/
        return result;
    }

    public void OnDeath()
    {
        collider.isTrigger = true;

        // turn player character upside down to signal death
        //crawlDirection = Vector2.right;
        //clockwise = false;

        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce((new Vector3(LevelGeneration.ActiveLevel.transform.position.x, transform.position.y) - transform.position).normalized * jumpForceX * 0.5f + Vector3.up * jumpForceY * 0.2f);

        Physics2D.gravity = Vector2.down * Physics2D.gravity.magnitude;

        status = PlayerStatus.Airborne;

        animator.SetBool("isMoving", true);
        animator.SetBool("jumping", false);
    }

    public void OnRevive()
    {
        collider.isTrigger = false;
        falling = true; // to prepare for reset on impact at the bottom
        var vel = rigidbody.velocity;
        vel.x = 0;
        rigidbody.velocity = vel;
    }
}
