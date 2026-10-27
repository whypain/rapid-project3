using PrimeTween;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageDuration = 0.2f;

    [Header("Camera Shake")]
    [SerializeField] private float maxShakeStrength = 1f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Transform Shake")]
    [SerializeField] private Transform shakeTarget;
    [SerializeField] private float maxShakeStrengthTarget = 1f;
    [SerializeField] private float shakeStrengthTarget = 0.1f;
    [SerializeField] private float shakeDurationTarget = 0.2f;

    public void OnDamageTaken(float _, float damage)
    {
        if (camera == null || damageMaterial == null) return;
        if (damage <= 0) return;

        float shakeStrTarget = Mathf.Min(maxShakeStrengthTarget, shakeStrengthTarget * damage);
        float shakeStrCamera = Mathf.Min(maxShakeStrength, shakeStrength * damage);

        if (shakeTarget != null) Tween.ShakeLocalPosition(shakeTarget, shakeStrTarget * Vector3.one, shakeDurationTarget);

        Tween.ShakeCamera(camera, shakeStrCamera, shakeDuration);
        Tween.Custom(0f, 1f, damageDuration/2f, value =>
        {
            damageMaterial.SetFloat("_FXStrength", value);
        })
        .OnComplete(() =>
        {
            Tween.Custom(1f, 0f, damageDuration/2f, value =>
            {
                damageMaterial.SetFloat("_FXStrength", value);
            });
        });
    }
}