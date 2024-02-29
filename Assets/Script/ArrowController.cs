using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform ball; // R�f�rence � la balle

    void Update()
    {
        // V�rifie si la balle est d�finie
        if (ball != null)
        {
            // Oriente la fl�che directement vers la balle
            transform.LookAt(ball);
        }
    }
}
