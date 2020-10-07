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
//StartState - start of game. Input instructions
public class StartState : GameState
{
    float t = 0f;
    
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        t += Time.deltaTime;

        if (t >= Guard.Instance.GetClipLength())
            owner.GotoState(GameStateType.Intro);
    }
    public override void Exit()
    {
        Guard.Instance.StopTalking();
    }
}
// IntroState - introduction of puzzle + instructions, maybe some narrative exposition
public class IntroState : GameState
{
    float t = 0;
    string response = "";
    public override void Enter()
    {
        Guard.Instance.StartIntro(gm.CurrentPuzzle);
    }
    public override void Update()
    {
        t += Time.deltaTime;

        if (t >= Guard.Instance.GetClipLength() + 0.5f)
        {
            response = "yes";
        }
        /*
        // wait for user 'yes' input
        if (Input.GetKeyDown(gm.speechInput))
        {
            // StartSpeech()
        }

        if (Input.GetKeyUp(gm.speechInput))
        {
            // EndSpeech()
            //check string
            // if yes { goto ListeningState }
            // if no { play intro again }
        }*/
        if (response == "yes")
            owner.GotoState(GameStateType.Listening);
        if (response == "no")
        {
            response = "";
            t = 0;
            Guard.Instance.StartIntro(gm.CurrentPuzzle);
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
    float t = 0;
    string response = "";

    public override void Enter()
    {
        Puzzle.Instance.StartPuzzle(gm.CurrentPuzzle);
    }
    public override void Update()
    {
        // wait for user input
        if (Input.GetKeyDown(gm.speechInput))
        {
            // pause audio clip
            // StartSpeech(out string)
        }

        if (Input.GetKeyUp(gm.speechInput))
        {
            // EndSpeech()
            // check string
            // if correct { goto CorrectState }
            // if wrong { play wrong-answer feedback, then resume audio clip }
        }
    }
    public override void Exit()
    {
        
    }
}
public class CorrectState : GameState
{
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        // when sound is done playing, increment currentPuzzle and goto IntroState
    }
    public override void Exit()
    {

    }
}