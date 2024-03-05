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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GenerateTargetPoint(); // Génère un premier point cible
        StartCoroutine(PerformAction());
    }

    private void Update()
    {
        MoveDrone();
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
        // Fait aller chercher la balle par le drone
        // Vous devrez remplir cette méthode avec votre propre logique de récupération de balle
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
