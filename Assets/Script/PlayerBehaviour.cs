using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    // Class for the player object
    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    public float gravityModifier = 2.0f;
    public bool isOnGround = true;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity *= gravityModifier;

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
