using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class Boss : MonoBehaviour, ICollisionEffect
{
    public static Boss Instance { get; private set; }
    public BossPhase[] Phases => phases;
    public float BaseHealth => baseHealth;

    public UnityEvent<string> OnSkillUsed;
    public UnityEvent<int> OnPhaseChanged;
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDead;

    [Header("Boss Settings")]
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float attackInterval = 100f;
    [SerializeField] private float delayOnPhaseChange = 1f;
    [SerializeField] private BossPhase[] phases;

    [Header("Debug")]
    [SerializeField] private float damageDebug;

    private Skill[] currentSkills;
    private Skill usingSkill;
    private int currentPhase;
    private float phaseChangeTimer;
    private float health;
    private float attackTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        health = baseHealth;
        currentPhase = 0;
    }

    [ContextMenu("Debug Take Damage")]
    private void DebugTakeDamage()
    {
        TakeDamage(damageDebug);
    }

    public void SetSkills(Skill[] skills)
    {
        currentSkills = skills;
        foreach (var skill in skills)
        {
            skill.Initialize(this);
        }
    }

    public void OnHit(Ball ball, Collision2D collision)
    {
        TakeDamage(ball.Attack);
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        OnHealthChanged?.Invoke(health);
        if (usingSkill != null && usingSkill.IsCancellable)
        {
            usingSkill.Cancel();
            usingSkill = null;
        }


        if (health <= 0)
        {
            Die();
            return;
        }

        CheckPhaseChange();
    } 

    private void CheckPhaseChange()
    {
        for (int i = 0; i < phases.Length; i++)
        {
            if (health >= phases[i].healthThreshold)
            {
                if (currentPhase != i)
                {
                    ChangePhase(i);
                }
                break;
            }
        }
    }

    private void ChangePhase(int phase)
    {
        Debug.Log($"Phase changed to {phase}");

        currentPhase = phase;
        phaseChangeTimer = delayOnPhaseChange;
        SetSkills(phases[phase].skills);
        OnPhaseChanged?.Invoke(phase);
    }

    private void OnPhaseActive()
    {
        BossPhase phase = phases[currentPhase];
        if (phase == null)
        {
            Debug.LogWarning($"No phase data defined for phase {currentPhase}");
            return;
        }

        phase.onPhaseStart?.Invoke();

        Skill openningSkill = phases[currentPhase].openningSkill;
        if (openningSkill == null)
        {
            Debug.LogWarning($"No opening skill defined for phase {currentPhase}");
            return;
        }

        if (usingSkill != null)
        {
            usingSkill.Cancel();
            usingSkill = null;
        }
        UseSkill(openningSkill);
    }

    private void UseSkill(Skill skill)
    {
        if (skill == null || usingSkill != null) return;

        usingSkill = skill;
        attackTimer = attackInterval;
        usingSkill.Use(() => { usingSkill = null; }).Forget();
        OnSkillUsed?.Invoke(skill.Name);
    }

    private void FindSkillAndUse()
    {
        if (currentSkills == null || currentSkills.Length == 0) return;
        if (usingSkill != null)
        {
            attackTimer = attackInterval;
            return;
        }

        Skill[] validSkills = Array.FindAll(currentSkills, skill => skill.CanUse());
        if (validSkills.Length == 0) return;

        Skill skillToUse = validSkills[UnityEngine.Random.Range(0, validSkills.Length)];
        UseSkill(skillToUse);
    }

    private void Die()
    {
        // Handle boss death logic here
        Debug.Log("Boss defeated!");
        OnDead?.Invoke();
    }

    private void Update()
    {
        if (phaseChangeTimer > 0)
        {
            phaseChangeTimer -= Time.deltaTime;
            if (phaseChangeTimer <= 0)
            {
                // Phase change delay is over, you can trigger any events or logic here
                Debug.Log($"Phase {currentPhase} is now active.");
                OnPhaseActive();
            }

            return;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackTimer = attackInterval;
            FindSkillAndUse();
        }
    }
}
