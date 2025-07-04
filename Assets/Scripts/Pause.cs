using System;
using TMPro;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    
    private bool isPaused;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }

            pausePanel.SetActive(isPaused);
        }
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.StopGame();
        
        ResumeGame();
        
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
}
