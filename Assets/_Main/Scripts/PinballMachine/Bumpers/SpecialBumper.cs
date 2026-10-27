using System.Collections.Generic;
using UnityEngine;

public class SpecialBumper : Bumper, ICollisionEffect
{
    [SerializeField] private float spread = 45f;
    [SerializeField] private int hitsToDupe = 3;

    [Header("Sound Reference")]
    [SerializeField] private SoundConfig ballDupe;

    public override void OnHit(Ball ball, Collision2D collision)
    {
        base.OnHit(ball, collision);
        if (ball.BumperHitCount <= 0) return;
        if (BallManager.Instance == null) return;

        BallManager ballManager = BallManager.Instance;
        int ballsToDupe = Mathf.Min(2, ball.BumperHitCount / hitsToDupe);

        if (ballsToDupe <= 1) return;

        Vector2 collisionDir = ball.Velocity.normalized;
        List<Vector2> splashForces = GetFanSplashDirs(collisionDir, ballsToDupe);

        bool spawned = false;
        for (int i = 0; i < splashForces.Count; i++)
        {
            Vector2 force = splashForces[i] * ball.Speed;
            spawned = ballManager.SpawnBallAt(ball.transform.position, force);

            if (!spawned)
            {
                Debug.LogWarning("Failed to spawn ball. Maximum number of balls reached.");
                break;
            }
        }

        if (spawned)
        {
            SoundManager.Instance.PlaySoundFromPool(ballDupe);
            ball.ReleaseBall();
        }
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
