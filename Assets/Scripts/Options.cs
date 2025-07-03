using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;

    public void SetVolume()
    {
        volumeText.text = $"Volume: {Math.Round(volumeSlider.value, 0)} %";
        GameManager.Instance.SetVolume(volumeSlider.value);
    }
    
    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);    
    }
}
