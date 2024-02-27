using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    // Initialisation des variables changeables dans l'éditeur
    [SerializeField] private PlayerBehaviour1 player;
    [SerializeField] private float pushForce;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        Debug.DrawRay(transform.position, (player.transform.position - transform.position), Color.green);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si le joueur est en collision avec la balle
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Collision enter");

            // Ajoute une force à la balle
            GetComponent<Rigidbody>().AddForce((transform.position - collision.transform.position).normalized * pushForce);
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
