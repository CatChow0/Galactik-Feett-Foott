using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // Angle d'Euler pour ajuster la rotation de la fl�che
    [SerializeField]
    private Vector3 eulerAngleOffset;

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;

            // Ignorez la rotation autour de l'axe Y
            direction.y = 0f;

            Quaternion rotation = Quaternion.LookRotation(direction);

            // Appliquer l'offset d'Euler � la rotation
            rotation *= Quaternion.Euler(eulerAngleOffset);

            transform.rotation = rotation;
        }
    }
}