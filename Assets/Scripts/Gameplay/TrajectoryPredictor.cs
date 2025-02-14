using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField, Range(10, 100)] private int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f)] private float increment = 0.025f;

    private LineRenderer trajectoryLine;

    private void Awake()
    {
        trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(false);
    }

    public void PredictTrajectory(Vector3 direction, Vector3 initialPosition, float initialSpeed, float mass, float drag)
    {
        Vector3 velocity = direction * (initialSpeed / mass);
        Vector3 position = initialPosition;
        Vector3 nextPosition;

        UpdateLineRender(maxPoints, 0, position);

        for (int i = 1; i < maxPoints; i++)
        {
            velocity = CalculateNewVelocity(velocity, drag, increment);
            nextPosition = position + velocity * increment;

            position = nextPosition;
            UpdateLineRender(maxPoints, i, position);
        }
    }

    private void UpdateLineRender(int count, int pointIndex, Vector3 pointPosition)
    {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointIndex, pointPosition);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
    }

    public bool IsTrajectoryVisible => trajectoryLine.enabled;
}
