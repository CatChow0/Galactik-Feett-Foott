using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerBehaviour1 : MonoBehaviour
{

    // Initialisation des variables changeables dans l'�diteur
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int id;

    // Initialisation des variables
    private Rigidbody rb;
    float hor, vert;
    bool jump, jetpack;

    private void Awake()
    {
        // R�cup�ration du rigidbody
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
        }
        else if (id == 2)
        {
            hor = Input.GetAxis("Horizontal2");
            vert = Input.GetAxis("Vertical2");
            jump = Input.GetButtonDown("Jump2");
            jetpack = Input.GetButton("Fire2");
        }

        if (jump)
        {
            rb.AddForce(transform.up * jumpForce);
        }

        // Gestion du jetpack
        if (jetpack)
        {
            rb.AddForce(transform.up * 50);
        }

        //float mouseX = Input.GetAxis("Mouse X");
        //transform.Rotate(transform.up, angularSpeed * mouseX);

        // Rotation du joueur
        transform.Rotate(transform.up, angularSpeed * hor);

        // Debug
        Debug.DrawRay(transform.position, transform.forward * 20, Color.blue);
        Debug.DrawRay(transform.position, Vector3.forward * 20, Color.cyan);
    }

    private void FixedUpdate()
    {
        // Deplacement du joueur
        rb.AddForce(transform.forward * moveSpeed * vert);
    }
}
