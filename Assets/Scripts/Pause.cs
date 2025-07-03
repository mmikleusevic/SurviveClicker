using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = isPaused ? 1 : 0;

            isPaused = !isPaused;
        }
    }
}
