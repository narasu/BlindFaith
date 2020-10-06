using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameFSM owner;

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

// FirstState - start of game, input instructions

// IntroState - introduction of puzzle + instructions, maybe some in-between narrative exposition

// ListeningState - user is listening to sound fragment

// SpeakingState - enter state with spacebar, user is speaking

// CorrectState - feedback on correct answer puzzle, increment currentPuzzle counter and goto IntroState

// WrongState - feedback on wrong answer, loop back to ListeningState

