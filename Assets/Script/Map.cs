using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    // --------------------------------------------- //
    // ----------------- VARIABLES ----------------- //
    // --------------------------------------------- //

    private static int player1Score;
    private static int player2Score;

    // --------------------------------------------------------- //
    // ----------------- FUNCTIONS PRINCIPALES ----------------- //
    // --------------------------------------------------------- //
    void Start()
    {

        Screen.fullScreen = true;                                           // Met le jeu en plein écran
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);  // Met la résolution du jeu en 1920x1080
        Cursor.lockState = CursorLockMode.Locked;                           // Immobilise le curseur
        Cursor.visible = false;                                             // Cache le curseur
    }

    // ------------------------------------------------------ //
    // ----------------- COMPTES LES POINTS ----------------- //
    // ------------------------------------------------------ //
    public static void AddPoint(int playerID)
    {
        if (playerID == 1)
        {
            player1Score++;
        }
        else if (playerID == 2)
        {
            player2Score++;
        }
    }
}
