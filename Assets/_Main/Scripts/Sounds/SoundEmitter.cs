using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource audioSource;
    private IObjectPool<SoundEmitter> poolReference;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Call this right after creating or fetching from pool
    public void SetPool(IObjectPool<SoundEmitter> pool)
    {
        poolReference = pool;
    }

    public void PlaySound(AudioClip clip, Vector3 position, Vector2 randomPitchRange, AudioMixerGroup mixerGroup, float volume = 1f)
    {
        transform.position = position;
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        audioSource.volume = volume;
        audioSource.Play();

        // Release back to pool when done playing
        StartCoroutine(ReturnToPoolWhenFinished());
    }

    private IEnumerator ReturnToPoolWhenFinished()
    {
        // Wait until the audio clip finishes playing
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Return to pool (or Destroy if pool reference missing)
        if (poolReference != null)
        {
            poolReference.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopAndReset()
    {
        StopAllCoroutines();
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}