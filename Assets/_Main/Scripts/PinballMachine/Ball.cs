using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private BallManager ballManager;

    public void Initialize(BallManager manager)
    {
        ballManager = manager;
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.localPosition = Vector3.zero;
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    public void ReleaseBall()
    {
        ballManager.ReleaseBall(this);
    }

    public void IncrementHitCount()
    {
        ballManager.IncrementHitCount();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICollisionEffect collFX))
        {
            collFX.OnHit(this, collision);
        }
    }
}
