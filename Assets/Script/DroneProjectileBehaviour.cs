using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneProjectileBehaviour : MonoBehaviour
{

    public Ball ball; // La balle que le projectile va suivre

    private Rigidbody rb;
    private float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = rb.velocity.magnitude;
    }

    private void Update()
    {
        // Change la direction du projectile pour pointer vers la balle
        Vector3 direction = (ball.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    //On collision with ball destroy self and push the ball
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            collision.rigidbody.AddForce(transform.forward * 20, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

}
