using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Goal Settings")]
    [SerializeField] private int playerID; // 1 or 2
    [SerializeField] private Ball ball;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Check if the ball has entered the goal
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            GameManager.GetInstance().ScorePoint(playerID);
        }
    }

}
