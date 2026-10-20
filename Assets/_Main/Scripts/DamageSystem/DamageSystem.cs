using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : MonoBehaviour, IResetable
{
    public static DamageSystem Instance { get; private set; }

    [SerializeField] private float timeTilDestroy = 5f;

    private Dictionary<Ball, float> damagedBalls = new Dictionary<Ball, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetTimeTilDestroy(Ball ball)
    {
        if (damagedBalls.TryGetValue(ball, out float destroyTime))
        {
            return Mathf.Max(0f, destroyTime - Time.time);
        }
        return 0f;
    }

    public void RegisterDamagedBall(Ball ball)
    {
        if (!damagedBalls.ContainsKey(ball))
        {
            damagedBalls.Add(ball, Time.time + timeTilDestroy);
        }
    }

    public void UnregisterDamagedBall(Ball ball)
    {
        if (damagedBalls.ContainsKey(ball))
        {
            damagedBalls.Remove(ball);
        }
    }

    private void Update()
    {
        UpdateDamagedBalls();
    }

    private void UpdateDamagedBalls()
    {
        List<Ball> ballsToRemove = new List<Ball>();

        foreach (var entry in damagedBalls)
        {
            Ball ball = entry.Key;
            float destroyTime = entry.Value;

            if (Time.time >= destroyTime)
            {
                ballsToRemove.Add(ball);
            }
        }

        foreach (var ball in ballsToRemove)
        {
            damagedBalls.Remove(ball);
            ball.ReleaseBall();
        }
    }

    public void Reset()
    {
        damagedBalls.Clear();
    }
}