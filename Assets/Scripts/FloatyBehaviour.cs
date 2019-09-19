using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyBehaviour : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)]
    float frequency;

    [SerializeField, Range(0.001f, 0.01f)]
    float floatSpeed;

    float animationTimeHalf;
    float timeElapsed;

    Vector3 movementPerFrame;

    // Start is called before the first frame update
    void Start()
    {
        animationTimeHalf = 0.5f / frequency;
        movementPerFrame = new Vector3(0, floatSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= animationTimeHalf)
        {
            timeElapsed = 0;
            movementPerFrame = -movementPerFrame;
        }

        transform.position = transform.position + movementPerFrame;
    }
}
