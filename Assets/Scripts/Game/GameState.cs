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
    
    public override void Enter()
    {
        gm.timeOutTimer = 0f;
        gm.checkingTimeOut = false;
    }
    public override void Update()
    {
        if (gm.checkingTimeOut)
        {
            gm.timeOutTimer += Time.deltaTime;
        }

        if (gm.checkingTimeOut && gm.timeOutTimer >= Guard.Instance.timeOutLimit)
        {
            Guard.Instance.OnTimeout.Invoke();
            gm.checkingTimeOut = false;
        }

        // start voice input
        if (Input.GetKeyDown(gm.speechInput) && !DictationEngine.Instance.isOpened && Guard.Instance.readyToListen)
        {
            gm.checkingTimeOut = false;
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
    float timeElapsed;

    public override void Enter()
    {
        Guard.Instance.IntroducePuzzle(gm.CurrentPuzzle);
        timeElapsed = 0f;
    }
    public override void Update()
    {
        timeElapsed += Time.deltaTime;
        
        // go to next state after clip is done
        if (timeElapsed >= Guard.Instance.GetClipLength() + 0.5f)
        {
            Debug.Log(timeElapsed);
            owner.GotoState(GameStateType.Listening);
        }
    }
    public override void Exit()
    {
        Guard.Instance.StopTalking();
    }
}
// ListeningState - user is listening to sound fragment
public class ListeningState : GameState
{
    
    

    public override void Enter()
    {
        Puzzle.Instance.StartPuzzle(gm.CurrentPuzzle);
        gm.CorrectPhrase = Puzzle.Instance.GetCorrectPhrase(gm.CurrentPuzzle);
        gm.SpeechText = null;
    }
    public override void Update()
    {
        
        // start voice input
        if (Input.GetKeyDown(gm.speechInput) && !DictationEngine.Instance.isOpened)
        {
            DictationEngine.Instance.StartDictationEngine();
            Puzzle.Instance.Pause();
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
                DictationEngine.Instance.CloseDictationEngine();
                gm.InputBad();
                Guard.Instance.EvaluateAnswer(false);
                Puzzle.Instance.StartCoroutine(Puzzle.Instance.Unpause());
                Debug.Log("WRONG");
                gm.SpeechText = null;
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
    float t;
    bool done = false;
    public override void Enter()
    {
        done = false;
        t = 0f;
        Guard.Instance.EvaluateAnswer(true);
        gm.InputGood();
    }
    public override void Update()
    {
        t += Time.deltaTime;
        if (t >= Guard.Instance.GetClipLength() + 0.5f && !done)
        {
            done = true;
            Debug.Log("done");
            // DIRTY HACK BECAUSE NO TIME AND IT'S JUST A PROTOTYPE SO WHATEVER
            if (gm.CurrentPuzzle == 1)
            {
                Puzzle.Instance.PlayDemoEndClip();
            }
            if (gm.CurrentPuzzle == 0)
            {
                gm.CurrentPuzzle = 1;
                owner.GotoState(GameStateType.Intro); 
            }
        }
        // when sound is done playing, increment currentPuzzle and goto IntroState
    }
    public override void Exit()
    {

    }
}