using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private GameFSM fsm;

    int currentPuzzle = 0;
    public KeyCode speechInput = KeyCode.Space;

    private void Awake()
    {
        instance = this;

        fsm = new GameFSM();
        fsm.Initialize(this);

        fsm.AddState(GameStateType.Start, new StartState());
        fsm.AddState(GameStateType.Intro, new IntroState());
        fsm.AddState(GameStateType.Listening, new ListeningState());
        fsm.AddState(GameStateType.Correct, new CorrectState());
    }

    private void Start()
    {
        fsm.GotoState(GameStateType.Start);
    }

    private void Update()
    {
        fsm.UpdateState();
    }
}
