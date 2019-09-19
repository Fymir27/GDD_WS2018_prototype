using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipScript : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)]
    float animationFrequency;

    float animationTime;
    float timeElapsed;

    // Start is called before the first frame update
    void Start()
    {
        animationTime = 1f / animationFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= animationTime)
        {
            timeElapsed = 0;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
