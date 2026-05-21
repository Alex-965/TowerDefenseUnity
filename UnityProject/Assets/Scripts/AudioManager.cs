using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Програвачі (Audio Sources)")]
    public AudioSource musicSource; 
    public AudioSource sfxSource;   

    [Header("Аудіокліпи (Звуки)")]
    public AudioClip backgroundMusic;
    public AudioClip clickSound;
    public AudioClip shootSound;
    public AudioClip buildSound;
    public AudioClip enemyDeathSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip sellSound; 
    
    // НОВЕ: Звук отримання шкоди базою (коли ворог дійшов)
    public AudioClip baseHitSound; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    // --- Функції для виклику звуків ---

    public void PlayClick()
    {
        if (clickSound != null) sfxSource.PlayOneShot(clickSound);
    }

    public void PlayShoot()
    {
        if (shootSound != null) sfxSource.PlayOneShot(shootSound);
    }

    public void PlayBuild()
    {
        if (buildSound != null) sfxSource.PlayOneShot(buildSound);
    }

    public void PlaySell()
    {
        if (sellSound != null) sfxSource.PlayOneShot(sellSound);
    }

    public void PlayEnemyDeath()
    {
        if (enemyDeathSound != null) sfxSource.PlayOneShot(enemyDeathSound, 0.6f); 
    }

    // НОВЕ: Відтворення звуку удару по базі
    public void PlayBaseHit()
    {
        // Робимо його трохи гучнішим (1.0f), щоб привернути увагу
        if (baseHitSound != null) sfxSource.PlayOneShot(baseHitSound, 1.0f); 
    }

    public void PlayWin()
    {
        if (musicSource != null) musicSource.Stop(); 
        if (winSound != null) sfxSource.PlayOneShot(winSound);
    }

    public void PlayLose()
    {
        if (musicSource != null) musicSource.Stop(); 
        if (loseSound != null) sfxSource.PlayOneShot(loseSound);
    }
}