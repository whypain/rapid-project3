using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public static event Action OnGameStarted;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameMenuUI;

    [SerializeField] private SoundConfig menuBgm;
    [SerializeField] private SoundConfig gameBgm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetGameMenu();
    }

    public void SetGameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        gameMenuUI.SetActive(false);
    }

    public void SetGameMenu()
    {
        if (ResetManager.Instance != null) ResetManager.Instance.Reset();
        gameOverUI.SetActive(false);
        gameMenuUI.SetActive(true);

        SoundManager.Instance.PlayMusic(menuBgm);
    }

    public void SetInGame()
    {
        Time.timeScale = 1f;
        gameOverUI.SetActive(false);
        gameMenuUI.SetActive(false);

        SoundManager.Instance.PlayMusic(gameBgm);
        OnGameStarted?.Invoke();
    }
}
