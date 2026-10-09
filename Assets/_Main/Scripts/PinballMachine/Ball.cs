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
    
    public void ReleaseBall()
    {
        ballManager.ReleaseBall(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICollisionEffect collFX))
        {
            ballManager.OnBumperHit();
            collFX.OnHit(this);
        }
    }
}
