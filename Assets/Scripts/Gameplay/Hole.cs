using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hole : MonoBehaviour
{
    [SerializeField] private float moveToPositionDuration = 1.5f;
    [SerializeField] private float movementYOffset = -5f;
    [SerializeField] private float minFlipAt = 3f;
    [SerializeField] private float maxFlipAt = 15f;

    private float flipTimer;
    private Coroutine moveCoroutine;
    private bool bonusSide = true;
    private Animator animator;

    public bool BonusSide => bonusSide;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        flipTimer = Random.Range(minFlipAt, maxFlipAt);
    }

    private void Update()
    {
        if (!GameplayManager.CanContinue) return;

        flipTimer -= Time.deltaTime;
        if (flipTimer <= 0)
        {
            flipTimer = Random.Range(minFlipAt, maxFlipAt);
            animator.Play(bonusSide ? "Flip" : "Flip back", 0, 0);
        }
    }

    public void OnFlipDone() => bonusSide = !bonusSide;

    public void MoveToNewPosition(Vector3 newPosition)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveToPositionAnimation(newPosition));
    }

    private IEnumerator MoveToPositionAnimation(Vector3 newPosition)
    {
        var startPoint = transform.position;
        var controlPoint = Vector3.Lerp(startPoint, newPosition, 0.5f);
        controlPoint.y = movementYOffset;

        float timer = moveToPositionDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float t = Mathf.Clamp01((moveToPositionDuration - timer) / moveToPositionDuration);
            t = Mathf.Sin(Mathf.PI / 2 * t);
            Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, newPosition, t);
            transform.position = Vector3.Lerp(m1, m2, t);
            yield return null;
        }

        transform.position = newPosition;
    }
}
