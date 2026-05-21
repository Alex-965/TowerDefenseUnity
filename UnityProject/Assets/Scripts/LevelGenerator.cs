using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro; // НОВЕ: Потрібно для роботи з текстом на кнопці

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    [Header("Налаштування поля")]
    public int columns = 12;
    public int rows = 8;
    
    [Header("Шлях ворогів")]
    public List<int> pathCells;
    public List<Transform> waypoints = new List<Transform>();
    
    [Header("Префаби ворогів (0-Гоблін, 1-Орк, 2-Привид)")]
    public GameObject[] enemyPrefabs; 
    public Transform enemyParent;

    [Header("Шаблони")]
    public GameObject cellPrefab;
    public Transform boardParent;

    [Header("Керування хвилями (UI)")]
    public GameObject startWaveButton;       // Сама кнопка
    public TextMeshProUGUI startWaveButtonText; // Текст на ній (для таймера)

    // Стан хвиль за бюджетом
    private int currentWaveNumber = 0;
    private List<GameObject> enemySpawnQueue = new List<GameObject>(); 
    private int totalEnemiesInCurrentWave = 0;
    
    private Coroutine waveCountdownCoroutine; // Змінна для зберігання таймера

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateGrid();
        
        // ЗАМІСТЬ Invoke запускаємо підготовку першої хвилі (чекаємо кліку)
        PrepareNextWave();
    }

    void GenerateGrid()
    {
        int totalCells = columns * rows;
        Cell[] allCreatedCells = new Cell[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            GameObject newCell = Instantiate(cellPrefab);
            newCell.transform.SetParent(boardParent, false);
            allCreatedCells[i] = newCell.GetComponent<Cell>();
        }

        foreach (int cellIndex in pathCells)
        {
            if (cellIndex >= 0 && cellIndex < totalCells) 
            {
                allCreatedCells[cellIndex].SetAsPath();
                waypoints.Add(allCreatedCells[cellIndex].transform);
            }
        }
    }

    // НОВЕ: Логіка підготовки та очікування
    public void PrepareNextWave()
    {
        if (startWaveButton != null) startWaveButton.SetActive(true); // Показуємо кнопку

        if (currentWaveNumber == 0)
        {
            // Якщо це сама перша хвиля - таймера немає, чекаємо нескінченно
            if (startWaveButtonText != null) startWaveButtonText.text = "ПОЧАТИ БІЙ";
        }
        else
        {
            // Для всіх наступних хвиль запускаємо 10-секундний таймер
            waveCountdownCoroutine = StartCoroutine(WaveCountdownRoutine());
        }
    }

    // НОВЕ: Таймер відліку часу до наступної хвилі
    IEnumerator WaveCountdownRoutine()
    {
        int timeLeft = 10;
        while (timeLeft > 0)
        {
            if (startWaveButtonText != null)
                startWaveButtonText.text = $"НАСТУПНА ХВИЛЯ ({timeLeft})";
                
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // Коли 10 секунд пройшли - примусово запускаємо хвилю
        ForceStartWave();
    }

    // НОВЕ: Ця функція викликається при натисканні на кнопку в грі!
    public void ForceStartWave()
    {
        // Якщо таймер ще йшов, зупиняємо його, бо гравець натиснув кнопку
        if (waveCountdownCoroutine != null)
        {
            StopCoroutine(waveCountdownCoroutine);
            waveCountdownCoroutine = null;
        }

        // Ховаємо кнопку, поки йде бій
        if (startWaveButton != null) startWaveButton.SetActive(false);

        // Запускаємо генерацію
        StartNextWave();
    }

    void StartNextWave()
    {
        currentWaveNumber++;
        enemySpawnQueue.Clear();

        int waveBudget = 20 + (currentWaveNumber * 30); 
        int maxEnemiesLimit = 50;
        int currentEnemiesCount = 0;

        while (waveBudget >= 10 && currentEnemiesCount < maxEnemiesLimit)
        {
            int maxAvailableIndex = 1; 
            if (currentWaveNumber >= 3 && currentWaveNumber < 5) maxAvailableIndex = 2; 
            else if (currentWaveNumber >= 5) maxAvailableIndex = 3; 

            int randomIndex = Random.Range(0, maxAvailableIndex);
            GameObject candidatePrefab = enemyPrefabs[randomIndex];
            int cost = candidatePrefab.GetComponent<Enemy>().goldReward; 

            if (waveBudget >= cost)
            {
                enemySpawnQueue.Add(candidatePrefab);
                waveBudget -= cost;
                currentEnemiesCount++;
            }
            else if (waveBudget >= 10)
            {
                enemySpawnQueue.Add(enemyPrefabs[0]);
                waveBudget -= 10;
                currentEnemiesCount++;
            }
        }

        totalEnemiesInCurrentWave = enemySpawnQueue.Count;

        if (GameManager.instance != null)
        {
            GameManager.instance.StartNewWave(currentWaveNumber, totalEnemiesInCurrentWave);
        }

        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        float spawnInterval = 1.6f - (currentWaveNumber * 0.1f);
        if (spawnInterval < 0.4f) spawnInterval = 0.4f; 

        while (enemySpawnQueue.Count > 0)
        {
            if (waypoints.Count > 0)
            {
                GameObject prefabToSpawn = enemySpawnQueue[0];
                enemySpawnQueue.RemoveAt(0);

                GameObject newEnemy = Instantiate(prefabToSpawn, enemyParent);
                newEnemy.GetComponent<Enemy>().Initialize(waypoints);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void WaveCleared()
    {
        // ЗАМІСТЬ старого Invoke викликаємо нову підготовку
        PrepareNextWave();
    }
}