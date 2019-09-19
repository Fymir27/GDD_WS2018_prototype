using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuReenterTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && ControllerScript.Instance.GameOver)
        {
            ControllerScript.Instance.OnMainMenuReenter();
        }        
    }
}
