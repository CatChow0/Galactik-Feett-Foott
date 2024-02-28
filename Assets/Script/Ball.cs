using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [Header("Ball Settings")]
    // Initialisation des variables changeables dans l'�diteur
    [SerializeField] private PlayerBehaviour1 player;
    [SerializeField] private float pushForce;
    private Rigidbody rb;

    private void Awake()
    {
        // Recuperation du rigidbody
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.position = ballStartPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        Debug.DrawRay(transform.position, (player.transform.position - transform.position), Color.green);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // V�rifie si le joueur est en collision avec la balle
        if (collision.transform.CompareTag("Player"))
        {
             //Debug.Log("Collision enter");

            // Ajoute une force � la balle
            rb.AddForce((transform.position - collision.transform.position).normalized * pushForce * (collision.rigidbody.velocity.magnitude / 7.5f));
        }
       
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Collision stay");
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Collision exit");
    }
}
