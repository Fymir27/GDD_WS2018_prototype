using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapewormBehaviour : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)]
    float animationFrequency;

    [SerializeField, Range(1f, 5f)]
    float movementSpeed;

    float animationTime;
    float timeElapsed;

    int timesToAppearAgain = 2;

    // Start is called before the first frame update
    void Start()
    {
        animationTime = 1f / animationFrequency;
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= animationTime)
        {
            timeElapsed = 0;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        var pos = transform.position;
        pos.y -= movementSpeed * Time.deltaTime;
        transform.position = pos;
    }

    private void OnBecameInvisible()
    {
        return;

        if (!enabled || timesToAppearAgain == 0)
            return;

        var pos = transform.position;
        pos.y += 30;
        transform.position = pos;

        timesToAppearAgain--;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }
}
