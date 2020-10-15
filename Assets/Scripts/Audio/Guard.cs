using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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


    #region Audio
    private AudioSource audioSource;
    public AudioClip congrats;
    public AudioClip[] startAudio; //audio for beginning of the game
    public AudioClip[] puzzleIntros;

    public AudioClip understand; // "Do you understand?"
    public AudioClip repeat; // "I repeat: "
    public AudioClip didntCatch; // "I didn't catch that "
    public AudioClip goodFeedback;

    public AudioClip wrong; 
    public AudioClip correct;
    #endregion
    
    internal bool introFinished;
    bool continueLoop = false;
    public bool readyToListen;
    public bool isListening;
    public float timeOutLimit = 15.0f;
    int currentSegment = 0;

    internal UnityEvent OnDidntCatch;
    internal UnityEvent OnTimeout;

    private void Awake()
    {
        instance = this;

        
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        OnDidntCatch = new UnityEvent();
        OnTimeout = new UnityEvent();

        OnDidntCatch.AddListener(() => StartCoroutine(React(2)));
        OnTimeout.AddListener(() => continueLoop = true);

        audioSource.clip = congrats;
        audioSource.Play();

        StartCoroutine(IntroduceGame());
    }



    public IEnumerator IntroduceGame()
    {
        yield return new WaitForSeconds(congrats.length + 0.5f);

        while (currentSegment < startAudio.Length)
        {
            readyToListen = false;
            continueLoop = false;
            Debug.Log("Introduction segment " + currentSegment);
            //play instruction segment
            audioSource.clip = startAudio[currentSegment];
            audioSource.Play();
            yield return new WaitForSeconds(startAudio[currentSegment].length);

            //Do you understand?
            Debug.Log("Do you understand?");
            audioSource.clip = understand;
            audioSource.Play();
            readyToListen = true;

            GameManager.Instance.checkingTimeOut = true;
            GameManager.Instance.timeOutTimer = 0f;

            yield return new WaitUntil(() => continueLoop == true);
        }

        GameManager.Instance.OnInstructionFinished.Invoke();
    }

    

    void GotoNextSegment()
    {
        currentSegment++;
        continueLoop = true;
    }

    //react to user speech
    public void ProcessSpeech(string _speech)
    {
        StopTalking();

        if (_speech == "i understand" || _speech == "yes")
        {
            StartCoroutine(React(0));
        }
        else if (_speech == "i don't understand" || _speech == "no")
        {
            StartCoroutine(React(1));
        }
        else
        {
            StartCoroutine(React(2));
        }
    }

    IEnumerator React(int i)
    {
        switch (i)
        {
            case 0:
                audioSource.PlayOneShot(goodFeedback);
                yield return new WaitForSeconds(goodFeedback.length + 0.6f);
                Debug.Log("Good");
                GotoNextSegment();
                yield break;
            case 1:
                Debug.Log("I repeat");
                audioSource.PlayOneShot(repeat);
                yield return new WaitForSeconds(repeat.length + 0.4f);
                continueLoop = true;
                yield break;
            case 2:
                Debug.Log("I didn't catch that");
                audioSource.PlayOneShot(didntCatch);
                yield return new WaitForSeconds(didntCatch.length);
                yield break;
        }
    }


    //start the introduction to specified puzzle
    public void IntroducePuzzle(int _currentPuzzle)
    {
        audioSource.clip = puzzleIntros[_currentPuzzle];
        Debug.Log("introducing puzzle " + _currentPuzzle);
        audioSource.Play();
    }
    //correct or incorrect
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

    private void OnDestroy()
    {
        OnDidntCatch.RemoveAllListeners();
        OnTimeout.RemoveAllListeners();
    }
    private void OnApplicationQuit()
    {
        OnDidntCatch.RemoveAllListeners();
        OnTimeout.RemoveAllListeners();
    }
}
