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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bumper"))
        {
            ballManager.OnBumperHit();
        }
    }

    void OnDestroy()
    {
        ballManager.OnBallDestroyed(this);
    }
}
