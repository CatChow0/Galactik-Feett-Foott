using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Settings")]
    // Initialisation des variables changeables dans l'editeur
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxSlowSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jetpackForce;
    [SerializeField] public int id;
    [SerializeField] private int fov;
    [SerializeField] private Camera cam;

    [Header("Player Dash settings")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashFov;

    [Header("Grapple Hook Settings")]
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grappleMinRange;
    [SerializeField] private Ball targetBall;
    [SerializeField] private GameObject grappleObject;


    private bool isGrappling;
    private Quaternion initialRotation;

    // Initialisation des variables
    private Rigidbody rb;
    float hor, vert, currentSpeed;
    bool slow, jump, jetpack, jumpAllow, dash, grappleHook;

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
            grappleHook = Input.GetButtonDown("GrappleHook");
        }
        else if (id == 2)
        {
            hor = Input.GetAxis("Horizontal2");
            vert = Input.GetAxis("Vertical2");
            jump = Input.GetButtonDown("Jump2");
            jetpack = Input.GetButton("Fire2");
            slow = Input.GetButton("Slow2");
            dash = Input.GetButtonDown("Dash2");
            grappleHook = Input.GetButtonDown("GrappleHook1");
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

        if (grappleHook)
        {
            // si le joueur est en train de grapple
            if (isGrappling)
            {
                isGrappling = false;
            }
            else
            {
                Grapple();
            }
        }

        if (isGrappling)
        {
            // Fait pivoter le joueur vers la balle de manière douce
            Quaternion targetRotation = Quaternion.LookRotation(targetBall.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * grappleSpeed * 0.1f); // réduit la vitesse de rotation du joueur

            grappleObject.SetActive(true);

            // Modifie la position et la taille de l'objet en fonction de la position du joueur et de la balle
            Vector3 playerPosition = transform.position;
            Vector3 ballPosition = targetBall.transform.position;
            Vector3 midPoint = (playerPosition + ballPosition) / 2;

            grappleObject.transform.position = midPoint;
            grappleObject.transform.LookAt(ballPosition);
            grappleObject.transform.localScale = new Vector3(grappleObject.transform.localScale.x, grappleObject.transform.localScale.y, Vector3.Distance(playerPosition, ballPosition));

        }
        else
        {
            // Rotation du joueur
            transform.Rotate(transform.up, angularSpeed * hor);

            // Rétablit la rotation initiale du joueur en x et z
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, currentRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            grappleObject.SetActive(false);
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

        if (isGrappling)
        {
            float step = grappleSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetBall.transform.position, step);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Verifie si le joueur est en collision avec le sol
        if (collision.transform.CompareTag("Terrain"))
        {
            //Debug.Log("Collision enter with ground");

            jumpAllow = true;
        }

        // Verifie si le joueur est en collision avec la balle
        if (collision.gameObject == targetBall.gameObject && isGrappling)
        {
            isGrappling = false;

            // Ajoute une force à la balle dans la direction du mouvement du joueur
            Rigidbody ballRigidbody = targetBall.GetComponent<Rigidbody>();
            Vector3 pushDirection = (targetBall.transform.position - transform.position).normalized;
            ballRigidbody.AddForce(pushDirection * grappleSpeed, ForceMode.Impulse);
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
        // Recupere la direction du joueur et ajoute une force dans cette direction pour le dash et utilise une coroutine pour le temps de dash et un lerp pour la vitesse
        Vector3 dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        // set la fov de la camera a 120 en utilisant un lerp
        StartCoroutine(DashTime());
        DoFov(dashFov);
    }

    private IEnumerator DashTime()
    {
        // Attend le temps de dash et remet la vitesse du joueur a la normale
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.8f);
        DoFov(fov);
    }

    private void DoFov(float end_value)
    {
        // Clean fov

        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, end_value, 0.5f);

        cam.DOFieldOfView(end_value, 0.15f);
    }

    public int GetID()
    {
        return id;
    }

    private void Grapple()
    {
        // Vérifie si la balle est à portée
        if (Vector3.Distance(transform.position, targetBall.transform.position) >= grappleMinRange)
        {
            // Grapple
            isGrappling = true;
            initialRotation = transform.rotation;
        }
    }
}
