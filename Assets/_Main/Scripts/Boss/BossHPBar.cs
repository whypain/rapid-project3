using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private GameObject separatorPrefab;
    [SerializeField] private Transform separatorParent;

    void Start()
    {
        Boss boss = Boss.Instance;
        if (boss == null)
        {
            Debug.LogError("Boss instance not found!");
            return;
        }

        Initialize();
        boss.OnHealthChanged.AddListener(UpdateHPBar);
    }

    void UpdateHPBar(float currentHealth)
    {
        if (hpFill == null)
        {
            Debug.LogError("HP Fill Image is not assigned!");
            return;
        }

        float fillAmount = currentHealth / Boss.Instance.BaseHealth;
        hpFill.fillAmount = Mathf.Clamp01(fillAmount);
    }

    void Initialize()
    {
        if (separatorPrefab == null)
        {
            Debug.LogError("Separator prefab is not assigned!");
            return;
        }
        if (Boss.Instance == null)
        {
            Debug.LogError("Boss instance not found!");
            return;
        }

        Boss boss = Boss.Instance;
        BossPhase[] phases = boss.Phases;
        if (phases == null || phases.Length == 0)
        {
            Debug.LogError("Boss phases are not defined!");
            return;
        }

        foreach (Transform child in separatorParent)
        {
            Destroy(child.gameObject);
        }

        float hpWidth = hpFill.rectTransform.rect.width;
        Debug.Log($"width: {hpWidth}, health: {boss.BaseHealth}");
        for (int i = 0; i < phases.Length; i++)
        {
            BossPhase phase = phases[i];
            GameObject separator = Instantiate(separatorPrefab, separatorParent);
            separator.transform.localPosition = new Vector3(hpWidth * phase.healthThreshold / boss.BaseHealth, 0, 0);
            Debug.Log($"Separator for phase {i} at position: {separator.transform.localPosition.x}");
        }
    }

    void OnDestroy()
    {
        Boss boss = Boss.Instance;
        if (boss != null)
        {
            boss.OnHealthChanged.RemoveListener(UpdateHPBar);
        }
    }
}
