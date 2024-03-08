using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

    public static GameManager instance;

    public int scorePlayer1, scorePlayer2;
    private Transform ballStartPos;
    private Transform player1StartPos;
    private Transform player2StartPos;
    private bool allowGoal;
    private Timer timerInstance;

    public GameObject EndGameMenu;
    public GameObject scoredUi;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoredText;

    // ---------------------------------------------- //
    // --------- RECUPERATION DE L'INSTANCE --------- //
    // ---------------------------------------------- //

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

    // -------------------------------------------------------- //
    // ----------------- FONCTIONS PRINCIPALE ----------------- //
    // -------------------------------------------------------- //
    void Start()
    {
        timerInstance = Timer.GetInstance();
        SpawnPosition();
    }

    void Update()
    {
        EndGame();
    }

    // ----------------------------------------------------- //
    // -------------- POSITION DE DEPART DU JEU ------------ //
    // ----------------------------------------------------- //
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
        player1.GetComponent<PlayerBehaviour>().energyAmount = player1.GetComponent<PlayerBehaviour>().defaultEnergy;

        // Récupère le joueur 2 et le place à sa position de départ
        player2StartPos = GameObject.Find("Player2StartPos").transform;
        GameObject player2 = GameObject.Find("Player2");
        player2.transform.position = player2StartPos.position;
        player2.transform.rotation = player2StartPos.rotation;
        player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        player2.GetComponent<PlayerBehaviour>().energyAmount = player2.GetComponent<PlayerBehaviour>().defaultEnergy;
    }

    // -------------------------------------------- //
    // -------------- RESTART DU TIMER ------------ //
    // -------------------------------------------- //
    public void RestartGame()
    {
        timerInstance.ResetTimer();
    }


    // ------------------------------------------ //
    // -------------- POINT MARQUE -------------- //
    // ------------------------------------------ //
    public void ScorePoint(int playerID)
    {
        if (playerID == 1 && allowGoal)
        {
            scorePlayer2++;
            allowGoal = false;
            scoredText.text = "Player 2 scored";
            scoredUi.SetActive(true);

            StartCoroutine(WaitGameRoutine());
        }
        else if (playerID == 2 && allowGoal)
        {
            scorePlayer1++;
            allowGoal = false;
            scoredText.text = "Player 1 scored";
            scoredUi.SetActive(true);

            StartCoroutine(WaitGameRoutine());
        }
    }

    // -------------------------------------------- //
    // -------------- RESET DU SCORE -------------- //
    // -------------------------------------------- //
    public void ResetScore()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
    }

    IEnumerator WaitGameRoutine()
    {
        yield return new WaitForSecondsRealtime(5);
        scoredUi.SetActive(false);

        SpawnPosition();
    }
    // -------------------------------------------- //
    // -------------- RESTART DU JEU -------------- //
    // -------------------------------------------- //
    public void GameRestart()
    {
        SpawnPosition();
        ResetScore();
        RestartGame();
        Time.timeScale = 1;
    }

    // ---------------------------------------- //
    // -------------- FIN DU JEU -------------- //
    // ---------------------------------------- //
    public void EndGame()
    {
        if (timerInstance.timeRemaining <= 0)
        {
            Time.timeScale = 0;
            EndGameMenu.SetActive(true);

            if (scorePlayer1 > scorePlayer2)
            {
                winnerText.text = "Player 1 wins";
            }
            else if (scorePlayer1 < scorePlayer2)
            {
                winnerText.text = "Player 2 wins";
            }
            else
            {
                winnerText.text = "Draw";
            }
            scoreText.text = scorePlayer1 + " - " + scorePlayer2;
        }
    }
}
