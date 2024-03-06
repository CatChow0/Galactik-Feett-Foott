using TMPro;
using UnityEngine;


public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    public TextMeshProUGUI timeText;
    private GameManager gameManagerEntity;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerEntity = GameManager.GetInstance();
        timer = Timer.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText1.text = gameManagerEntity.scorePlayer1.ToString();
        scoreText2.text = gameManagerEntity.scorePlayer2.ToString();

        int minutes = Mathf.FloorToInt(timer.timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timer.timeRemaining - minutes * 60);

        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeText.text = niceTime;
    }
}
