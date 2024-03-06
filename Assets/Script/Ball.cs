using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [Header("Ball Settings")]
    // Initialisation des variables changeables dans l'éditeur
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private float pushForce;
    [SerializeField] private float maxBallSpeed;
    public int lastPlayerTouchId;
    private Rigidbody rb;

    public AudioSource BouncePlayer;

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

    private void FixedUpdate()
    {
        // Limite la vitesse de la balle
        if (rb.velocity.magnitude > maxBallSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxBallSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si le joueur est en collision avec la balle
        if (collision.transform.CompareTag("Player"))
        {
            // Récupère l'id du joueur qui a touché la balle
            lastPlayerTouchId = collision.transform.GetComponent<PlayerBehaviour>().id;

            // Ajoute une force à la balle
            rb.AddForce((transform.position - collision.transform.position).normalized * pushForce * (collision.rigidbody.velocity.magnitude / 7.5f));
        }
        BouncePlayer.Play();
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
