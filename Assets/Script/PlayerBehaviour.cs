using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PlayerBehaviour : MonoBehaviour
{

    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

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
    bool slow, jump, jetpack, jumpAllow, dash, grappleHook, menu;
    private bool jetpackUsed = false;
    private bool newJetpackUsed = false;
    private bool jetpackCooldown = false;
    private bool grappleUsed = false;
    private bool dashUsed = false;
    private bool newDashUsed = false;
    private bool start = false;
    private bool restart = false;
    public float defaultEnergy;

    Image handleImage = null;
    Image handleFill = null;
    RectTransform handleTransform = null;
    RectTransform handleFillTransform = null;

    public GameObject mainMenu;
    public bool isPaused = false;

    // ------------------------------------------------------- //
    // ----------------- FONCTION PRINCIPALE ----------------- //
    // ------------------------------------------------------- //

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(RegenEnergy());
        RetrieveEnergySlider();
        defaultEnergy = energyAmount;
    }

    void Update()
    {
        InputManager();

        JumpManager();

        JetpackManager();

        DashManager();

        MovementCooldown();

        GrappleManager();

        EnergySliderUpdate();

        OpenMenu();

    }

    private void FixedUpdate()
    {
        MovementManager();

        GrapplingManager();
    }

    // ------------------------------------------------------ //
    // ---------- GESTION DES COLLISIONS DU JOUEUR ---------- //
    // ------------------------------------------------------ //

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

            // Ajoute une force a la balle dans la direction du mouvement du joueur
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

    // ------------------------------------------------------ //
    // --------------- GESTION DU JETPACK  ------------------ //
    // ------------------------------------------------------ //
    private void JetpackManager()
    {

        // Verifie si le joueur a assez d'energie pour utiliser le jetpack //

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
            rb.AddForce(transform.up * jetpackForce * 100 * Time.deltaTime);
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

    // --------------------------------------------------- //
    // --------------- GESTION DU DASH  ------------------ //
    // --------------------------------------------------- //
    private void DashManager()
    {

        // Verifie si le joueur a assez d'energie pour dash //

        if (dash && energyAmount >= dashEnergy)
        {
            if (dashUsed)
            {
                newDashUsed = true;
            }
            else if (!dashUsed)
            {
                dashUsed = true;
            }

            Vector3 dashDirection = transform.forward;                                           // Direction du dash
            rb.AddForce(dashDirection * dashForce*100 * Time.deltaTime, ForceMode.Impulse);      // Ajoute une force au joueur dans la direction du dash
            StartCoroutine(DashTime());                                                          // Lance le Cooldown du dash
            energyAmount -= dashEnergy;                                                          // Retire l'energie utilisee pour le dash
            DoFov(dashFov);                                                                      // Change la fov de la camera
        }
    }

    // -------------------------------------------------- //
    // ------------ DUREE DU DASH ET FOV  --------------- //
    // -------------------------------------------------- //
    private IEnumerator DashTime()
    {
        // Attend le temps de dash et remet la vitesse du joueur a la normale
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.8f);
        DoFov(fov);
    }

    private void DoFov(float end_value)
    {

        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, end_value, 0.5f); // Change la fov de la camera SANS DoTween

        cam.DOFieldOfView(end_value, 0.15f);                              // Change la fov de la camera AVEC DoTween 
    }

    // ------------------------------------------------------ //
    // ------------ COOLDOWN DU DASH ET JETPACK  ------------ //
    // ------------------------------------------------------ //
    private void MovementCooldown()
    {
        if (start)
        {
            if (restart)                                            // Si le joueur a utilise un autre mouvement pendant le cooldown
            {
                restart = false;
                CooldownRemainingTime = 0;
                newDashUsed = false;
                newJetpackUsed = false;
                grappleUsed = false;
            }
            if (CooldownRemainingTime >= energyRegenCooldown)       // Si le cooldown est termine
            {
                jetpackCooldown = false;
                dashUsed = false;
                grappleUsed = false;
                start = false;
            }
            else if (CooldownRemainingTime < energyRegenCooldown)   // Si le cooldown n'est pas termine
            {
                CooldownRemainingTime += Time.deltaTime;
            }
        }
        else if (!start)                                           // Si le joueur n'a pas utilise de mouvement pendant le cooldown
        {
            CooldownRemainingTime = 0;
            start = false;
        }
    }

    // ------------------------------------------------------ //
    // ------------ REGENERATION DE L'ENERGIE  -------------- //
    // ------------------------------------------------------ //
    private IEnumerator RegenEnergy()
    {
        while (true)
        {
            // Si l'energie n'est pas a son maximum et que le jetpack 
            // n'est pas utilise ou que l'energie est inferieure a la
            // limite minimale pour le jetpack, et que le cooldown du
            // jetpack est termine

            if (energyAmount < maxEnergyAmount && ((!jetpack || energyAmount < jetpackMinEnergy * 2) && !jetpackCooldown && !newJetpackUsed) && !dashUsed && !grappleUsed)
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
            else if (grappleUsed)
            {
                start = true;
            }
            yield return new WaitForSeconds(energyRegenTime);
        }
    }

    // ------------------------------------------------------ //
    // ------------ RECUPERATION DU SLIDER D'ENERGIE  ------- //
    // ------------------------------------------------------ //
    private void RetrieveEnergySlider()
    {

        Transform handleFile = transform.Find("Canvas/Slider/Handle Slide Area/Handle");    // Recupere les elements du slider d'energie
        Transform handleTransformFile = transform.Find("Canvas/Slider/Handle Slide Area");
        Transform handleFillTransformFile = transform.Find("Canvas/Slider/Fill Area");
        Transform handleFillFile = transform.Find("Canvas/Slider/Fill Area/Fill");

        if (handleTransformFile != null)
        {
            handleTransform = handleTransformFile.GetComponent<RectTransform>();
        }

        if (handleFile != null)
        {
            handleImage = handleFile.GetComponent<Image>();
        }

        if (handleFillTransformFile != null)
        {
            handleFillTransform = handleFile.GetComponent<RectTransform>();
        }

        if (handleFillTransform != null)
        {
            handleFill = handleFillFile.GetComponent<Image>();
        }

    }

    // ------------------------------------------------------ //
    // ------------ MISE A JOUR DU SLIDER D'ENERGIE  -------- //
    // ------------------------------------------------------ //
    private void EnergySliderUpdate()
    {

        if (handleTransform != null && handleImage != null)
        {
            float handlePosition = (energyAmount / maxEnergyAmount) * handleTransform.rect.height;
            handleImage.rectTransform.anchoredPosition = new Vector2(handleImage.rectTransform.anchoredPosition.x, -handlePosition);

            float fillAmount = handlePosition + 10;
            float fillPosition = handlePosition / 2;
            handleFill.rectTransform.sizeDelta = new Vector2(handleFill.rectTransform.sizeDelta.x, fillAmount);
            handleFill.rectTransform.anchoredPosition = new Vector2(handleFill.rectTransform.anchoredPosition.x, handleTransform.anchoredPosition.y - fillPosition);
        }
    }

    // ------------------------------------------------------ //
    // ------------ RECUPERATION DES INPUTS  ---------------- //
    // ------------------------------------------------------ //
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
            menu = Input.GetButtonDown("Menu");
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
            menu = Input.GetButtonDown("Menu");
        }
    }

    // ----------------------------------------------------- //
    // ------------- GESTION DU SAUT DU JOUEUR ------------- //
    // ----------------------------------------------------- //

    private void JumpManager()
    {
        if (jump && jumpAllow)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    // ------------------------------------------------ //
    // ------------ GESTION DU GRAPPIN  --------------- //
    // ------------------------------------------------ //

    private void GrappleManager()
    {
        if (grappleHook)
        {

            // si le joueur est en train de grapple //
            if (isGrappling)
            {
                isGrappling = false;
            }

            // Verifie si la balle est a portee //
            else if ((Vector3.Distance(transform.position, targetBall.transform.position) >= grappleMinRange) && energyAmount >= maxEnergyAmount)
            {
                isGrappling = true;
                initialRotation = transform.rotation;
                energyAmount -= maxEnergyAmount;
                grappleUsed = true;
            }
        }

        if (isGrappling)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetBall.transform.position - transform.position);            // Fait pivoter le joueur vers la balle de maniere douce
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * grappleSpeed * 0.1f);    // reduit la vitesse de rotation du joueur

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

    // ------------------------------------------------------ //
    // ------------ GESTION DU GRAPPIN DU JOUEUR ------------ //
    // ------------------------------------------------------ //
    private void GrapplingManager()
    {
        if (isGrappling)
        {
            float step = grappleSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetBall.transform.position, step);
        }
    }

    // ------------------------------------------------------- //
    // ------------ GESTION DU MOUVEMENT DU JOUEUR ----------- //
    // ------------------------------------------------------- //
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
        rb.AddForce(transform.forward * currentSpeed*50 * Time.fixedDeltaTime) ;
        // Rotation du joueur
        transform.Rotate(transform.up, angularSpeed*200 * hor * Time.fixedDeltaTime);
    }

    // ------------------------------------------------------ //
    // ------------ OUVERTURE DU MENU DE PAUSE  ------------- //
    // ------------------------------------------------------ //

    private void OpenMenu()
    {
        if (menu && !isPaused)
        {
            mainMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }
}
