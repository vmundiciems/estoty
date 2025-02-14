using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private TMP_Text topTimer;
    [SerializeField] private BonusTimerText bonusText;
    [SerializeField] private BonusTimerText failText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        bonusText.Text.text = gameplayManager.BonusTimerValue.ToString("+0;-0");
        bonusText.gameObject.SetActive(false);
        failText.Text.text = gameplayManager.FailTimerValue.ToString("+0;-0");
        failText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameplayManager.OnStageEnded += OnStageEnded;
        GameplayManager.OnStageReset += OnStageReset;

        Ball.OnFall += OnBallFall;
        Ball.OnHolePassed += OnHolePassed;
    }

    private void OnDisable()
    {
        GameplayManager.OnStageEnded -= OnStageEnded;
        GameplayManager.OnStageReset -= OnStageReset;

        Ball.OnFall -= OnBallFall;
        Ball.OnHolePassed -= OnHolePassed;
    }

    private void LateUpdate()
    {
        topTimer.text = $"{gameplayManager.GameplayTimer:0}";
    }

    private void OnStageEnded()
    {
        topTimer.gameObject.SetActive(false);
        endPanel.SetActive(true);
        scoreText.text = gameplayManager.Score.ToString();
    }

    private void OnHolePassed(Hole hole, Ball ball)
    {
        if (hole.BonusSide)
        {
            bonusText.PlayAnimation();
        }
        else
        {
            failText.PlayAnimation();
        }
    }

    private void OnBallFall(Ball ball)
    {
        failText.PlayAnimation();
    }

    private void OnStageReset()
    {
        topTimer.gameObject.SetActive(true);
        endPanel.SetActive(false);
    }
}
