using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public string Name => skillName;
    public bool IsCancellable;

    [SerializeField] private string skillName;
    [SerializeField] private float cooldown;
    [SerializeField] private GameObject visual;

    protected Boss owner;
    private float lastUsedTime;

    public void Initialize(Boss boss)
    {
        owner = boss;
    }

    protected virtual async UniTask OnUse()
    {
        // to shut up the warning, but this method should be overridden in derived classes
        await UniTask.Yield();

        throw new NotImplementedException("OnUse method must be implemented in derived classes.");
    }

    public virtual bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }
    public virtual async UniTask Use(Action callback = null)
    {
        lastUsedTime = Time.time;
        if (visual != null) visual.SetActive(true);

        await OnUse();

        if (visual != null) visual.SetActive(false);
        callback?.Invoke();
    }

    public virtual void Cancel()
    {
        visual?.SetActive(false);
    }
}
