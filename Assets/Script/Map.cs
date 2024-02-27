using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Player 1 and 2 score
    private static int player1Score;
    private static int player2Score;

    // Start is called before the first frame update
    void Start()
    {

        // Set Window Title and full screen
        Screen.fullScreen = true;
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        //Hide the cursor and lock it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Make the cursor invisible
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Add a point to player who goal
    public static void AddPoint(int playerID)
    {
        if (playerID == 1)
        {
            player1Score++;
            Debug.Log("Player 1 Score: " + player1Score);
        }
        else if (playerID == 2)
        {
            player2Score++;
            Debug.Log("Player 2 Score: " + player2Score);
        }
    }
}
