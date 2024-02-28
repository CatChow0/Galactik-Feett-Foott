using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerBehaviour1 : MonoBehaviour
{
    [Header("Player Settings")]
    // Initialisation des variables changeables dans l'editeur
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxSlowSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jetpackForce;
    [SerializeField] private int id;

    [Header("Player Dash settings")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashForce;

    // Initialisation des variables
    private Rigidbody rb;
    float hor, vert, currentSpeed;
    bool slow, jump, jetpack, jumpAllow, dash;

    private void Awake()
    {
        // Recuperation du rigidbody
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
            jetpack = Input.GetButton("Fire1");
            slow = Input.GetButton("Slow1");
            dash = Input.GetButtonDown("Dash1");
        }
        else if (id == 2)
        {
            hor = Input.GetAxis("Horizontal2");
            vert = Input.GetAxis("Vertical2");
            jump = Input.GetButtonDown("Jump2");
            jetpack = Input.GetButton("Fire2");
            slow = Input.GetButton("Slow2");
            dash = Input.GetButtonDown("Dash2");
        }

        if (jump && jumpAllow)
        {
            rb.AddForce(transform.up * jumpForce);
        }

        // Gestion du jetpack
        if (jetpack)
        {
            rb.AddForce(transform.up * jetpackForce);
        }

        if (dash)
        {
            Dash();
        }

        // Rotation du joueur
        transform.Rotate(transform.up, angularSpeed * hor);

        // Debug
        Debug.DrawRay(transform.position, transform.forward * 20, Color.blue);
        Debug.DrawRay(transform.position, Vector3.forward * 20, Color.cyan);
    }

    private void FixedUpdate()
    {
        // Deplacement du joueur en marchant/slow
        if (slow)
        {
            currentSpeed = moveSpeed * vert;
            if (currentSpeed >= maxSlowSpeed)
            {
                //Debug.Log("Walking");
                currentSpeed = maxSlowSpeed;
            }
            else if (currentSpeed <= 0 && currentSpeed <= -maxSlowSpeed)
            {
                //Debug.Log("Walking Reverse");
                currentSpeed = -maxSlowSpeed;
            }
        }
        // Deplacement du joueur en temps normal
        else
        {
            currentSpeed = moveSpeed * vert;
            if (currentSpeed >= 0 && currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            else if (currentSpeed <= 0 && currentSpeed <= -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }
        rb.AddForce(transform.forward * currentSpeed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Verifie si le joueur est en collision avec le sol
        if (collision.transform.CompareTag("Terrain"))
        {
            //Debug.Log("Collision enter with ground");

            jumpAllow = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Verifie si le joueur est en collision avec le sol
        if (collision.transform.CompareTag("Terrain"))
        {
            //Debug.Log("Collision stay with ground");

            jumpAllow = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Collision leave with ground");

        jumpAllow = false;
    }

    private void Dash()
    {
        Debug.Log("Dash");
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
    }

}
