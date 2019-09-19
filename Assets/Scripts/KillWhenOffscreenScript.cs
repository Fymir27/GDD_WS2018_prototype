using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillWhenOffscreenScript : MonoBehaviour
{

    bool visible = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        if(visible)
        {
            Destroy(gameObject);
        }
    }
}
