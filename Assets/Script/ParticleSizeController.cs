using UnityEngine;

public class ParticleSizeController : MonoBehaviour
{
    public Rigidbody ballRigidbody; // Référence au Rigidbody de la balle
    public float minSize = 1f; // Taille minimale des particules
    public float maxSize = 5f; // Taille maximale des particules
    public float minSpeed = 0f; // Vitesse minimale de la balle
    public float maxSpeed = 10f; // Vitesse maximale de la balle

    private ParticleSystem particleSystem;

    void Start()
    {
        // Récupère la référence au composant ParticleSystem
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Vérifie que la référence au Rigidbody de la balle est définie
        if (ballRigidbody != null)
        {
            // Calcule une valeur normalisée de la vitesse de la balle entre 0 et 1
            float speedNormalized = Mathf.InverseLerp(minSpeed, maxSpeed, ballRigidbody.velocity.magnitude);

            // Utilise cette valeur pour définir la taille des particules entre minSize et maxSize
            var mainModule = particleSystem.main;
            mainModule.startSize = Mathf.Lerp(minSize, maxSize, speedNormalized);
        }
    }
}
