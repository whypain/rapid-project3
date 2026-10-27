using System.Collections.Generic;
using UnityEngine;

public class PowerUpUIManager : MonoBehaviour, IResetable
{
    [SerializeField] private Transform powerUpContainer;
    [SerializeField] private PowerUpUI powerUpUIPrefab;

    [SerializeField] private string[] buttonPrompts = { "K", "L", ";" };

    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite electricIcon;
    [SerializeField] private Sprite windIcon;
    [SerializeField] private Sprite placeholderIcon;

    private List<PowerUpUI> powerUpUIList = new List<PowerUpUI>();

    private void OnEnable()
    {
        PowerUpSystem.OnInventoryChanged += UpdatePowerUpUI;
    }

    private void OnDisable()
    {
        PowerUpSystem.OnInventoryChanged -= UpdatePowerUpUI;
    }

    private void Start()
    {
        Prefill();
    }

    private void Prefill()
    {
        if (powerUpUIList.Count >= 3) return;

        for(int i = 0; i < 3; i++)
        {
            PowerUpUI powerUpUI = Instantiate(powerUpUIPrefab, powerUpContainer);

            if (powerUpUI != null)
            {
                powerUpUI.SetPowerUpInfo("", 0, buttonPrompts[i]);
                powerUpUIList.Add(powerUpUI);
            }
        }
    }

    private void UpdatePowerUpUI()
    {
        // Create new UI elements for each power-up in the inventory
        foreach (var kvp in PowerUpSystem.inventory)
        {
            PowerUpUI existing = powerUpUIList.Find(ui => ui.PowerUpName == kvp.Key);
            if (existing != null)
            {
                existing.SetPowerUpInfo(kvp.Key, kvp.Value, buttonPrompts[powerUpUIList.IndexOf(existing)]);
                existing.SetIcon(GetIcon(kvp.Key));
                continue;
            }

            PowerUpUI available = powerUpUIList.Find(ui => string.IsNullOrEmpty(ui.PowerUpName));
            if (available != null)
            {
                available.SetPowerUpInfo(kvp.Key, kvp.Value, buttonPrompts[powerUpUIList.IndexOf(available)]);
                available.SetIcon(GetIcon(kvp.Key));
                continue;
            }

            Debug.LogWarning($"[PowerUpUIManager] No available UI slot for power-up: {kvp.Key}");
        }

        List<PowerUpUI> toEmpty = powerUpUIList.FindAll(ui => !PowerUpSystem.inventory.ContainsKey(ui.PowerUpName));
        foreach (var ui in toEmpty)
        {
            ui.SetPowerUpInfo("", 0, buttonPrompts[powerUpUIList.IndexOf(ui)]);
            ui.SetIcon(placeholderIcon);
        }
    }

    private Sprite GetIcon(string skillName)
    {
        return skillName switch
        {
            "Fire" => fireIcon,
            "Electric" => electricIcon,
            "Wind" => windIcon,
            _ => null
        };
    }

    public void Reset()
    {
        foreach (Transform child in powerUpContainer)
        {
            Destroy(child.gameObject);
        }

        powerUpUIList.Clear();

        Prefill();
    }
}
