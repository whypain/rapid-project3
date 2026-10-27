using UnityEngine;

public class PowerUpDrop : MonoBehaviour, IResetable
{
    private string powerTypeLabel = string.Empty;
    private bool isCollected;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.isTrigger = false;
    }

    // ฟังก์ชันตั้งค่าเริ่มต้นให้กับไอเทมดรอปชิ้นนี้
    public void Initialize(string typeLabel)
    {
        powerTypeLabel = typeLabel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCollected)
            return;

        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball == null)
            return;

        OnHit(ball, collision);
    }

    public void OnHit(Ball ball, Collision2D collision)
    {
        if (isCollected || ball == null)
            return;

        PowerUpSystem system = PowerUpSystem.Instance;

        if (system == null)
        {
            Debug.LogWarning("Cannot collect Power-Up: PowerUpSystem is missing.");
            return;
        }

        if (string.IsNullOrEmpty(powerTypeLabel))
        {
            Debug.LogWarning("PowerUpDrop has not been initialized.");
            return;
        }
        // if (!system.StoreCollectedPower(powerTypeLabel)) return;

        isCollected = true;
        system.StoreCollectedPower(powerTypeLabel);

        Destroy(gameObject);
    }

    public void Reset()
    {
        Destroy(gameObject);
    }
}

