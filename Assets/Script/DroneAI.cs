using System.Collections;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

    [Header("Drone Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Ball ball; // La balle que le drone va chercher
    [SerializeField] private float actionIntervalMin = 10f;
    [SerializeField] private float actionIntervalMax = 21f;
    [SerializeField] private LayerMask wallMask;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Fetch Ball Settings")]
    [SerializeField] private float throwForce = 10f;

    private Vector3 targetPoint;
    private Rigidbody rb;
    private bool isFetchingBall;


    // ------------------------------------------------------- //
    // ----------------- FONCTION PRINCIPALE ----------------- //
    // ------------------------------------------------------- //

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        GenerateTargetPoint(); // Génère un premier point cible

        FetchBall(); // Le drone va chercher la balle au début du match

        StartCoroutine(PerformAction());
    }

    private void Update()
    {
        if (!isFetchingBall)
        {
            MoveDrone();
        }
    }

    // ------------------------------------------------------ //
    // ----------------- MOUVEMENT DU DRONE ----------------- //
    // ------------------------------------------------------ //

    private void MoveDrone()
    {
        // Si le drone est proche du point cible ou si un mur est
        // détecté devant lui, génère un nouveau point cible

        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            GenerateTargetPoint();
        }

        if (Physics.Raycast(transform.position, targetPoint - transform.position, Vector3.Distance(transform.position, targetPoint), wallMask))
        {
            GenerateTargetPoint();
        }

        // Déplace le drone vers le point cible et
        // s'arrête s'il détecte un mur devant lui

        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Physics.Raycast(transform.position, rb.velocity, speed * Time.deltaTime, wallMask))
        {
            rb.velocity = Vector3.zero;
        }
    }

    // ------------------------------------------------------------- //
    // ----------- GÉNÉRATION D'UN POINT CIBLE ALÉATOIRE ----------- //
    // ------------------------------------------------------------- //

    private void GenerateTargetPoint()
    {
        float x = Random.Range(0, 80);
        float y = transform.position.y; // Garde la même hauteur
        float z = Random.Range(0, 100);

        targetPoint = new Vector3(x, y, z);
    }


    // ------------------------------------------------------------------------- //
    // ------------------ TIR DU PROJECTILE A TETE CHERCHEUSE ------------------ //
    // ------------------------------------------------------------------------- //

    public void ShootProjectile()
    {
        Vector3 direction = (ball.transform.position - transform.position).normalized;                      // Donne une vitesse au projectile dans la direction de la balle

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);     // Instancie un nouveau projectile

        projectile.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);                // Ajuste la rotation du projectile pour que l'axe X pointe vers la balle

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();                                      // Applique la vitesse au projectile
        projectileRb.velocity = direction * projectileSpeed;

        DroneProjectileBehaviour projectileBehaviour = projectile.GetComponent<DroneProjectileBehaviour>(); // Donne la balle au projectile pour qu'il puisse la suivre
        if (projectileBehaviour != null)
        {
            projectileBehaviour.ball = ball;
        }
    }
    
    // -------------------------------------------------------------- //
    // ----------------- RÉCUPÉRATION DE LA BALLE ------------------- //
    // -------------------------------------------------------------- //

    public void FetchBall()
    {
        StartCoroutine(FetchBallCoroutine());
    }

    private IEnumerator FetchBallCoroutine()
    {
        isFetchingBall = true;
        float initialHeight = transform.position.y; // Stocke la hauteur initiale du drone

        
        while (true)                                // Le drone se place au-dessus de la balle mais conserve sa hauteur
        {

            // Stocke la position au -dessus de la balle
            // et la direction vers cette positions

            Vector3 aboveBallPosition = new Vector3(ball.transform.position.x, transform.position.y, ball.transform.position.z);
            Vector3 direction = (aboveBallPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, aboveBallPosition);

            // Adapate la vitesse du drone en
            // fonction de la distance à la balle

            if (distance > 0.1f)
            {
                rb.velocity = direction * speed;                                     // Si le drone est loin de la balle, il se déplace à sa vitesse normale
            }
            else
            {
                float ballSpeed = ball.GetComponent<Rigidbody>().velocity.magnitude; // Si le drone est proche de la balle, il adapte sa vitesse pour correspondre à celle de la balle
                rb.velocity = direction * ballSpeed;                                 
                break;                                                               // Sort de la boucle une fois que le drone est au-dessus de la balle
            }
            yield return null;
        }

        // Le drone descend au-dessus de la balle //

        while (true)
        {
            // Stocke la position de descente et
            // la direction vers cette position

            Vector3 descendPosition = new Vector3(ball.transform.position.x, ball.transform.position.y + 1.5f, ball.transform.position.z);
            Vector3 direction = (descendPosition - transform.position).normalized;

            // Si un mur est détecté devant le drone
            // modifie la direction de mouvement du drone

            if (Physics.Raycast(transform.position, direction, speed * Time.deltaTime, wallMask))
            {
                direction = Vector3.Cross(direction, Vector3.up); // Se déplace parallèlement au mur
            }

            rb.velocity = direction * speed;

            // Si le drone est suffisamment proche
            // de la balle, sort de la boucle

            if (Vector3.Distance(transform.position, descendPosition) < 0.5f)
            {
                break;
            }

            yield return null;
        }

        // Le drone "attache" la balle à lui en la rendant
        // kinematic et en la déplaçant à sa position

        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.transform.position = transform.position + Vector3.down * 1.5f;

        // Le drone retourne au centre de la carte //

        Vector3 centerPosition = new Vector3(40, initialHeight, 50);
        while (Vector3.Distance(transform.position, centerPosition) > 1f)           // Augmente la marge d'erreur à 1f
        {
            Vector3 direction = (centerPosition - transform.position).normalized;
            rb.velocity = direction * speed;

            ball.transform.position = transform.position + Vector3.down * 1.5f;     // Déplace la balle à la position du drone

            yield return null;
        }


        // Le drone lâche la balle et lui donne
        // une force dans une direction aléatoire

        ball.GetComponent<Rigidbody>().isKinematic = false;
        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        ball.GetComponent<Rigidbody>().AddForce(randomDirection * throwForce, ForceMode.Impulse);

        isFetchingBall = false;

    }

    // -------------------------------------------------------------- //
    // ----------------- ACTIONS ALEATOIRES DU DRONE ---------------- //
    // -------------------------------------------------------------- //
    private IEnumerator PerformAction()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(actionIntervalMin, actionIntervalMax));

            // Choisit aléatoirement entre tirer un 
            // projectile et aller chercher la balle

            if (Random.value < 0.5f)
            {
                ShootProjectile();
            }
            else
            {
                FetchBall();
            }
        }
    }
}
