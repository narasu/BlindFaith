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
    public override void Enter()
    {
        // PlaySound(intro)
    }
    public override void Update()
    {

    }
    public override void Exit()
    {

    }
}
// IntroState - introduction of puzzle + instructions, maybe some narrative exposition
public class IntroState : GameState
{
    public override void Enter()
    {
        // PlaySound(intro[currentPuzzle])
    }
    public override void Update()
    {
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
        }
        
    }
    public override void Exit()
    {

    }
}
// ListeningState - user is listening to sound fragment
public class ListeningState : GameState
{
    public override void Enter()
    {
        // PlayLoop(audioLoop[currentPuzzle], currentPositionInLoop)
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
        // PlaySound(correct)
    }
    public override void Update()
    {
        // when sound is done playing, increment currentPuzzle and goto IntroState
    }
    public override void Exit()
    {

    }
}