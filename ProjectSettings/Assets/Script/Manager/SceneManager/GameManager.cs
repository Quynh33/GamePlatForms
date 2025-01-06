using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;

    public static GameManager Instance { get; private set; }
    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    [SerializeField] Bench bench;
    public bool gameIsPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                Time.timeScale = 0;
                gameIsPaused = true;
                pauseMenu.FadeUIIn(fadeTime);
            }
            else
            {
                // Tắt pause menu
                UnpauseGame();
            }
        }
    }

    public void UnpauseGame()
    {
        pauseMenu.FadeUIOut(fadeTime);
        Time.timeScale = 1;
        gameIsPaused = false;
    }
    public void RespawnPlayer()
    {
        if(bench != null)

        {
            if(bench.interacted)
            {
                respawnPoint = bench.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }
        PlayerMovement.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerMovement.Instance.Respawn();
    }
}
