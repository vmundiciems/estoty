using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static event Action OnStageEnded;
    public static event Action OnStageReset;
    public static bool CanContinue { get; private set; }

    [SerializeField] private float gameplayDuration = 30;
    [SerializeField] private int bonusTimerValue = 5;
    [SerializeField] private int failTimerValue = -10;
    [SerializeField] private int bonusScore = 100;

    private float gameplayTimer;
    private int score;

    public float GameplayTimer => gameplayTimer;
    public int Score => score;
    public int BonusTimerValue => bonusTimerValue;
    public int FailTimerValue => failTimerValue;

    private void Start()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        gameplayTimer = gameplayDuration;
        score = 0;
        CanContinue = true;
        OnStageReset?.Invoke();
    }

    private void Update()
    {
        gameplayTimer -= Time.deltaTime;
        if (gameplayTimer < 0)
        {
            CanContinue = false;
            OnStageEnded?.Invoke();
        }
    }

    private void OnEnable()
    {
        Ball.OnFall += OnBallFall;
        Ball.OnHolePassed += OnHolePassed;
    }

    private void OnDisable()
    {
        Ball.OnFall -= OnBallFall;
        Ball.OnHolePassed -= OnHolePassed;
    }

    private void OnHolePassed(Hole hole, Ball ball)
    {
        if (hole.BonusSide)
        {
            score += bonusScore;
            gameplayTimer += bonusTimerValue;
        }
        else
        {
            gameplayTimer += failTimerValue;
        }
    }

    private void OnBallFall(Ball ball)
    {
        gameplayTimer += failTimerValue;
    }
}
