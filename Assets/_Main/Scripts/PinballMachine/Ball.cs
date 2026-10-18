using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float bumpMultiplier = 1.2f;
    [SerializeField] private float baseAttack = 1f;
    [SerializeField] private float baseBumpForce = 1f;
    [SerializeField] private float baseGravity = 1f;

    private BallManager ballManager;
    private int bumperHitCount;

    private float attack;
    private float bumpForce;

    public void Initialize(BallManager manager)
    {
        ballManager = manager;

        bumperHitCount = 0;
        UpdateStats();
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.localPosition = Vector3.zero;

        bumperHitCount = 0;
        UpdateStats();
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force * bumpForce, ForceMode2D.Impulse);
    }
    
    public void ReleaseBall()
    {
        ballManager.ReleaseBall(this);
    }

    public void OnHit()
    {
        ballManager?.IncrementHitCount();

        bumperHitCount++;
        UpdateStats();
    }

    void UpdateStats()
    {
        attack = baseAttack + (bumperHitCount * bumpMultiplier);
        bumpForce = baseBumpForce + (bumperHitCount * bumpMultiplier);

        if (rb != null)
        {
            rb.gravityScale = baseGravity + (bumperHitCount * bumpMultiplier);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICollisionEffect collFX))
        {
            collFX.OnHit(this, collision);
        }
    }
}
