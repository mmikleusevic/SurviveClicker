using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;    
    }
}
