using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Панелі меню (Заглушки)")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // --- ФУНКЦІЯ ДЛЯ ЗВУКУ КЛІКУ ---
    public void PlayClickSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClick();
        }
    }

    // Запуск гри
    public void PlayGame()
    {
        PlayClickSound(); // Додаємо звук
        SceneManager.LoadScene("SampleScene"); // ВПИШИ ТОЧНУ НАЗВУ СЦЕНИ З ГРОЮ
    }

    // --- Налаштування ---
    public void OpenSettings()
    {
        PlayClickSound(); // Додаємо звук
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound(); // Додаємо звук
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- Автори (Credits) ---
    public void OpenCredits()
    {
        PlayClickSound(); // Додаємо звук
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        PlayClickSound(); // Додаємо звук
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // --- Вихід з гри ---
    public void QuitGame()
    {
        PlayClickSound(); // Додаємо звук
        Debug.Log("Гра закривається!");
        Application.Quit();
    }
}