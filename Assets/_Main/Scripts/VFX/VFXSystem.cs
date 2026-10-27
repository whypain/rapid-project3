using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum VFXType
{
    BumperExplosion,
    BossExplosion,
}

[System.Serializable]
public class VFX
{
    public VFXType Type;
    public float Duration;
    public GameObject Prefab;
    public Vector3 Offset;
}

public class VFXSystem : MonoBehaviour
{
    public static VFXSystem Instance { get; private set; }

    [SerializeField] private List<VFX> vfxs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask PlayVFX(VFXType type, Vector3 position = default)
    {
        VFX vfx = GetVFX(type);
        if (vfx != null)
        {
            var instance = Instantiate(vfx.Prefab, position + vfx.Offset, Quaternion.identity);
            Destroy(instance, vfx.Duration);

            await UniTask.Delay(System.TimeSpan.FromSeconds(vfx.Duration));
        }
        else
        {
            Debug.LogWarning($"VFX prefab for type {type} is not assigned.");
        }
    }

    private VFX GetVFX(VFXType type)
    {
        foreach (var vfx in vfxs)
        {
            if (vfx.Type == type)
                return vfx;
        }
        return null;
    }
}
