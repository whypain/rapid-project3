using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InputActionReference openSettingsAction;
    [SerializeField] private AudioMixer mixer;

    // [SerializeField] private SliderSettings sensSettings;
    [SerializeField] private SliderSettings masterVolumeSettings;
    [SerializeField] private SliderSettings sfxVolumeSettings;
    [SerializeField] private SliderSettings bgmVolumeSettings;

    private float beforeTimeScale;

    private void OnEnable()
    {
        openSettingsAction.action.performed += ToggleMenu;

        // sensSettings.OnValueChanged += OnSensChanged;
        masterVolumeSettings.OnValueChanged += OnMasterVolumeChanged;
        sfxVolumeSettings.OnValueChanged += OnSFXVolumeChanged;
        bgmVolumeSettings.OnValueChanged += OnBGMVolumeChanged;
    }

    private void OnDisable()
    {
        openSettingsAction.action.performed -= ToggleMenu;

        // sensSettings.OnValueChanged -= OnSensChanged;
        masterVolumeSettings.OnValueChanged -= OnMasterVolumeChanged;
        sfxVolumeSettings.OnValueChanged -= OnSFXVolumeChanged;
        bgmVolumeSettings.OnValueChanged -= OnBGMVolumeChanged;
    }

    private void Start()
    {
        SyncValues();
    }

    public void Open()
    {
        SyncValues();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        beforeTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Time.timeScale = beforeTimeScale;
        PlayerPrefs.Save();
    }

    private void SyncValues()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 50f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 50f);
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 50f);

        Debug.Log($"Loaded Master: {masterVolume}");

        masterVolumeSettings.SetValue(masterVolume);
        sfxVolumeSettings.SetValue(sfxVolume);
        bgmVolumeSettings.SetValue(bgmVolume);
    }

    private void OnMasterVolumeChanged(float value)
    {
        mixer.SetFloat("MasterVolume", LinearToDecibel(value * 0.01f));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        mixer.SetFloat("SFXVolume", LinearToDecibel(value * 0.01f));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void OnBGMVolumeChanged(float value)
    {
        mixer.SetFloat("BGMVolume", LinearToDecibel(value * 0.01f));
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (canvasGroup.alpha > 0f)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private float LinearToDecibel(float linear)
    {
        return Mathf.Log10(linear) * 20f;
    }

    private float DecibelToLinear(float decibel)
    {
        return Mathf.Pow(10f, decibel / 20f);
    }
}
