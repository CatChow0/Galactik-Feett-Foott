using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private int scorePlayer1, scorePlayer2;
    private Transform ballStartPos;
    private Transform player1StartPos;
    private Transform player2StartPos;
    private bool allowGoal;

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
        SpawnPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Met les joueurs et la balle à leur position de départ
    void SpawnPosition()
    {

        allowGoal = true;

        // Récupère la balle et la place à sa position de départ
        ballStartPos = GameObject.Find("BallStartPos").transform;
        Ball ball = FindObjectOfType<Ball>();
        ball.transform.position = ballStartPos.position;
        ball.transform.rotation = ballStartPos.rotation;

        // Récupère le joueur 1 et le place à sa position de départ
        player1StartPos = GameObject.Find("Player1StartPos").transform;
        PlayerBehaviour1 player1 = FindObjectOfType<PlayerBehaviour1>();
        player1.transform.position = player1StartPos.position;
        player1.transform.rotation = player1StartPos.rotation;

        // Récupère le joueur 2 et le place à sa position de départ
        player2StartPos = GameObject.Find("Player2StartPos").transform;
        PlayerBehaviour1 player2 = FindObjectOfType<PlayerBehaviour1>();
        player2.transform.position = player2StartPos.position;
        player2.transform.rotation = player2StartPos.rotation;
    }

    public void GameRoutine(bool restart)
    {
        if (restart)
        {
            SpawnPosition();
        }
    }


    // Update le score en cas de but
    public void ScorePoint(int playerID)
    {
        Debug.Log("Goal scored by player " + playerID);
        if (playerID == 1 && allowGoal)
        {
            scorePlayer2++;
            Debug.Log("Player 2 scored : Player 1 | " + scorePlayer1 + " - " + scorePlayer2 + " | Player 2");

            allowGoal = false;
            GlobalUi.UpdateScore(playerID, scorePlayer2);

            StartCoroutine(waitGameRoutine());
        }
        else if (playerID == 2 && allowGoal)
        {
            scorePlayer1++;
            Debug.Log("Player 1 scored : Player 1 | " + scorePlayer1 + " - " + scorePlayer2 + " | Player 2");

            allowGoal = false;
            GlobalUi.UpdateScore(playerID, scorePlayer1);

            StartCoroutine(waitGameRoutine());
        }
    }

    IEnumerator waitGameRoutine()
    {
        yield return new WaitForSecondsRealtime(5);
        
        GameRoutine(true);
    }
}
