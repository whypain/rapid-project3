using UnityEngine;

public class Fire : PowerUp
{
    private readonly GameObject explosionEffectPrefab;
    public Fire(
        PowerUpSystem system,
        Ball targetBall,
        float durationSeconds,
        GameObject explosionPrefab)
        : base(system, targetBall, durationSeconds)
    {
        explosionEffectPrefab = explosionPrefab;
    }
    public override void OnHit(Ball ball, Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bumper"))
            return;
        if (explosionEffectPrefab != null)
        {
            Object.Instantiate(explosionEffectPrefab, ball.transform.position, Quaternion.identity);
        }
        Object.Destroy(collision.gameObject);
    }
}
