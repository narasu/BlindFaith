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
    public float timeOutLimit = 10.0f;
    int currentSegment = 0;

    internal UnityEvent OnUnderstood;
    internal UnityEvent OnNotUnderstood;
    internal UnityEvent OnDidntCatch;
    internal UnityEvent OnTimeout;

    private void Awake()
    {
        instance = this;

        
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        OnUnderstood = new UnityEvent();
        OnNotUnderstood = new UnityEvent();
        OnDidntCatch = new UnityEvent();
        OnTimeout = new UnityEvent();

        OnUnderstood.AddListener(() => GotoNextSegment());
        OnNotUnderstood.AddListener(() => StartCoroutine(IntroduceGame()));
        OnDidntCatch.AddListener(() => ListenAgain());
        OnTimeout.AddListener(() => continueLoop = true);

        StartCoroutine(IntroduceGame());
    }

    public IEnumerator IntroduceGame()
    {
        
        while (currentSegment < startAudio.Length)
        {
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

            //Wait for player's response
            IEnumerator timer = Timer();
            //StartCoroutine(timer);
            yield return new WaitUntil(() => continueLoop==true); // either GameManager.SpeechText has been filled, or x time has passed
            //StopCoroutine(timer);
        }

        GameManager.Instance.OnInstructionFinished.Invoke();
    }

    IEnumerator Timer()
    {
        Debug.Log("Timer started");
        yield return new WaitForSeconds(timeOutLimit);
        Debug.Log("Timer ended");
        OnTimeout.Invoke();
    }

    void GotoNextSegment()
    {
        currentSegment++;
        continueLoop = true;
    }

    void ListenAgain()
    {
        //reset dictation engine
        DictationEngine.Instance.CloseDictationEngine();
        DictationEngine.Instance.StartDictationEngine();
    }

    //react to user speech
    public IEnumerator ReactToSpeech(string _speech)
    {
        StopTalking();
        if (_speech == "i understand" || _speech == "yes")
        {
            DictationEngine.Instance.CloseDictationEngine();
            audioSource.PlayOneShot(goodFeedback);
            yield return new WaitForSeconds(goodFeedback.length + 0.2f);
            Debug.Log("Good");
            OnUnderstood.Invoke();
            yield break;
        }
        else if (_speech == "i don't understand" || _speech == "no")
        {
            DictationEngine.Instance.CloseDictationEngine();
            Debug.Log("I repeat");
            audioSource.PlayOneShot(repeat);
            yield return new WaitForSeconds(repeat.length + 0.2f);
            OnNotUnderstood.Invoke();
            yield break;
        }
        else
        {
            Debug.Log("I didn't catch that");
            audioSource.PlayOneShot(didntCatch);
            yield return new WaitForSeconds(didntCatch.length);
            OnDidntCatch.Invoke();
        }
    }
    //start the introduction to specified puzzle
    public void IntroducePuzzle(int _currentPuzzle)
    {
        audioSource.clip = puzzleIntros[_currentPuzzle];
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
        OnUnderstood.RemoveAllListeners();
        OnNotUnderstood.RemoveAllListeners();
        OnDidntCatch.RemoveAllListeners();
    }
    private void OnApplicationQuit()
    {
        OnUnderstood.RemoveAllListeners();
        OnNotUnderstood.RemoveAllListeners();
        OnDidntCatch.RemoveAllListeners();
    }
}
