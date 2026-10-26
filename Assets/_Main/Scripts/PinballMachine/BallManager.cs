using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

[DefaultExecutionOrder(-1)]
public class BallManager : MonoBehaviour, IResetable
{
    public static BallManager Instance { get; private set; }

    [SerializeField] private Transform ballParent;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private int dupHitCount = 8;
    [SerializeField] private TextMeshProUGUI textDupHit;
    [Header("Input")]

    private int currHitCount;
    private List<Ball> activeBalls = new List<Ball>();

    private IObjectPool<Ball> ballPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
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

        // SpawnBall();
        // UpdateUI();
    }

    private void OnEnable()
    {
        GameState.OnGameStarted += Reset;
    }

    private void OnDisable()
    {
        GameState.OnGameStarted -= Reset;
    }

    void SpawnBall()
    {
        if (ballPool == null)
        {
            Debug.LogError("Ball pool is not initialized.");
            return;
        }
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

    public void SpawnBallAt(Vector3 position, Vector2 velocity)
    {
        Ball newBall = ballPool.Get();
        newBall.ResetBall();
        newBall.transform.position = position;
        newBall.AddForce(velocity);
    }

    public void Reset()
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

    public void IncrementHitCount()
    {
        currHitCount++;

        if (currHitCount >= dupHitCount)
        {
            currHitCount = 0;
            SpawnBall();
        }

        UpdateUI();
    }

    public void ReleaseBall(Ball ball)
    {
        if (activeBalls.Contains(ball))
        {
            activeBalls.Remove(ball);
            ballPool.Release(ball);
        }

        if (activeBalls.Count == 0)
        {
            GameState.Instance.SetGameOver();
        }
    }

    private void UpdateUI()
    {
        textDupHit.text = $"{dupHitCount - currHitCount}";
    }
}
