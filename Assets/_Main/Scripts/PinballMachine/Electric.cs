using UnityEngine;

public class Electric : PowerUp
{
    [SerializeField] private float minimumSpeed = 8f;
    public Electric(
        PowerUpSystem system,
        float durationSeconds)
        : base(system, durationSeconds)
    {
    }

    public override string Name => "electric";

    public override void OnHit(Ball ball, Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bumper")) return;
        Rigidbody2D rb = ball.Rigidbody;
        if (rb == null) return;

        GameObject[] allBumpers =
            GameObject.FindGameObjectsWithTag("Bumper");

        GameObject targetBumper = null;
        float closestDistance = Mathf.Infinity;
        Vector3 ballPosition = ball.transform.position;

        foreach (GameObject bumper in allBumpers)
        {
            if (bumper == collision.gameObject)
                continue;

            float distance = Vector2.Distance(
                ballPosition,
                bumper.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetBumper = bumper;
            }
        }

        if (targetBumper == null)
            return;

        Vector2 direction =
            (Vector2)(targetBumper.transform.position - ballPosition);

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.Normalize();

        float speed = Mathf.Max(
            rb.linearVelocity.magnitude,
            minimumSpeed);

        rb.linearVelocity = direction * speed;
    }
}
