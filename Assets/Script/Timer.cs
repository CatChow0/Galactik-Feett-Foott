using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    [SerializeField] public float timeRemaining;
    private float defaultTime;


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
        defaultTime = instance.timeRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        CountdownTimer();
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
}
