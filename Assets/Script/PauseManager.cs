using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel; // The panel that shows when game is paused
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Settings")]
    public string mainMenuSceneName;
    
    private static PauseManager instance;
    private bool isPaused = false;

    public static PauseManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Hide pause menu at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        // Make sure game is not paused
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Don't pause if game is already over or won
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver())
        {
            return;
        }
        
        if (WinConditionManager.Instance != null && WinConditionManager.Instance.HasWon())
        {
            return;
        }
        
        isPaused = true;
        
        // Show pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Resume the game
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        // Resume time first
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // Resume time first
        Time.timeScale = 1f;
        
        // Temporary: reload current scene
        SceneManager.LoadScene("MenuScence");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
