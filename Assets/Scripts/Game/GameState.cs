using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType { Start, Intro, Listening, Correct }
public abstract class GameState
{
    protected GameFSM owner;
    protected GameManager gm;

    public void Initialize(GameFSM _owner)
    {
        owner = _owner;
        gm = _owner.owner;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
//StartState - start of game
public class StartState : GameState
{
    float t = 0f;
    bool dict = false;

    public override void Enter()
    {
        
    }
    public override void Update()
    {
        // start voice input
        if (Input.GetKeyDown(gm.speechInput) && !DictationEngine.Instance.isOpened)
        {
            DictationEngine.Instance.StartDictationEngine();
            Debug.Log("Listening...");
        }

        //check if user has said anything so far
        if (gm.SpeechText != null)
        {
            DictationEngine.Instance.CloseDictationEngine();
            Guard.Instance.ProcessSpeech(gm.SpeechText);
            gm.SetSpeechText(null);
        }

    }
    public override void Exit()
    {
        Guard.Instance.StopAllCoroutines();
        Guard.Instance.StopTalking();
    }
}
// IntroState - introduction of puzzle, instructions, maybe some narrative exposition
public class IntroState : GameState
{
    bool dict = false;
    float timeElapsed = 0f;
    public override void Enter()
    {
        Guard.Instance.IntroducePuzzle(gm.CurrentPuzzle);
        gm.SpeechText = null;
    }
    public override void Update()
    {
        timeElapsed += Time.deltaTime;
        
        // go to next state after clip is done
        if (timeElapsed >= Guard.Instance.GetClipLength() + 0.5f)
        {
            owner.GotoState(GameStateType.Listening);
        }
    }
    public override void Exit()
    {
        Guard.Instance.StopTalking();
        DictationEngine.Instance.CloseDictationEngine();
    }
}
// ListeningState - user is listening to sound fragment
public class ListeningState : GameState
{
    float timeElapsed = 0f;
    bool dict = false;

    public override void Enter()
    {
        Puzzle.Instance.StartPuzzle(gm.CurrentPuzzle);
    }
    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        // start voice input
        if (Input.GetKeyDown(gm.speechInput))
        {
            if (!dict)
            {
                DictationEngine.Instance.StartDictationEngine();
                dict = true;
            }
            Puzzle.Instance.SetVolume(0.2f);
        }

        //check if user has given the correct answer
        if (gm.SpeechText != null)
        {
            if (gm.IsPhraseCorrect())
            {
                owner.GotoState(GameStateType.Correct);
            }
            else
            {
                Guard.Instance.EvaluateAnswer(false);
                Puzzle.Instance.SetVolume(0.7f);
                Debug.Log("WRONG");
                gm.SpeechText = null;
                timeElapsed = 0f;
            }
        }

    }
    public override void Exit()
    {
        Puzzle.Instance.StopAudio();
        gm.SpeechText = null;
        DictationEngine.Instance.CloseDictationEngine();
    }
}

public class CorrectState : GameState
{
    float t = 0;
    bool done = false;
    public override void Enter()
    {
        Guard.Instance.EvaluateAnswer(true);
    }
    public override void Update()
    {
        t += Time.deltaTime;
        if (t >= Guard.Instance.GetClipLength() + 0.5f && !done)
        {
            done = true;
            Debug.Log("done");
            Puzzle.Instance.PlayDemoEndClip();
        }
        // when sound is done playing, increment currentPuzzle and goto IntroState
    }
    public override void Exit()
    {

    }
}