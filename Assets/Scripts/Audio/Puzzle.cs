﻿using System.Collections;
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

    public AudioClip endOfDemo;

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

    public void PlayDemoEndClip()
    {
        audioSource.loop = false;
        audioSource.clip = endOfDemo;
        audioSource.Play();
    }

    public string GetCorrectPhrase(int _currentPuzzle)
    {
        return puzzleAnswers[_currentPuzzle];
    }
    public void Pause()
    {
        audioSource.Pause();
    }

    public IEnumerator Unpause()
    {
        yield return new WaitForSeconds(2.2f);
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
