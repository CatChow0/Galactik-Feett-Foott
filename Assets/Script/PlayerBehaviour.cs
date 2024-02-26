using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        // Check if the player collided with the ground and not another object
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the player is no longer on the ground
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}
