using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

    [Header("Ball Settings")]
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private float pushForce;
    [SerializeField] private float maxBallSpeed;
    public int lastPlayerTouchId;
    private Rigidbody rb;

    public AudioSource BouncePlayer;

    // ------------------------------------------------------- //
    // ----------------- FONCTION PRINCIPALE ----------------- //
    // ------------------------------------------------------- //
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxBallSpeed)                   // Limite la vitesse de la balle
        {
            rb.velocity = rb.velocity.normalized * maxBallSpeed;
        }
    }

    // ----------------------------------------------------------- //
    // ----------------- COLLISION AVEC LA BALLE ----------------- //
    // ----------------------------------------------------------- //

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
}
