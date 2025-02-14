using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Pool;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallManager : MonoBehaviour
{
    [SerializeField] private TrajectoryPredictor predictor;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private float touchSensitivity = 0.004f;
    [SerializeField] private float shootingForce = 20;
    [SerializeField] private Vector2 angleLimitMin;
    [SerializeField] private Vector2 angleLimitMax;
    [SerializeField] private Ball currentBall;
    [SerializeField] private float ballRespawnDuration = 1;

    private ObjectPool<Ball> ballPool;
    private float ballTimer;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
#if UNITY_EDITOR
        TouchSimulation.Enable();
#endif
    }

    private void Start()
    {
        ballPool = new ObjectPool<Ball>(
            () => Instantiate(ballPrefab, Vector3.zero, Quaternion.identity),
            (b) => 
            { 
                b.gameObject.SetActive(true); 
                b.transform.position = Vector3.zero; 
                b.Rigidbody.isKinematic = true; 
            },
            (b) => b.gameObject.SetActive(false),
            (b) => Destroy(b.gameObject)
        );
        RespawnBall();
    }

    private void OnEnable()
    {
        GameplayManager.OnStageReset += OnStageReset;
        Ball.OnFall += OnBallFall;
        Ball.OnHolePassed += OnHolePassed;
    }

    private void OnDisable()
    {
        GameplayManager.OnStageReset -= OnStageReset;
        Ball.OnFall -= OnBallFall;
        Ball.OnHolePassed -= OnHolePassed;
    }

    private void OnStageReset()
    {
        predictor.SetTrajectoryVisible(false);
        RespawnBall();
    }

    private void OnBallFall(Ball ball)
    {
        ballPool.Release(ball);
    }

    private void OnHolePassed(Hole hole, Ball ball)
    {
        OnBallFall(ball);
    }

    private void Update()
    {
        if (!GameplayManager.CanContinue) return;

        if (currentBall == null)
        {
            ballTimer -= Time.deltaTime;
            if (ballTimer <= 0)
            {
                RespawnBall();
            }
        }

        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    RespawnBall();
                    transform.rotation = Quaternion.Euler(angleLimitMin.x, 0, 0);
                    predictor.SetTrajectoryVisible(true);
                    PredictTrajectory();
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                    if (predictor.IsTrajectoryVisible)
                    {
                        predictor.SetTrajectoryVisible(false);
                        currentBall.Rigidbody.isKinematic = false;
                        currentBall.Rigidbody.AddForce(transform.forward * shootingForce, ForceMode.Impulse);
                        currentBall = null;
                        ballTimer = ballRespawnDuration;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    var direction = (touch.screenPosition - touch.startScreenPosition) * touchSensitivity;
                    float tX = Mathf.Clamp(direction.y, 0, 1);
                    float tY = Mathf.Clamp(direction.x, -1, 1);

                    transform.rotation = Quaternion.Euler(
                        Mathf.Lerp(angleLimitMin.x, angleLimitMax.x, tX),
                        Mathf.Lerp(angleLimitMin.y, angleLimitMax.y, (1 + tY) / 2),
                        0);

                    PredictTrajectory();
                    break;
            }
        }
    }

    private void PredictTrajectory()
    {
        predictor.PredictTrajectory(
            transform.forward,
            transform.position,
            shootingForce,
            ballPrefab.Rigidbody.mass,
            ballPrefab.Rigidbody.linearDamping);
    }

    private void RespawnBall()
    {
        if (currentBall == null)
        {
            currentBall = ballPool.Get();
        }
    }
}
