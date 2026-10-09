using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

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

    private IObjectPool<Ball> ballPool;

    private void Start()
    {
        action = actionRef.action;

        action.Enable();
        action.performed += OnAction;

        ballPool = new ObjectPool<Ball>(
            createFunc: () =>
            {
                Ball newBall = Instantiate(ballPrefab, ballParent);
                newBall.Initialize(this);
                return newBall;
            },
            actionOnGet: (ball) =>
            {
                ball.gameObject.SetActive(true);
                activeBalls.Add(ball);
            },
            actionOnRelease: (ball) =>
            {
                ball.gameObject.SetActive(false);
            },
            // actionOnDestroy: (ball) =>
            // {
            //     Destroy(ball.gameObject);
            // },
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
        );
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

        Ball newBall = ballPool.Get();
        newBall.ResetBall();
    }

    void OnAction(InputAction.CallbackContext _)
    {
        foreach (var ball in activeBalls)
        {
            if (ball != null)
            {
                ballPool.Release(ball);
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

    public void ReleaseBall(Ball ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
            ballPool.Release(ball);
        }
    }

    void OnDestroy()
    {
        action.performed -= OnAction;
        action.Disable();
    }
}
