using UnityEngine;

public class Fire : PowerUp
{
    private readonly GameObject explosionEffectPrefab;
    public Fire(
        PowerUpSystem system,
        float durationSeconds,
        GameObject explosionPrefab)
        : base(system,  durationSeconds)
    {
        explosionEffectPrefab = explosionPrefab;
    }

    public override string Name => "fire";

    public override void OnHit(Ball ball, Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bumper"))
            return;
        if (explosionEffectPrefab != null)
        {
            Object.Instantiate(explosionEffectPrefab, ball.transform.position, Quaternion.identity);
        }
        if (collision.gameObject.TryGetComponent(out Bumper bumper))
        {
            bumper.Deactivate();
        }
    }
}
