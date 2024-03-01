using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform ball; // R�f�rence � la balle
    public GameObject arrow; // R�f�rence au GameObject de la fl�che

    void Update()
    {
        // V�rifie si la balle est d�finie
        if (ball != null)
        {
            // Calcule la direction de la balle par rapport � la position de la fl�che
            Vector3 direction = ball.position - transform.position;

            // Calcule l'angle d'Euler autour de l'axe Y local de la fl�che
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Cr�e une nouvelle rotation uniquement autour de l'axe Y
            Quaternion rotation = Quaternion.Euler(0f, -angle, 0f);

            // Applique la rotation au GameObject de la fl�che
            arrow.transform.localRotation = rotation;
        }
    }
}
