using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform ball; // Référence à la balle

    void Update()
    {
        // Vérifie si la balle est définie
        if (ball != null)
        {
            // Oriente la flèche directement vers la balle
            transform.LookAt(ball);
        }
    }
}
