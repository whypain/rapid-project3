using PrimeTween;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeFrequency = 10f;

    public void Shake()
    {
        if (camera == null) return;

        Tween.ShakeCamera(camera, shakeStrength, shakeDuration, shakeFrequency);
    }
}
