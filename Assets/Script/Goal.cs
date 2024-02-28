using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Goal Settings")]
    [SerializeField] private int playerID; // 1 or 2

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if the ball has entered the goal
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (playerID == 1)
            {
                Map.AddPoint(playerID);
            }
            else if (playerID == 2)
            {
                Map.AddPoint(playerID);
            }
        }
    }
    
}
