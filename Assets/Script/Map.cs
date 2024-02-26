using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
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
}
