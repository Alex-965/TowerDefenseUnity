using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Налаштування бази")]
    public int baseHealth = 20;
    public TextMeshProUGUI healthText;

    [Header("Економіка")]
    public int gold = 300;
    public TextMeshProUGUI goldText;

    [Header("UI Екрани та Хвилі")]
    public GameObject gameOverPanel;  
    public GameObject victoryPanel;   
    public TextMeshProUGUI waveText; 
    
    [Header("Меню Паузи")]
    public GameObject pauseMenuPanel;
    private bool isPaused = false;
    
    private int currentWave = 0;
    private int totalWaves = 10; 
    private int enemiesLeftInWave; 
    private bool isGameFinished = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateHealthUI();
        UpdateGoldUI();
        UpdateWaveUI();
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameFinished)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isGameFinished) return; 
        
        if (AudioManager.instance != null) AudioManager.instance.PlayClick();

        isPaused = !isPaused;

        if (isPaused)
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void StartNewWave(int waveNumber, int enemyCount)
    {
        currentWave = waveNumber;
        enemiesLeftInWave = enemyCount;
        UpdateWaveUI(); 
    }

    public void TakeDamage(int damage)
    {
        if (isGameFinished) return;
        
        baseHealth -= damage;
        UpdateHealthUI();

        // НОВЕ: ЗВУК КОЛИ ВОРОГ ДІЙШОВ ДО КІНЦЯ
        if (AudioManager.instance != null) AudioManager.instance.PlayBaseHit();
        
        if (baseHealth <= 0) GameOver();
        else EnemyDestroyed(); 
    }

    public void EnemyDestroyed()
    {
        if (isGameFinished) return;

        enemiesLeftInWave--;

        if (enemiesLeftInWave <= 0)
        {
            if (currentWave >= totalWaves)
            {
                WinGame();
            }
            else
            {
                LevelGenerator.instance.WaveCleared();
            }
        }
    }

    public void AddGold(int amount)
    {
        if (isGameFinished) return;
        gold += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (isGameFinished) return false;
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    void UpdateHealthUI() { if (healthText != null) healthText.text = "Здоров'я бази: " + baseHealth; }
    void UpdateGoldUI() { if (goldText != null) goldText.text = "Золото: " + gold; }
    void UpdateWaveUI() { if (waveText != null) waveText.text = "Хвиля: " + currentWave + " / " + totalWaves; }

    void GameOver()
    {
        isGameFinished = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 
        
        if (AudioManager.instance != null) AudioManager.instance.PlayLose();
    }

    void WinGame()
    {
        isGameFinished = true;
        if (victoryPanel != null) victoryPanel.SetActive(true); 
        Time.timeScale = 0f; 
        
        if (AudioManager.instance != null) AudioManager.instance.PlayWin();
    }

    public void RestartGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayClick();
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayClick();
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}