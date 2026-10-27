using System.Linq;
using UnityEngine;

public static class ColorMixer
{
    public enum Mode
    {
        Additive,
        Subtractive,
        Average
    }

    public static Color MixColors(Color color1, Color color2, Mode mode)
    {
        switch (mode)
        {
            case Mode.Additive:
                return new Color(
                    Mathf.Clamp01(color1.r + color2.r),
                    Mathf.Clamp01(color1.g + color2.g),
                    Mathf.Clamp01(color1.b + color2.b),
                    Mathf.Clamp01(color1.a + color2.a)
                );
            case Mode.Subtractive:
                return new Color(
                    Mathf.Clamp01(color1.r - color2.r),
                    Mathf.Clamp01(color1.g - color2.g),
                    Mathf.Clamp01(color1.b - color2.b),
                    Mathf.Clamp01(color1.a - color2.a)
                );
            case Mode.Average:
                return new Color(
                    (color1.r + color2.r) / 2f,
                    (color1.g + color2.g) / 2f,
                    (color1.b + color2.b) / 2f,
                    (color1.a + color2.a) / 2f
                );
            default:
                return Color.black;
        }
    }
}

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem particle;

    [Header("Settings")]
    [SerializeField] private float bumpMultiplier = 1.2f;
    [SerializeField] private float baseAttack = 1f;
    [SerializeField] private float baseBumpForce = 1f;
    [SerializeField] private float baseGravity = 1f;

    [Header("Color")]
    [SerializeField] private Color fireColor = Color.darkRed;
    [SerializeField] private Color electricColor = Color.purple;
    [SerializeField] private Color windColor = Color.green;
    [SerializeField] private ColorMixer.Mode colorMixMode = ColorMixer.Mode.Average;

    [Header("Sound")]
    [SerializeField] private SoundConfig ballHitWall;

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
        UpdateStats();
        UpdateParticleColor();
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.localPosition = Vector3.zero;

        bumperHitCount = 0;
        //activePowerUps.Clear();
        UpdateStats();
    }

    private void OnEnable()
    {
        PowerUpSystem.OnPowerUpChanged += UpdateParticleColor;
    }

    private void OnDisable()
    {
        PowerUpSystem.OnPowerUpChanged -= UpdateParticleColor;
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
        // ballManager?.IncrementHitCount();

        bumperHitCount++;
        UpdateStats();
    }

    public void ResetHitCount()
    {
        bumperHitCount = 0;
        UpdateStats();
    }

    public void UpdateParticleColor()
    {
        if (particle == null) return;

        var main = particle.main;

        var activePowerUps = PowerUpSystem.activePowerUps;

        if (activePowerUps.Count == 0)
        {
            main.startColor = Color.white;
            particle.gameObject.SetActive(false);
            Debug.Log($"[POWER-UP] No active power-ups.");
            return;
        }

        var currentColor = main.startColor.color;

        foreach (var powerUp in activePowerUps)
        {
            var powerUpColor = powerUp.Name switch
            {
                "fire" => fireColor,
                "electric" => electricColor,
                "wind" => windColor,
                _ => Color.white
            };

            currentColor = ColorMixer.MixColors(currentColor, powerUpColor, colorMixMode);
        }

        main.startColor = currentColor;
        particle.gameObject.SetActive(true);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var powerUp in PowerUpSystem.activePowerUps.ToList())
        {
            if (powerUp == null || powerUp.IsExpired()) continue;

            powerUp.OnHit(this, collision);
        }

        if (collision.gameObject.TryGetComponent(out ICollisionEffect collisionEffect))
        {
            collisionEffect.OnHit(this, collision);
        }
        else
        {
            // If the ball hits a wall or any other object without a collision effect, play the wall hit sound
            SoundManager.Instance.PlaySoundFromPool(ballHitWall);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out ITriggerEffect triggerEffect))
        {
            triggerEffect.OnHit(this);
        }
    }

}
