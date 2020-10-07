using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private AudioSource audioSource;

    
    public AudioClip startAudio; //audio for beginning of the game
    public AudioClip repeat; // "I repeat: "
    public AudioClip wrong; 
    public AudioClip correct;
    public List<AudioClip> puzzleIntros; // list of introduction clips for each puzzle

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = startAudio;
        audioSource.playOnAwake = false;
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

    public void StopTalking()
    {
        audioSource.Stop();
    }

    // 
}
