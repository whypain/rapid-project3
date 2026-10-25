using UnityEngine;

[System.Serializable]
public class PowerUp
{
    protected PowerUpSystem powerUpSystem;
    protected Ball ball;
    protected float duration;
    protected float durationLeft;
    protected PowerUp(
        PowerUpSystem system,
        Ball targetBall,
        float durationSeconds)
    {
        powerUpSystem = system;
        ball = targetBall;
        duration = Mathf.Max(0f, durationSeconds);
        durationLeft = duration;
    }
    public virtual void Update(float deltaTime)
    {
        durationLeft -= deltaTime;
    }
    public virtual void OnHit(Ball ball, Collision2D collision)
    {
        // ball.OnHit();
    }
    public bool IsExpired()
    {
        return durationLeft <= 0;
    }
    public virtual void OnExpire()
    {
        // ใช้คืนสถานะเมื่อบัฟหมดเวลา
    }
}
