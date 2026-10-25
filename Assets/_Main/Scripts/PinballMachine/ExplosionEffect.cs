using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField, Min(0.1f)] private float radius = 2f;
    [SerializeField, Min(0f)] private float lifetime = 1f;

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius);

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            // if (hit.CompareTag("Bumper"))
            // {
                // Destroy(hit.gameObject);
            // }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
