using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text damageTimer;
    [SerializeField] private Ball ball;

    private void Update()
    {
        if (ball != null)
        {
            float timeTilDestroy = DamageSystem.Instance.GetTimeTilDestroy(ball);
            if (timeTilDestroy > 0) damageTimer.text = $"{timeTilDestroy:F0}";
            else damageTimer.text = string.Empty;
        }
    }
}
