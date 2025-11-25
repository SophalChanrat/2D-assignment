using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel; // The panel that shows when player dies
    public Button tryAgainButton;
    public Button mainMenuButton;
    
    [Header("Settings")]
    public float gameOverDelay = 1f; // Delay before showing game over screen
    public string mainMenuSceneName; 
    
    private static GameOverManager instance;
    private bool isGameOver = false;

    public static GameOverManager Instance
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
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(TryAgain);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        // Resume time (in case it was paused)
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        StartCoroutine(ShowGameOverCoroutine());
    }

    IEnumerator ShowGameOverCoroutine()
    {
        // Wait a moment before showing game over
        yield return new WaitForSeconds(gameOverDelay);
        
        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Optional: Play game over sound here
    }

    public void TryAgain()
    {
        // Resume time
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // Resume time
        Time.timeScale = 1f;
        
        Debug.Log("Main Menu button pressed - Will go to: " + mainMenuSceneName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
