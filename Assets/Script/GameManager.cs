using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private float scorePlayer1, scorePlayer2;
    private Transform ballStartPos;
    private Transform player1StartPos;
    private Transform player2StartPos;

    public static GameManager GetInstance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            return instance = FindObjectOfType<GameManager>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Récupère la balle et la place à sa position de départ
        ballStartPos = GameObject.Find("BallStartPos").transform;
        Ball ball = FindObjectOfType<Ball>();
        ball.transform.position = ballStartPos.position;

        // Récupère le joueur 1 et le place à sa position de départ
        player1StartPos = GameObject.Find("Player1StartPos").transform;
        PlayerBehaviour1 player1 = FindObjectOfType<PlayerBehaviour1>();
        player1.transform.position = player1StartPos.position;

        // Récupère le joueur 2 et le place à sa position de départ
        player2StartPos = GameObject.Find("Player2StartPos").transform;
        PlayerBehaviour1 player2 = FindObjectOfType<PlayerBehaviour1>();
        player2.transform.position = player2StartPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScorePoint(int playerID)
    {
        if (playerID == 1)
        {
            scorePlayer2++;
            Debug.Log("Player 2 scored : Player 1 | " + scorePlayer1 + " - " + scorePlayer2 + " | Player 2");
        }
        else if (playerID == 2)
        {
            scorePlayer1++;
            Debug.Log("Player 1 scored : Player 1 | " + scorePlayer1 + " - " + scorePlayer2 + " | Player 2");
        }
    }
}
