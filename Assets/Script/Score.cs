using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    private GameManager gameManagerEntity;
    private int scorePlayer1;
    private int scorePlayer2;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerEntity = GameManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText1.text = gameManagerEntity.scorePlayer1.ToString();
        scoreText2.text = gameManagerEntity.scorePlayer2.ToString();
    }
}
