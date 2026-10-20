using UnityEngine;

public class ForceBumper : MonoBehaviour, ICollisionEffect
{
    [SerializeField] private float forceMagnitude = 10f;
    [SerializeField] private Collider2D bumperCollider;

    private bool isActive;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void OnHit(Ball ball, Collision2D collision)
    {
        if (!isActive) return;

        ContactPoint2D contact = new ContactPoint2D();
        foreach (ContactPoint2D c in collision.contacts)
        {
            if (c.collider != bumperCollider) continue;

            contact = c;
            break;
        }
        if (contact.collider == null)
        {
            Debug.LogWarning("No contact point found for the bumper collision.");
            return;
        }

        Vector2 forceDirection = contact.normal;
        ball.AddForce(forceDirection * forceMagnitude);

        // Debug.DrawRay(contact.point, forceDirection, Color.red, 2f);
        // Debug.Break();
    }
}
