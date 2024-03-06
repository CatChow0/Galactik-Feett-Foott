using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Settings")]
    // Initialisation des variables changeables dans l'editeur
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxSlowSpeed;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] public int id;
    [SerializeField] private int fov;
    [SerializeField] private Camera cam;

    [Header("Player Dash settings")]
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashFov;
    [SerializeField] private float dashEnergy;

    [Header("Player Jetpack settings")]
    [SerializeField] private float jetpackForce;
    [SerializeField] private float jetpackMinEnergy;
    [SerializeField] private float jetpackEnergy;
    [SerializeField] private float jetpackMultiplier;

    [Header("Player Energy settings")]
    [SerializeField] public float energyAmount;
    [SerializeField] private float maxEnergyAmount;
    [SerializeField] private AnimationCurve energyRegenCurve;
    [SerializeField] private float energyRegenTime;
    [SerializeField] private float energyRegenCooldown;

    [Header("Grapple Hook Settings")]
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grappleMinRange;
    [SerializeField] private Ball targetBall;
    [SerializeField] private GameObject grappleObject;

    [Header("Movement Cooldown Settings")]
    [SerializeField] private float CooldownRemainingTime;
    private static Timer CooldownTimer;
    


    private bool isGrappling;
    private Quaternion initialRotation;
    private Rigidbody rb;
    float hor, vert, currentSpeed;
    bool slow, jump, jetpack, jumpAllow, dash, grappleHook;
    private bool jetpackUsed = false;
    private bool newJetpackUsed = false;
    private bool jetpackCooldown = false;
    private bool dashUsed = false;
    private bool newDashUsed = false;
    private bool start = false;
    private bool restart = false;
    public float defaultEnergy;

    Image handleImage = null;
    RectTransform handleTransform = null;

    private void Awake()
    {
        // Recuperation du rigidbody
        rb = GetComponent<Rigidbody>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RegenEnergy());
        RetrieveEnergySlider();
        defaultEnergy = energyAmount;
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();

        JumpManager();

        JetpackManager();

        DashManager();

        MovementCooldown();

        GrappleManager();

        EnergySliderUpdate();

        // Debug
        Debug.DrawRay(transform.position, transform.forward * 20, Color.blue);
        Debug.DrawRay(transform.position, Vector3.forward * 20, Color.cyan);
    }

    private void FixedUpdate()
    {
        MovementManager();

        GrapplingManager();
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

            // Ajoute une force � la balle dans la direction du mouvement du joueur
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

    // Gestion du jetpack
    private void JetpackManager()
    {
        if (jetpack && energyAmount >= jetpackEnergy)
        {
            if (!jetpackUsed && energyAmount >= (jetpackMinEnergy * 2))
            {
                energyAmount -= jetpackMinEnergy;
                jetpackUsed = true;
            }

            if (jetpackUsed)
            {
                newJetpackUsed = true;
            }
            rb.AddForce(transform.up * jetpackForce);
            energyAmount -= jetpackEnergy * Time.deltaTime * jetpackMultiplier;
        }
        else
        {
            if (jetpackUsed)
            {
                jetpackUsed = false;
                jetpackCooldown = true;
            }
        }
    }

    // Gestion du dash
    private void DashManager()
    {
        if (dash && energyAmount >= dashEnergy)
        {
            //Debug.Log("Dash");
            if (dashUsed)
            {
                newDashUsed = true;
            }
            else if (!dashUsed)
            {
                dashUsed = true;
            }
            // Recupere la direction du joueur et ajoute une force dans cette direction pour le dash et utilise une coroutine pour le temps de dash et un lerp pour la vitesse
            Vector3 dashDirection = transform.forward;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            // set la fov de la camera a 120 en utilisant un lerp
            StartCoroutine(DashTime());
            energyAmount -= dashEnergy;
            DoFov(dashFov);
        }
    }

    // Coroutine pour le temps de dash
    private IEnumerator DashTime()
    {
        // Attend le temps de dash et remet la vitesse du joueur a la normale
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.8f);
        DoFov(fov);
    }

    // Change la fov de la camera
    private void DoFov(float end_value)
    {
        // Clean fov

        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, end_value, 0.5f);

        cam.DOFieldOfView(end_value, 0.15f);
    }

    // Gestion du cooldown du dash et du jetpack
    private void MovementCooldown()
    {
        if (start)
        {
            if (restart)
            {
                restart = false;
                CooldownRemainingTime = 0;
                newDashUsed = false;
                newJetpackUsed = false;
                //Debug.Log("Restart Countdown");
            }
            if (CooldownRemainingTime >= energyRegenCooldown)
            {
                jetpackCooldown = false;
                dashUsed = false;
                start = false;
            }
            else if (CooldownRemainingTime < energyRegenCooldown)
            {
                CooldownRemainingTime += Time.deltaTime;
                //Debug.Log("Countdown Start");
            }
        }
        else if (!start)
        {
            CooldownRemainingTime = 0;
            start = false;
        }
    }

    // Coroutine pour la regen d'energie
    private IEnumerator RegenEnergy()
    {
        while (true)
        {
            // Si l'energie n'est pas a son maximum et que le jetpack n'est pas utilise ou que l'energie est inferieure a la limite minimale pour le jetpack, et que le cooldown du jetpack est termine
            if (energyAmount < maxEnergyAmount && ((!jetpack || energyAmount < jetpackMinEnergy * 2) && !jetpackCooldown && !newJetpackUsed) && !dashUsed)
            {
                float regenAmount = energyRegenCurve.Evaluate(energyAmount / maxEnergyAmount);
                energyAmount += regenAmount;
                if (energyAmount > maxEnergyAmount)
                {
                    energyAmount = maxEnergyAmount;
                }
            }
            else if (dashUsed && !jetpackCooldown)
            {
                start = true;
                if (newDashUsed || jetpackCooldown)
                {
                    jetpackCooldown = false;
                    restart = true;
                }
            }
            else if (jetpackCooldown)
            {
                start = true;
                if (newJetpackUsed || dashUsed)
                {
                    dashUsed = false;
                    restart = true;
                }
            }

            yield return new WaitForSeconds(energyRegenTime);
        }
    }

    // Recupere l'id
    public int GetID()
    {
        return id;
    }

    // Recupere le slider de l'energie
    private void RetrieveEnergySlider()
    {
        Transform handleFile = transform.Find("Canvas/Slider/Handle Slide Area/Handle");
        Transform handleTransformFile = transform.Find("Canvas/Slider/Handle Slide Area");
        if (handleTransformFile != null)
        {
            handleTransform = handleTransformFile.GetComponent<RectTransform>();
        }
        else if (handleTransform == null)
        {
            Debug.LogError("Handle Transform not found");
        }
        if (handleFile != null)
        {
            //Debug.Log("Handle image found");
            handleImage = handleFile.GetComponent<Image>();
        }
        else if (handleImage == null)
        {
            Debug.LogError("Handle image not found");
        }
    }

    // Met a jour le slider de l'energie
    private void EnergySliderUpdate()
    {
        if (handleTransform != null && handleImage != null)
        {
            float handlePosition = (energyAmount / maxEnergyAmount) * handleTransform.rect.height;
            handleImage.rectTransform.anchoredPosition = new Vector2(handleImage.rectTransform.anchoredPosition.x, handleTransform.anchoredPosition.y - handlePosition);
            //Debug.Log("Handle position : " + handlePosition);
        }
    }

    // Gere les inputs du joueur
    private void InputManager()
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
    }

    // Gestion du saut
    private void JumpManager()
    {
        if (jump && jumpAllow)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    // Gestion du grappin
    private void GrappleManager()
    {
        if (grappleHook)
        {
            // si le joueur est en train de grapple
            if (isGrappling)
            {
                isGrappling = false;
            }
            // Verifie si la balle est a portee
            else if (Vector3.Distance(transform.position, targetBall.transform.position) >= grappleMinRange)
            {
                // Grapple
                isGrappling = true;
                initialRotation = transform.rotation;
            }
        }

        if (isGrappling)
        {
            // Fait pivoter le joueur vers la balle de maniere douce
            Quaternion targetRotation = Quaternion.LookRotation(targetBall.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * grappleSpeed * 0.1f); // r�duit la vitesse de rotation du joueur

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

            // Retablit la rotation initiale du joueur en x et z
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, currentRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            grappleObject.SetActive(false);
        }
    }

    // Gestion lors du grappin
    private void GrapplingManager()
    {
        if (isGrappling)
        {
            float step = grappleSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetBall.transform.position, step);
        }
    }

    // Gestion du deplacement
    private void MovementManager()
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
        // Rotation du joueur
        transform.Rotate(transform.up, angularSpeed * hor);
    }

}
