using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isOnGround = false;
    private CharacterController player_controller;
    public float speed = 10.0f;

    public float smoothTurn = 0.1f;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        player_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float move_horizontal = Input.GetAxisRaw("Horizontal");
        float move_vertical = Input.GetAxisRaw("Vertical");

        moveDirection.x = move_horizontal;
        moveDirection.z = move_vertical;
        moveDirection.Normalize();

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurn, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 move = transform.forward * speed * Time.deltaTime;
            player_controller.Move(move);
        }
        

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
