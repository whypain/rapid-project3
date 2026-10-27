using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField inputField;

    public event Action<float> OnValueChanged;

    private float min, max;

    private void Awake()
    {
        min = slider.minValue;
        max = slider.maxValue;
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        inputField.onEndEdit.AddListener(OnInputFieldValueChanged);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        inputField.onEndEdit.RemoveListener(OnInputFieldValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        if (inputField == null) return;

        SetValue(value);
    }

    void OnInputFieldValueChanged(string value)
    {
        if (slider == null) return;

        if (float.TryParse(value, out float parsedValue))
        {
            SetValue(parsedValue);
        }
    }

    public void SetValue(float value)
    {
        if (slider == null || inputField == null) return;

        float clampedValue = Mathf.Clamp(value, min, max);

        slider.value = clampedValue;
        inputField.text = clampedValue.ToString("0.00");
        OnValueChanged?.Invoke(clampedValue);
    }

    public float GetValue()
    {
        return slider != null ? slider.value : 0f;
    }
}
