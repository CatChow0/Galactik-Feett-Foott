using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform ball; // Référence à la balle
    public GameObject arrow; // Référence au GameObject de la flèche

    void Update()
    {
        // Vérifie si la balle est définie
        if (ball != null)
        {
            // Calcule la direction de la balle par rapport à la position de la flèche
            Vector3 direction = ball.position - transform.position;

            // Calcule l'angle d'Euler autour de l'axe Y local de la flèche
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Crée une nouvelle rotation uniquement autour de l'axe Y
            Quaternion rotation = Quaternion.Euler(0f, -angle, 0f);

            // Applique la rotation au GameObject de la flèche
            arrow.transform.localRotation = rotation;
        }
    }
}
