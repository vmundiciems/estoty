using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static event Action<Ball> OnFall;
    public static event Action<Hole, Ball> OnHolePassed;

    [SerializeField] private Rigidbody rBody;

    public Rigidbody Rigidbody => rBody;

    private void Update()
    {
        if (transform.position.y < -20)
        {
            OnFall?.Invoke(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hole")) return;

        var hole = other.transform.parent.GetComponent<Hole>();
        OnHolePassed?.Invoke(hole, this);
    }
}
