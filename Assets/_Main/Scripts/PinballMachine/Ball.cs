using System.Collections.Generic;
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

    [SerializeField] private List<PowerUp> activePowerUps = new List<PowerUp>();

    private BallManager ballManager;
    private int bumperHitCount;

    private float attack;
    private float bumpForce;

    public Rigidbody2D Rigidbody => rb;
    public float Attack => attack;
    public int BumperHitCount => bumperHitCount;
    public Vector2 Velocity => rb.linearVelocity;
    public float Speed => rb.linearVelocity.magnitude;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        UpdateStats();
    }
    public void Initialize(BallManager manager)
    {
        ballManager = manager;

        bumperHitCount = 0;
        activePowerUps.Clear();
        UpdateStats();
    }

    public void ResetBall()
    {
        ClearPowerUps();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.localPosition = Vector3.zero;

        bumperHitCount = 0;
        //activePowerUps.Clear();
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
    public void ApplyPowerUp(PowerUp powerUp)
    {
        if (powerUp != null)
        {
            activePowerUps.Add(powerUp);
            Debug.Log($"Ball received {powerUp.GetType().Name}");
        }
    }
    public List<PowerUp> GetPowerUp()
    {
        return activePowerUps;
    }

    public void OnHit()
    {
        // ballManager?.IncrementHitCount();

        Debug.Log("HIt");
        bumperHitCount++;
        UpdateStats();
    }

    public void ResetHitCount()
    {
        bumperHitCount = 0;
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
    private void Update()
    {
        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            PowerUp powerUp = activePowerUps[i];

            if (powerUp == null)
            {
                activePowerUps.RemoveAt(i);
                continue;
            }

            powerUp.Update(Time.deltaTime);

            if (powerUp.IsExpired())
            {
                powerUp.OnExpire();
                activePowerUps.RemoveAt(i);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            PowerUp powerUp = activePowerUps[i];
            
            if (powerUp == null || powerUp.IsExpired()) continue;

            powerUp.OnHit(this, collision);
        }
        if (collision.gameObject.TryGetComponent(out ICollisionEffect collisionEffect))
        {
            collisionEffect.OnHit(this, collision);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out ITriggerEffect triggerEffect))
        {
            triggerEffect.OnHit(this);
        }
    }

    private void ClearPowerUps()
    {
        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            activePowerUps[i].OnExpire();
        }

        activePowerUps.Clear();
    }

    private void OnDestroy()
    {
        ClearPowerUps();
    }

}
