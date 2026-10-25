using System.Collections.Generic;
using UnityEngine;

public class SpecialBumper : MonoBehaviour, ICollisionEffect
{
    [SerializeField] private float spread = 45f;
    [SerializeField] private int hitsToDupe = 3;

    public void OnHit(Ball ball, Collision2D collision)
    {
        if (ball.BumperHitCount <= 0) return;
        if (BallManager.Instance == null) return;

        BallManager ballManager = BallManager.Instance;
        int ballsToDupe = Mathf.Min(2, ball.BumperHitCount / hitsToDupe);

        if (ballsToDupe <= 1) return;

        Vector2 collisionDir = ball.Velocity.normalized;
        List<Vector2> splashForces = GetFanSplashDirs(collisionDir, ballsToDupe);

        for (int i = 0; i < splashForces.Count; i++)
        {
            Vector2 force = splashForces[i] * ball.Speed;
            ballManager.SpawnBallAt(ball.transform.position, force);
        }

        ball.ReleaseBall();
    }

    private List<Vector2> GetFanSplashDirs(Vector2 direction, int count)
    {
        List<Vector2> forces = new List<Vector2>();

        direction = direction.normalized;
        float angleStep = spread / count;
        float leftMost = -count * angleStep / 2f;

        // to make the fan centered
        float correction = spread / count / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = leftMost + i * angleStep + correction;
            Vector2 dir = Quaternion.Euler(new Vector3(0, 0, angle)) * direction;
            forces.Add(dir.normalized);
        }

        return forces;
    }
}
