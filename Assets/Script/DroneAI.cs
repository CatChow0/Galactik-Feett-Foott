using System.Collections;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [Header("Drone Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Ball ball; // La balle que le drone va chercher
    [SerializeField] private float actionIntervalMin = 10f; // Intervalle minimum entre les actions
    [SerializeField] private float actionIntervalMax = 21f; // Intervalle maximum entre les actions
    [SerializeField] private LayerMask wallMask; // LayerMask pour identifier les murs

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 10f; // Vitesse du projectile
    [SerializeField] private GameObject projectilePrefab; // Préfab du projectile


    private Vector3 targetPoint; // Le point cible actuel du drone
    private Rigidbody rb;
    private Vector3 initialPosition; // La position initiale du drone
    private bool isFetchingBall = false; // Si le drone est en train de récupérer la balle

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GenerateTargetPoint(); // Génère un premier point cible
        StartCoroutine(PerformAction());
    }

    private void Update()
    {
        if (!isFetchingBall)
        {
            MoveDrone();
        }
    }

    private void MoveDrone()
    {
        // Si le drone est proche du point cible, génère un nouveau point cible
        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            GenerateTargetPoint();
        }

        // Si le chemin vers le point cible est bloqué par un mur, génère un nouveau point cible
        if (Physics.Raycast(transform.position, targetPoint - transform.position, Vector3.Distance(transform.position, targetPoint), wallMask))
        {
            GenerateTargetPoint();
        }

        // Déplace le drone vers le point cible à une vitesse constante
        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.velocity = direction * speed;

        // Si le drone est sur le point de traverser un mur, arrête le drone
        if (Physics.Raycast(transform.position, rb.velocity, speed * Time.deltaTime, wallMask))
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void GenerateTargetPoint()
    {
        // Génère un point cible aléatoire dans la zone de déplacement du drone
        float x = Random.Range(0, 80);
        float y = transform.position.y; // Garde la même hauteur
        float z = Random.Range(0, 100);

        targetPoint = new Vector3(x, y, z);
    }

    public void ShootProjectile()
    {
        // Donne une vitesse au projectile dans la direction de la balle
        Vector3 direction = (ball.transform.position - transform.position).normalized;

        // Instancie un nouveau projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Ajuste la rotation du projectile pour que l'axe X pointe vers la balle
        projectile.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        // Applique la vitesse au projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = direction * projectileSpeed;

        DroneProjectileBehaviour projectileBehaviour = projectile.GetComponent<DroneProjectileBehaviour>();
        if (projectileBehaviour != null)
        {
            projectileBehaviour.ball = ball;
        }
    }


    private void FetchBall()
    {
        StartCoroutine(FetchBallCoroutine());
    }

    private IEnumerator FetchBallCoroutine()
    {
        isFetchingBall = true;

        // Déplace le drone au-dessus de la balle
        Vector3 aboveBallPosition = new Vector3(ball.transform.position.x, ball.transform.position.y + 1, ball.transform.position.z);
        while (Vector3.Distance(transform.position, aboveBallPosition) > 0.1f)
        {
            Vector3 direction = (aboveBallPosition - transform.position).normalized;
            rb.velocity = direction * speed;
            yield return null;
        }

        // Ajoute un FixedJoint pour attacher la balle au drone
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = ball.GetComponent<Rigidbody>();

        // Fait remonter le drone à sa hauteur initiale
        Vector3 initialHeightPosition = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
        while (Vector3.Distance(transform.position, initialHeightPosition) > 0.1f)
        {
            Vector3 direction = (initialHeightPosition - transform.position).normalized;
            rb.velocity = direction * speed;
            yield return null;
        }

        // Déplace le drone au centre de la carte
        Vector3 centerPosition = new Vector3(40, initialPosition.y, 50); // Assumant que le centre de la carte est à (40, 50)
        while (Vector3.Distance(transform.position, centerPosition) > 0.1f)
        {
            Vector3 direction = (centerPosition - transform.position).normalized;
            rb.velocity = direction * speed;
            yield return null;
        }

        // Relâche la balle avec une force dans une direction aléatoire
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        ball.GetComponent<Rigidbody>().AddForce(randomDirection * 10, ForceMode.Impulse);

        // Supprime le FixedJoint pour détacher la balle du drone
        Destroy(joint);

        isFetchingBall = false;
    }

    private IEnumerator PerformAction()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(actionIntervalMin, actionIntervalMax));

            // Choisit aléatoirement entre tirer un projectile et aller chercher la balle
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
