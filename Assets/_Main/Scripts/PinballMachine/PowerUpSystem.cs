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

    private string storedPowerType = string.Empty;
    private bool isActive;
    private float nextDropTime;
    private int activeWindCount;

    public bool HasStoredPower => !string.IsNullOrEmpty(storedPowerType);

    public string StoredPowerType => storedPowerType;

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
        if (Keyboard.current.spaceKey.wasPressedThisFrame && HasStoredPower)
        {
            ActivateStoredPowerForAllBalls();
        }
        // if(Keyboard.current.aKey.wasPressedThisFrame)
        // {
        //     OnBossPhase3Start();
        // }

        if (isActive)
        {
            CheckDropInterval();
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
        int availableCount = 0;

        foreach (GameObject prefab in availablePrefabs)
        {
            if (prefab != null)
                availableCount++;
        }

        if (availableCount == 0)
        {
            Debug.LogWarning("PowerUpSystem: No drop prefabs assigned.");
            return;
        }

        int selectedIndex = Random.Range(0, availableCount);
        GameObject selectedPrefab = null;

        for (int i = 0; i < availablePrefabs.Length; i++)
        {
            if (availablePrefabs[i] == null)
                continue;

            if (selectedIndex == 0)
            {
                selectedPrefab = availablePrefabs[i];
                break;
            }

            selectedIndex--;
        }

        Vector2 min = Vector2.Min(spawnAreaMin, spawnAreaMax);
        Vector2 max = Vector2.Max(spawnAreaMin, spawnAreaMax);

        Vector2 spawnPosition = new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y));

        GameObject drop = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity);

        PowerUpDrop dropComponent =
            drop.GetComponent<PowerUpDrop>();

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

    public void StoreCollectedPower(string typeLabel)
    {
        if (typeLabel != "Fire" &&
            typeLabel != "Electric" &&
            typeLabel != "Wind")
        {
            Debug.LogWarning($"Unknown Power-Up type: {typeLabel}");
            return;
        }

        storedPowerType = typeLabel;

        Debug.Log($"[SYSTEM] Collected {typeLabel}. " + "Press Space to activate.");
    }

    private void ActivateStoredPowerForAllBalls()
    {
        if (!HasStoredPower)
            return;

        Ball[] activeBalls = FindObjectsByType<Ball>();

        if (activeBalls.Length == 0)
        {
            Debug.LogWarning("No Ball objects found.");
            return;
        }

        string typeToActivate = storedPowerType;

        // เคลียร์กระเป๋าก่อนสร้างบัฟ
        storedPowerType = string.Empty;

        int activatedCount = 0;

        foreach (Ball ball in activeBalls)
        {
            if (ball == null)
                continue;

            PowerUp powerUp = CreatePowerUp(
                typeToActivate,
                ball);

            if (powerUp == null)
                continue;

            ball.ApplyPowerUp(powerUp);

            if (powerUp is Wind windPower)
            {
                windPower.ActivateWindBarrier(windBarrierObject);
            }

            activatedCount++;
        }

        Debug.Log($"[ACTIVATED] {typeToActivate} applied to " + $"{activatedCount} ball(s).");
    }

    private PowerUp CreatePowerUp(
        string typeLabel,
        Ball targetBall)
    {
        switch (typeLabel)
        {
            case "Fire":
                return new Fire(
                    this,
                    targetBall,
                    powerUpDuration,
                    fireExplosionPrefab);

            case "Electric":
                return new Electric(
                    this,
                    targetBall,
                    powerUpDuration);

            case "Wind":
                return new Wind(
                    this,
                    targetBall,
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
        storedPowerType = string.Empty;
        activeWindCount = 0;

        if (windBumper != null)
            windBumper.Deactivate();

        if (windBarrierObject != null)
            windBarrierObject.SetActive(false);
    }
}