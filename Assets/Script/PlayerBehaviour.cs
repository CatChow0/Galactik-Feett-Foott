using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerBehaviour1 : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int id;

    private Rigidbody rb;
    float hor, vert;
    bool jump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Check id
        if (id == 1)
        {
            hor = Input.GetAxis("Horizontal");
            vert = Input.GetAxis("Vertical");
            jump = Input.GetButtonDown("Jump");
        }
        else if (id == 2)
        {
            hor = Input.GetAxis("Horizontal2");
            vert = Input.GetAxis("Vertical2");
            jump = Input.GetButtonDown("Jump2");
        }

        if (jump)
        {
            rb.AddForce(transform.up * jumpForce);
        }

        //float mouseX = Input.GetAxis("Mouse X");
        //transform.Rotate(transform.up, angularSpeed * mouseX);
        transform.Rotate(transform.up, angularSpeed * hor);
        Debug.DrawRay(transform.position, transform.forward * 20, Color.blue);
        Debug.DrawRay(transform.position, Vector3.forward * 20, Color.cyan);
    }

    private void FixedUpdate()
    {

        rb.AddForce(transform.forward * moveSpeed * vert);
    }
}
