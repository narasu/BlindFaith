using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private static Puzzle instance;
    public static Puzzle Instance
    {
        get
        {
            return instance;
        }
    }

    private AudioSource audioSource;

    public List<AudioClip> puzzleClips; // puzzle audio fragments
    public List<string> puzzleAnswers; // expected answers for each puzzle

    private void Awake()
    {
        instance = this;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    //start the introduction to specified puzzle
    public void StartPuzzle(int _currentPuzzle)
    {
        audioSource.clip = puzzleClips[_currentPuzzle];
        audioSource.Play();
    }

    public bool CheckAnswer(int _currentPuzzle, string _answer)
    {
        if (_answer == puzzleAnswers[_currentPuzzle])
            return true;
        else
            return false;
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Unpause()
    {
        audioSource.UnPause();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }
}
