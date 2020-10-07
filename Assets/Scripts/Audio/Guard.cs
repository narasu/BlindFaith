using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private static Guard instance;
    public static Guard Instance
    {
        get
        {
            return instance;
        }
    }
    private AudioSource audioSource;
    
    public AudioClip startAudio; //audio for beginning of the game
    internal bool introFinished;

    public AudioClip repeat; // "I repeat: "
    public AudioClip wrong; 
    public AudioClip correct;
    public List<AudioClip> puzzleIntros; // list of introduction clips for each puzzle

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = startAudio;

        audioSource.Play();
    }

    //start the introduction to specified puzzle
    public void StartIntro(int _currentPuzzle)
    {
        audioSource.clip = puzzleIntros[_currentPuzzle];
        audioSource.Play();
    }

    public void Repeat()
    {
        StopTalking(); // just in case
        audioSource.PlayOneShot(repeat);
        audioSource.PlayDelayed(repeat.length + 0.5f);
    }

    public void EvaluateAnswer(bool _correct)
    {
        if (_correct)
        {
            audioSource.clip = correct;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = wrong;
            audioSource.Play();
        }
    }

    public float GetClipLength()
    {
        return audioSource.clip.length;
    }

    public void StopTalking()
    {
        audioSource.Stop();
    }
}
