using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int scorePlayer1, scorePlayer2;
    private Transform ballStartPos;
    private Transform player1StartPos;
    private Transform player2StartPos;
    private bool allowGoal;
    private bool restart = false;
    private Timer timerInstance;

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
        timerInstance = Timer.GetInstance();
        SpawnPosition();
    }

    // Update is called once per frame
    void Update()
    {
        EndGame();
    }

    // Met les joueurs et la balle à leur position de départ
    void SpawnPosition()
    {

        allowGoal = true;

        // Récupère la balle et la place à sa position de départ
        ballStartPos = GameObject.Find("BallStartPos").transform;
        GameObject ball = GameObject.Find("Ball");
        ball.transform.position = ballStartPos.position;
        ball.transform.rotation = ballStartPos.rotation;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Récupère le joueur 1 et le place à sa position de départ
        player1StartPos = GameObject.Find("Player1StartPos").transform;
        GameObject player1 = GameObject.Find("Player1");
        player1.transform.position = player1StartPos.position;
        player1.transform.rotation = player1StartPos.rotation;
        player1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Récupère le joueur 2 et le place à sa position de départ
        player2StartPos = GameObject.Find("Player2StartPos").transform;
        GameObject player2 = GameObject.Find("Player2");
        player2.transform.position = player2StartPos.position;
        player2.transform.rotation = player2StartPos.rotation;
        player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void GameRoutine(bool respawn)
    {
        if (respawn && !restart)
        {
            SpawnPosition();
        }
        else if (respawn && restart)
        {
            SpawnPosition();
            scorePlayer1 = 0;
            scorePlayer2 = 0;
            restart = false;
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

            StartCoroutine(WaitGameRoutine());
        }
        else if (playerID == 2 && allowGoal)
        {
            scorePlayer1++;
            Debug.Log("Player 1 scored : Player 1 | " + scorePlayer1 + " - " + scorePlayer2 + " | Player 2");

            allowGoal = false;
            GlobalUi.UpdateScore(playerID, scorePlayer1);

            StartCoroutine(WaitGameRoutine());
        }
    }

    IEnumerator WaitGameRoutine()
    {
        yield return new WaitForSecondsRealtime(5);
        
        GameRoutine(true);
        timerInstance.ResetTimer();
    }

    public void EndGame()
    {
        if (timerInstance.timeRemaining <= 0)
        {
            Debug.Log("Time's up");

            if (scorePlayer1 > scorePlayer2)
            {
                Debug.Log("Player 1 wins");
            }
            else if (scorePlayer1 < scorePlayer2)
            {
                Debug.Log("Player 2 wins");
            }
            else
            {
                Debug.Log("Draw");
            }

            restart = true;
            StartCoroutine(WaitGameRoutine());
        }
    }
}
