using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinConditionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel; // Victory screen panel
    public TextMeshProUGUI victoryMessageText;
    public Button playAgainButton;
    public Button mainMenuButton;
    
    [Header("Enemy Counter UI")]
    public TextMeshProUGUI enemyCounterText; // Shows "Enemies: X" during gameplay
    
    [Header("Settings")]
    public float victoryDelay = 1.5f; // Delay before showing victory screen
    public string mainMenuSceneName = "MainMenu";
    
    private static WinConditionManager instance;
    private int totalEnemies;
    private int enemiesKilled;
    private bool hasWon = false;

    public static WinConditionManager Instance
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
        // Count total enemies in scene
        CountEnemies();
        
        // Hide victory panel at start
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        // Update enemy counter UI
        UpdateEnemyCounter();
        
        // Make sure time is not paused
        Time.timeScale = 1f;
    }

    void CountEnemies()
    {
        // Find all enemies in scene (objects with EnemyHealth component)
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        totalEnemies = enemies.Length;
        enemiesKilled = 0;
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        
        // Update counter UI
        UpdateEnemyCounter();
        
        // Check win condition
        if (enemiesKilled >= totalEnemies && !hasWon)
        {
            Win();
        }
    }

    void UpdateEnemyCounter()
    {
        if (enemyCounterText != null)
        {
            int enemiesRemaining = totalEnemies - enemiesKilled;
            enemyCounterText.text = $"Enemies: {enemiesRemaining}";
        }
    }

    void Win()
    {
        hasWon = true;
        StartCoroutine(ShowVictoryScreen());
    }

    IEnumerator ShowVictoryScreen()
    {
        // Wait a moment before showing victory
        yield return new WaitForSeconds(victoryDelay);
        
        // Show victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        // Set victory message
        if (victoryMessageText != null)
        {
            victoryMessageText.text = "VICTORY!";
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }

    public void PlayAgain()
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
        
        // Temporary: reload current scene
        SceneManager.LoadScene("MenuScence");
    }

    public bool HasWon()
    {
        return hasWon;
    }

    public int GetEnemiesRemaining()
    {
        return totalEnemies - enemiesKilled;
    }
}
