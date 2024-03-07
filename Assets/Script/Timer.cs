using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    private List<float> timerList = new List<float> { 60, 120, 180, 240, 300, 600, 900, 1800, 2700, 3600 };

    [SerializeField] public float timeRemaining;
    private float defaultTime;
    private int listIndex;

    public TextMeshProUGUI OptionsTimeText;


    public static Timer GetInstance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            return instance = FindObjectOfType<Timer>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //defaultTime = instance.timeRemaining;
        LoadTimeRemaining();
    }

    // Update is called once per frame
    void Update()
    {
        // si la scène actuelle est la scène de jeu
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            CountdownTimer();
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SaveTimeRemaining();
            LoadTimeRemaining();
            FormatTime();
        }
    }

    public void ResetTimer()
    {
        timeRemaining = defaultTime;
    }

    public void CountdownTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if (timeRemaining < 0)
        {
            timeRemaining = 0;
        }
    }

    public void SetPreviousTime()
    {
        if (listIndex == 0)
        {
            listIndex = timerList.Count - 1;
            timeRemaining = timerList[listIndex];
        }
        else if(listIndex > 0)
        {
            listIndex--;
            timeRemaining = timerList[listIndex];
        }
    }

    public void SetNextTime()
    {
        if (listIndex < (timerList.Count - 1))
        {
            listIndex++;
            timeRemaining = timerList[listIndex];
        }
        else if (listIndex == (timerList.Count - 1))
        {
            listIndex = 0;
            timeRemaining = timerList[listIndex];
        }
    }

    public void SaveTimeRemaining()
    {
        PlayerPrefs.SetInt("ListIndex", listIndex);
        PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
        PlayerPrefs.Save();
    }

    public void LoadTimeRemaining()
    {
        if (PlayerPrefs.HasKey("ListIndex"))
        {
            listIndex = PlayerPrefs.GetInt("ListIndex");
        }
        else
        {
            listIndex = 0;
            //Debug.LogError("No ListIndex has been loaded");
        }
        if (PlayerPrefs.HasKey("TimeRemaining"))
        {
            timeRemaining = PlayerPrefs.GetFloat("TimeRemaining");
        }
        else
        {
            timeRemaining = timerList[listIndex];
            //Debug.LogError("No time remaining has been loaded");
        }
    }

    public void FormatTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);

        string format = string.Format("{0:0}:{1:00}", minutes, seconds);

        OptionsTimeText.text = format;
    }
}
