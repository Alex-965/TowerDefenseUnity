using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target; 
    public float speed = 500f; 
    public float damage = 25f; 

    [Header("Обертання (Політ)")]
    // Встанови -45, 45, 90 або -90, щоб вирівняти стрілу кінчиком до ворога
    public float rotationOffset = 0f; 

    [Header("Спеціальні ефекти")]
    public bool isAoE = false;          // Для Мага (вибух по області)
    public float explosionRadius = 100f; // Радіус вибуху магії
    
    public bool isSlow = false;         // Для Заморожувача
    public float slowAmount = 0.5f;     // На скільки уповільнює (0.5 = на 50%)
    public float slowDuration = 2f;     // Тривалість заморозки в секундах

    [Header("Візуальні ефекти при влучанні")]
    public GameObject impactEffectPrefab; 
    public float effectDuration = 0.1f; // Швидкий спалах (0.1 сек)

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); 
            return;
        }

        // --- ЛОГІКА ОБЕРТАННЯ СНАРЯДУ ---
        Vector3 direction = target.position - transform.position;
        direction.z = 0; // 2D площина

        if (direction != Vector3.zero)
        {
            // Вираховуємо кут та додаємо зміщення (rotationOffset)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
        // --------------------------------

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 30f)
        {
            HitTarget(); 
        }
    }

    void HitTarget()
    {
        if (impactEffectPrefab != null)
        {
            GameObject effectIns = Instantiate(impactEffectPrefab, target.position, Quaternion.identity);
            
            // Кладемо ефект до ворогів, щоб він не ховався під дорогою
            effectIns.transform.SetParent(target.parent, true);
            effectIns.transform.SetAsLastSibling();

            if (isAoE)
            {
                float scaleSize = (explosionRadius * 2f) / 100f; 
                effectIns.transform.localScale = new Vector3(scaleSize, scaleSize, 1f);
            }
            else
            {
                effectIns.transform.localScale = Vector3.one;
            }

            Destroy(effectIns, effectDuration);
        }

        if (isAoE)
        {
            Explode(); 
        }
        else
        {
            DamageEnemy(target.gameObject);
        }

        Destroy(gameObject);
    }

    void DamageEnemy(GameObject enemy)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);

            if (isSlow)
            {
                enemyComponent.ApplySlow(slowAmount, slowDuration);
            }
        }
    }

    void Explode()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (distanceToEnemy <= explosionRadius)
            {
                DamageEnemy(enemy);
            }
        }
    }
}