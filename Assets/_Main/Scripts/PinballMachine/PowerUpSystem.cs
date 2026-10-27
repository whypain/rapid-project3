using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpSystem : MonoBehaviour, IResetable
{
    public static PowerUpSystem Instance { get; private set; }
    public static event Action OnPowerUpChanged;
    public static event Action OnInventoryChanged;

    [Header("Drop Settings")] 
    [SerializeField] private float dropInterval = 10f;

    [Header("Power-Up Drop Prefabs")]
    [SerializeField] private GameObject fireDropPrefab;
    [SerializeField] private GameObject electricDropPrefab;
    [SerializeField] private GameObject windDropPrefab;

    [Header("Wind References")]
    [SerializeField] private GameObject windBarrierObject;
    [SerializeField] private ForceBumper windBumper;

    [Header("Fire References")]
    [SerializeField] private GameObject fireExplosionPrefab;

    [Header("Phase 3 Spawn Area")]
    public Vector2 spawnAreaMin = new Vector2(-7f, -4f);
    public Vector2 spawnAreaMax = new Vector2(7f, 4f);

    [Header("Power-Up Settings")]
    [Min(0.1f)]
    [SerializeField] private float powerUpDuration = 7f;

    public static Dictionary<string, int> inventory = new Dictionary<string, int>();
    public static List<PowerUp> activePowerUps = new List<PowerUp>();

    [Header("Input")]
    [SerializeField] private InputActionReference ability1;
    [SerializeField] private InputActionReference ability2;
    [SerializeField] private InputActionReference ability3;

    private bool isActive;
    private float nextDropTime;
    private int activeWindCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (windBarrierObject != null)
            windBarrierObject.SetActive(false);
    }

    private void Update()
    {
        HandleAbilityInput();

        if (isActive)
        {
            CheckDropInterval();
        }

        UpdateActivePowerUps();
    }

    private void UpdateActivePowerUps()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            PowerUp powerUp = activePowerUps[i];
            powerUp.Update(deltaTime);

            if (powerUp.IsExpired())
            {
                powerUp.OnExpire();
                activePowerUps.RemoveAt(i);
                OnPowerUpChanged?.Invoke();
            }
        }
    }

    private void HandleAbilityInput()
    {
        if (ability1 != null && ability1.action != null && ability1.action.triggered)
        {
            ActivatePowerAtIndex(0);
        }
        else if (ability2 != null && ability2.action != null && ability2.action.triggered)
        {
            ActivatePowerAtIndex(1);
        }
        else if (ability3 != null && ability3.action != null && ability3.action.triggered)
        {
            ActivatePowerAtIndex(2);
        }
    }

    private void CheckDropInterval()
    {
        if (Time.time >= nextDropTime)
        {
            SpawnRandomPowerUp();
            nextDropTime = Time.time + dropInterval;
        }
    }

    public void OnBossPhase3Start()
    {
        if (isActive)
            return;

        isActive = true;
        nextDropTime = Time.time + dropInterval;
    }

    public void SpawnRandomPowerUp()
    {
        GameObject[] availablePrefabs =
        {
            fireDropPrefab,
            electricDropPrefab,
            windDropPrefab
        };

        // สุ่มเฉพาะ Prefab ที่กำหนดไว้แล้ว
        List<GameObject> available = new List<GameObject>();

        foreach (GameObject prefab in availablePrefabs)
        {
            if (prefab != null)
                available.Add(prefab);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("PowerUpSystem: No drop prefabs assigned.");
            return;
        }

        GameObject selectedPrefab = available[UnityEngine.Random.Range(0, available.Count)];

        Vector2 min = Vector2.Min(spawnAreaMin, spawnAreaMax);
        Vector2 max = Vector2.Max(spawnAreaMin, spawnAreaMax);

        Vector2 spawnPosition = new Vector2(
            UnityEngine.Random.Range(min.x, max.x),
            UnityEngine.Random.Range(min.y, max.y));

        GameObject drop = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity);

        PowerUpDrop dropComponent = drop.GetComponent<PowerUpDrop>();

        if (dropComponent == null)
        {
            Debug.LogError($"Drop prefab {selectedPrefab.name} " + "does not have PowerUpDrop.");
            Destroy(drop);
            return;
        }

        string typeLabel;

        if (selectedPrefab == fireDropPrefab)
            typeLabel = "Fire";
        else if (selectedPrefab == electricDropPrefab)
            typeLabel = "Electric";
        else
            typeLabel = "Wind";

        dropComponent.Initialize(typeLabel);
    }

    public bool StoreCollectedPower(string typeLabel)
    {
        if (typeLabel != "Fire" &&
            typeLabel != "Electric" &&
            typeLabel != "Wind")
        {
            Debug.LogWarning($"Unknown Power-Up type: {typeLabel}");
            return false;
        }

        Debug.Log($"<color=green>Collected Power-Up: {typeLabel}</color>");
        AddToInventory(typeLabel);
        return true;
    }

    private void AddToInventory(string typeLabel)
    {
        if (inventory.ContainsKey(typeLabel))
        {
            inventory[typeLabel]++;
        }
        else
        {
            inventory[typeLabel] = 1;
        }
        OnInventoryChanged?.Invoke();
    }

    private void RemoveFromInventory(string typeLabel)
    {
        if (inventory.ContainsKey(typeLabel) && inventory[typeLabel] > 0)
        {
            inventory[typeLabel]--;
            if (inventory[typeLabel] <= 0)
            {
                inventory.Remove(typeLabel);
            }

            OnInventoryChanged?.Invoke();
        }
    }

    public void ActivatePowerAtIndex(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return;

        // Ball[] activeBalls = FindObjectsByType<Ball>();

        // if (activeBalls.Length == 0)
        // {
        //     Debug.LogWarning("No Ball objects found.");
        //     return;
        // }

        string typeToActivate = inventory.ElementAt(index).Key;
        PowerUp powerUp = CreatePowerUp(typeToActivate);
        activePowerUps.Add(powerUp);

        if (powerUp == null)
        {
            Debug.LogWarning($"Failed to create Power-Up of type: {typeToActivate}");
            return;
        }
        if (powerUp is Wind windPower)
        {
            windPower.ActivateWindBarrier(windBarrierObject);
        }

        // Ball.ApplyPowerUp(powerUp);

        // foreach (Ball ball in activeBalls)
        // {
        //     if (ball != null)
        //         ball.UpdateParticleColor();
        // }

        RemoveFromInventory(typeToActivate);
        OnPowerUpChanged?.Invoke();
    }

    private PowerUp CreatePowerUp(string typeLabel)
    {
        switch (typeLabel)
        {
            case "Fire":
                return new Fire(
                    this,
                    powerUpDuration,
                    fireExplosionPrefab);

            case "Electric":
                return new Electric(
                    this,
                    powerUpDuration);

            case "Wind":
                return new Wind(
                    this,
                    powerUpDuration);

            default:
                return null;
        }
    }

    public void AcquireWindBarrier()
    {
        activeWindCount++;

        if (activeWindCount == 1)
        {
            if (windBarrierObject != null)
                windBarrierObject.SetActive(true);

            if (windBumper != null)
                windBumper.Activate();
        }
    }

    public void ReleaseWindBarrier()
    {
        activeWindCount = Mathf.Max(0, activeWindCount - 1);

        if (activeWindCount == 0)
        {
            if (windBumper != null)
                windBumper.Deactivate();

            if (windBarrierObject != null)
                windBarrierObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
        activeWindCount = 0;

        if (windBumper != null)
            windBumper.Deactivate();

        if (windBarrierObject != null)
            windBarrierObject.SetActive(false);
    }

    public void Reset()
    {
        isActive = false;
        inventory.Clear();
        activeWindCount = 0;

        if (windBumper != null)
            windBumper.Deactivate();

        if (windBarrierObject != null)
            windBarrierObject.SetActive(false);

        activePowerUps.Clear();
        OnPowerUpChanged?.Invoke();
        OnInventoryChanged?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 min = Vector2.Min(spawnAreaMin, spawnAreaMax);
        Vector2 max = Vector2.Max(spawnAreaMin, spawnAreaMax);
        Vector2 size = max - min;
        Vector2 center = (min + max) / 2f;

        Gizmos.DrawWireCube(center, size);
    }
}