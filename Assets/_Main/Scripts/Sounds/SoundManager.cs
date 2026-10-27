using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource bgmSource;

    [Header("Sound Config")]
    [SerializeField] private float minDelayBetweenSounds = 0.1f;

    [Header("Pool Config")]
    [SerializeField] private SoundEmitter soundEmitterPrefab;
    [SerializeField] private int defaultPoolSize = 5;
    [SerializeField] private int maxPoolSize = 40;

    [Header("Fade Config")]
    [SerializeField] private float fadeDuration = 1f;

    private IObjectPool<SoundEmitter> soundPool;

    private Dictionary<SoundConfig, float> nextSoundTime = new Dictionary<SoundConfig, float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        soundPool = new ObjectPool<SoundEmitter>(
            createFunc: CreateEmitter,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
    }

    private SoundEmitter CreateEmitter()
    {
        SoundEmitter emitter = Instantiate(soundEmitterPrefab, transform);
        emitter.SetPool(soundPool);
        return emitter;
    }

    private void OnGetFromPool(SoundEmitter emitter)
    {
        emitter.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(SoundEmitter emitter)
    {
        emitter.StopAndReset();
        emitter.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(SoundEmitter emitter)
    {
        Destroy(emitter.gameObject);
    }


    /// <summary>
    /// randomPitchRange: x = min pitch, y = max pitch - default is (1, 1) for no random pitch
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="randomPitchRange"></param>
    /// <param name="position"></param>
    public void PlaySoundFromPool(AudioClip clip, Vector2 randomPitchRange, AudioMixerGroup mixerGroup = null, Vector3 position = default, float volume = 1f)
    {
        if (clip == null) return;

        SoundEmitter emitter = soundPool.Get();
        emitter.PlaySound(clip, position, randomPitchRange, mixerGroup, volume);
    }

    public void PlaySoundFromPool(SoundConfig config, Vector3 position = default)
    {
        if (config == null || config.clip == null) return;

        if (nextSoundTime.TryGetValue(config, out float nextTime))
        {
            if (Time.time < nextTime) return;
        }

        PlaySoundFromPool(config.clip, config.randomPitchRange, config.mixerGroup, position, config.volume);
        nextSoundTime[config] = Time.time + minDelayBetweenSounds;
    }

    public async UniTask PlayDelayedSoundFromPool(SoundConfig config, float delay, Vector3 position = default)
    {
        if (config == null || config.clip == null) return;
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        PlaySoundFromPool(config.clip, config.randomPitchRange, config.mixerGroup, position, config.volume);
    }

    public void PlaySoundFromPool(SoundConfig[] configs, Vector3 position = default)
    {
        if (configs == null || configs.Length == 0) return;
        SoundConfig config = configs[UnityEngine.Random.Range(0, configs.Length)];
        PlaySoundFromPool(config, position);
    }

    public void PlayMusic(SoundConfig sc)
    {
        bgmSource.clip = sc.clip;
        bgmSource.volume = 0f;
        bgmSource.loop = true;
        bgmSource.Play();
        FadeVolume(0f, sc.volume);
    }

    public void StopMusic(Action onComplete = null)
    {
        FadeVolume(bgmSource.volume, 0f, () =>
        {
            bgmSource.Stop();
            onComplete?.Invoke();
        });
    }

    public void FadeVolume(float from, float to, Action onComplete = null)
    {
        Tween.Custom(from, to, fadeDuration, onValueChange: (value) => bgmSource.volume = value, ease: Ease.Linear, useUnscaledTime: true).OnComplete(onComplete);
    }

    // Test
    [Header("Test")]
    [SerializeField]
    private SoundConfig testSoundConfig;

    [Button]
    private void TestPlaySound()
    {
        PlaySoundFromPool(testSoundConfig.clip, testSoundConfig.randomPitchRange, testSoundConfig.mixerGroup, Vector3.zero);
    }
}