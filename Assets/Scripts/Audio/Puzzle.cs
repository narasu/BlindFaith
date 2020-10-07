using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private AudioSource audioSource;

    public Dictionary<int, AudioClip> puzzleClips; // puzzle audio fragments
    public Dictionary<int, string> puzzleAnswers; // expected answers for each puzzle

    private void Awake()
    {
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

    public void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }
}
