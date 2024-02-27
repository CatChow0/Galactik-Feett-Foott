using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour1 : MonoBehaviour
{

    // Initialisation des variables changeables dans l'�diteur
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;

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
        // R�cup�ration des inputs
        hor = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");
        jetpack = Input.GetButton("Fire1");

        // Gestion du saut 
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
        // D�placement du joueur
        rb.AddForce(transform.forward * moveSpeed * vert);
    }
}
