using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Header("Рух")]
    public float baseSpeed = 150f; 
    private float currentSpeed;     
    private List<Transform> path; 
    private int currentWaypointIndex = 0; 

    [Header("Обертання")]
    public bool isInitiallyLeftFacing = true;

    [Header("Здоров'я та Нагорода")]
    public float maxHealth = 100f; 
    private float currentHealth;   
    public Image healthBarFill;    
    public int goldReward = 10; 
    public bool isImmuneToSlow = false; 

    private bool isSlowed = false; 
    private bool isDead = false; 

    public void Initialize(List<Transform> waypoints)
    {
        path = waypoints;
        if (path != null && path.Count > 0)
        {
            transform.position = path[0].position;
        }
        currentHealth = maxHealth; 
        currentSpeed = baseSpeed; 
        isDead = false; 
    }

    void Update()
    {
        if (path == null || path.Count == 0 || isDead) return; 

        Transform target = path[currentWaypointIndex];
        
        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);

        Vector3 direction = target.position - transform.position;
        Vector3 scale = transform.localScale;
        float multiplier = isInitiallyLeftFacing ? -1f : 1f;

        if (direction.x > 0.05f) 
        {
            scale.x = multiplier * Mathf.Abs(scale.x);
        }
        else if (direction.x < -0.05f) 
        {
            scale.x = -multiplier * Mathf.Abs(scale.x); 
        }
        
        transform.localScale = scale;

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentWaypointIndex++; 

            if (currentWaypointIndex >= path.Count)
            {
                if (isDead) return;
                isDead = true; 

                if (GameManager.instance != null)
                {
                    GameManager.instance.TakeDamage(1); 
                }
                Destroy(gameObject); 
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; 

        currentHealth -= amount;
        
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            isDead = true; 

            // ЗВУК: Смерть ворога
            if (AudioManager.instance != null) AudioManager.instance.PlayEnemyDeath();

            if (GameManager.instance != null)
            {
                GameManager.instance.AddGold(goldReward);
                GameManager.instance.EnemyDestroyed(); 
            }
            Destroy(gameObject); 
        }
    }

    public void ApplySlow(float amount, float duration)
    {
        if (isImmuneToSlow || isDead) return; 

        if (!isSlowed)
        {
            StartCoroutine(SlowRoutine(amount, duration));
        }
    }

    System.Collections.IEnumerator SlowRoutine(float amount, float duration)
    {
        isSlowed = true;
        currentSpeed = baseSpeed * (1f - amount);
        GetComponent<Image>().color = new Color(0.5f, 0.8f, 1f); 

        yield return new WaitForSeconds(duration);

        if (this != null && !isDead) 
        {
            currentSpeed = baseSpeed;
            GetComponent<Image>().color = Color.white; 
            isSlowed = false;
        }
    }

    public int GetWaypointIndex()
    {
        return currentWaypointIndex;
    }

    public float GetDistanceToTarget()
    {
        if (path == null || currentWaypointIndex >= path.Count) return 0f;
        return Vector3.Distance(transform.position, path[currentWaypointIndex].position);
    }
}