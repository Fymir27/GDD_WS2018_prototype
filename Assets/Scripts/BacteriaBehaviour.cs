using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaBehaviour : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)]
    float movementSpeed = 1f;

    Rigidbody2D rb;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = new Vector3(0, movementSpeed); // vertical movement only
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocity = -velocity;
    }
}
