using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallManager : MonoBehaviour
{
    [SerializeField] private Transform ballParent;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private int dupHitCount = 8;

    [Header("Input")]
    [SerializeField] private InputActionReference actionRef;

    private int currHitCount;
    private List<Ball> activeBalls = new List<Ball>();
    private InputAction action;

    private void Start()
    {
        action = actionRef.action;

        action.Enable();
        action.performed += OnAction;
        SpawnBall();
    }

    void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("Ball prefab reference is missing.");
            return;
        }
        if (ballParent == null)
        {
            Debug.LogError("Ball parent reference is missing.");
            return;
        }

        Ball newBall = Instantiate(ballPrefab, ballParent);
        newBall.Initialize(this);
        activeBalls.Add(newBall);
    }

    void OnAction(InputAction.CallbackContext _)
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null)
            {
                Destroy(ball.gameObject);
            }
        }
        activeBalls.Clear();
        currHitCount = 0;
        SpawnBall();
    }

    public void OnBumperHit()
    {
        currHitCount++;
        if (currHitCount >= dupHitCount)
        {
            currHitCount = 0;
            SpawnBall();
        }
    }

    public void OnBallDestroyed(Ball ball)
    {
        activeBalls.Remove(ball);
    }

    void OnDestroy()
    {
        action.performed -= OnAction;
        action.Disable();
    }
}
