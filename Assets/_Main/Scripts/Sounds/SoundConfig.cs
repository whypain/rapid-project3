using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundConfig", menuName = "Audio/Sound Config")]
public class SoundConfig : ScriptableObject
{
    [ShowAssetPreview(64, 64)]
    public AudioClip clip;

    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)] 
    public float volume = 1f;

    [MinMaxSlider(0.1f, 2f)] 
    public Vector2 randomPitchRange = new Vector2(0.8f, 1.2f);

    public SoundConfig(AudioClip clip, AudioMixerGroup mixerGroup, float volume, Vector2 randomPitchRange)
    {
        this.clip = clip;
        this.mixerGroup = mixerGroup;
        this.volume = volume;
        this.randomPitchRange = randomPitchRange;
    }
}
