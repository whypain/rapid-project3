using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpSystem : MonoBehaviour, IResetable
{
    public static PowerUpSystem Instance { get; private set; }

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

    [Header("Inventory")]
    [Min(1)]
    [SerializeField] private int maxInventorySize = 9;
    private readonly List<string> inventory = new List<string>();

    private bool isActive;
    private float nextDropTime;
    private int activeWindCount;

    public bool HasStoredPower => inventory.Count > 0;

    public string StoredPowerType => inventory.Count > 0 ? inventory[0] : string.Empty;

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
        HandleInventoryInput();

        if (isActive)
        {
            CheckDropInterval();
        }
    }
    private void HandleInventoryInput()
    {
        if (Keyboard.current == null)return;

        // กด 1-9 เพื่อใช้ Power-Up ในช่องนั้นทันที
        for (int i = 0; i < Mathf.Min(maxInventorySize, 9); i++)
        {
            Key key = (Key)((int)Key.Digit1 + i);

            if (Keyboard.current[key].wasPressedThisFrame)
            {
                ActivatePowerAtIndex(i);
                break;
            }
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

        GameObject selectedPrefab = available[Random.Range(0, available.Count)];

        Vector2 min = Vector2.Min(spawnAreaMin, spawnAreaMax);
        Vector2 max = Vector2.Max(spawnAreaMin, spawnAreaMax);

        Vector2 spawnPosition = new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y));

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

        if (inventory.Count >= maxInventorySize)
        {
            Debug.LogWarning("Power-Up inventory is full!");
            return false;
        }
        inventory.Add(typeLabel);

        Debug.Log($"[INVENTORY] Collected {typeLabel}. " + $"Inventory: {inventory.Count}/{maxInventorySize}");
        return true;
    }
    public void ActivatePowerAtIndex(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return;

        Ball[] activeBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (activeBalls.Length == 0)
        {
            Debug.LogWarning("No Ball objects found.");
            return;
        }

        string typeToActivate = inventory[index];

        PowerUp powerUp = CreatePowerUp(typeToActivate);
        if (powerUp == null)
        {
            Debug.LogWarning($"Failed to create Power-Up of type: {typeToActivate}");
            return;
        }
        if (powerUp is Wind windPower)
        {
            windPower.ActivateWindBarrier(windBarrierObject);
        }

        Ball.ApplyPowerUp(powerUp);

        foreach (Ball ball in activeBalls)
        {
            if (ball != null)
                ball.UpdateParticleColor();
        }

        inventory.RemoveAt(index);
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