using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private int currentPuzzle = 0;
    private string speechText;
    private string correctPhrase;

    public int CurrentPuzzle { get => currentPuzzle; set => currentPuzzle = value; }
    public string SpeechText { get => speechText; set => speechText = value; }
    public string CorrectPhrase { get => correctPhrase; set => correctPhrase = value; }

    private GameFSM fsm;
    
    public KeyCode speechInput = KeyCode.Space;

    public UnityEvent OnInstructionFinished;

    private void Awake()
    {
        instance = this;
        
        fsm = new GameFSM();
        fsm.Initialize(this);

        fsm.AddState(GameStateType.Start, new StartState());
        fsm.AddState(GameStateType.Intro, new IntroState());
        fsm.AddState(GameStateType.Listening, new ListeningState());
        fsm.AddState(GameStateType.Correct, new CorrectState());

        OnInstructionFinished.AddListener(() => fsm.GotoState(GameStateType.Intro));
    }

    private void Start()
    {
        fsm.GotoState(GameStateType.Start);
    }

    private void Update()
    {
        fsm.UpdateState();
    }

    public bool IsPhraseCorrect()
    {
        return speechText == correctPhrase;
    }

    public void SetSpeechText(string s) => speechText = s;

    private void OnDestroy()
    {
        OnInstructionFinished.RemoveAllListeners();
    }
    private void OnApplicationQuit()
    {
        OnInstructionFinished.RemoveAllListeners();
    }
}
