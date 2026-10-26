using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup fadeBlack;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);

        fadeBlack.alpha = 1f;
        fadeBlack.interactable = false;
        fadeBlack.blocksRaycasts = true;
        Tween.Alpha(fadeBlack, 0f, 0.5f, useUnscaledTime: true).OnComplete(() =>
        {
            fadeBlack.interactable = false;
            fadeBlack.blocksRaycasts = false;
        });
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        GameState.Instance.SetInGame();
    }
}
