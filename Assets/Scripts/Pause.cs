using System;
using TMPro;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_Text gameOverText;
    
    private bool isPaused;
    private bool isGameOver;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
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

        isGameOver = false;
        
        ResumeGame();
        
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverText.text = String.Empty;
    }

    public void StopGame(string text)
    {
        pausePanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverText.text = text;
        isGameOver = true;
        PauseGame();
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
