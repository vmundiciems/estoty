using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Color gizmoColor = Color.blue;
#endif
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;
    [SerializeField, Range(1, 7)] private int numberOfHoles = 5;
    [SerializeField] private Hole holePrefab;
    [SerializeField] private float minDistanceBetweenHoles = 2;

    private readonly List<Hole> holes = new();

    public Vector3 MinBound => new(transform.position.x + minBound.x, transform.position.y, transform.position.z + minBound.y);
    public Vector3 MaxBound => new(transform.position.x + maxBound.x, transform.position.y, transform.position.z + maxBound.y);

    private void OnEnable()
    {
        GameplayManager.OnStageReset += Initialize;
        Ball.OnHolePassed += OnHolePassed;
    }

    private void OnDisable()
    {
        GameplayManager.OnStageReset -= Initialize;
        Ball.OnHolePassed -= OnHolePassed;
    }

    public void Initialize()
    {
        if (holes.Count > numberOfHoles)
        {
            for (int i = numberOfHoles - 1; i < holes.Count; i++)
            {
                Destroy(holes[i].gameObject);
            }
        }

        if (holes.Count < numberOfHoles)
        {
            for (int i = holes.Count; i < numberOfHoles; i++)
            {
                holes.Add(Instantiate(holePrefab, transform));
            }
        }

        foreach (var hole in holes)
        {
            hole.transform.position = GetNewRandomPosition();
        }

        foreach (var hole in holes)
        {
            hole.transform.position = ValidatePosition(hole, hole.transform.position);
        }
    }

    private Vector3 GetNewRandomPosition() => new(Random.Range(MinBound.x, MaxBound.x), 0, Random.Range(MinBound.z, MaxBound.z));

    private bool HasValidPosition(Hole currentHole, Vector3 expectedPosition)
    {
        foreach (var hole in holes)
        {
            if (currentHole == hole) continue;
            if (Vector3.Distance(expectedPosition, hole.transform.position) < minDistanceBetweenHoles) return false;
        }

        return true;
    }

    private Vector3 ValidatePosition(Hole hole, Vector3 position)
    {
        int numberOfAttempts = 0;
        while (!HasValidPosition(hole, position) && ++numberOfAttempts < 1000)
        {
            position = GetNewRandomPosition();
        }

        if (numberOfAttempts == 1000)
        {
            Debug.LogWarning("Failed to find proper position for hole", hole);
        }

        return position;
    }


    private void OnHolePassed(Hole hole, Ball ball)
    {
        var newPosition = ValidatePosition(hole, GetNewRandomPosition());
        hole.MoveToNewPosition(newPosition);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Gizmos.DrawLine(new Vector3(x + minBound.x, y, z + minBound.y), new Vector3(x + maxBound.x, y, z + minBound.y));
        Gizmos.DrawLine(new Vector3(x + maxBound.x, y, z + minBound.y), new Vector3(x + maxBound.x, y, z + maxBound.y));
        Gizmos.DrawLine(new Vector3(x + maxBound.x, y, z + maxBound.y), new Vector3(x + minBound.x, y, z + maxBound.y));
        Gizmos.DrawLine(new Vector3(x + minBound.x, y, z + maxBound.y), new Vector3(x + minBound.x, y, z + minBound.y));
    }
#endif
}
