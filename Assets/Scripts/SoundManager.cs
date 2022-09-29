using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource scoreAudioSource;
    [SerializeField] private AudioSource gameOverAudioSource;
    [SerializeField] private AudioSource buttonAudioSource;

    public void PlayScoreSound()
    {
        scoreAudioSource.Play();
    }
    public void PlayGameOverSound()
    {
        gameOverAudioSource.Play();
    }
    public void PlayButtonSound()
    {
        buttonAudioSource.Play();
    }
}
