using UnityEngine;
using UnityEngine.EventSystems; 

public class Tower : MonoBehaviour, IPointerClickHandler
{
    [Header("Характеристики")]
    public float range = 150f;
    public float fireRate = 1f;
    public float damage = 25f; 

    [Header("Обертання")]
    public float rotationOffset = -90f; 

    [Header("Стрільба")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Продаж та Інтерфейс")]
    public int towerPrice = 100;
    public GameObject hoverUI;
    public RectTransform rangeImage;
    public float visualMultiplier = 4f; 

    private Transform target;
    private float fireCountdown = 0f;
    private Cell currentCell; 

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (hoverUI != null) hoverUI.SetActive(false);
        if (rangeImage != null) rangeImage.sizeDelta = new Vector2(range * 2f, range * 2f);
    }

    public void SetCell(Cell cell)
    {
        currentCell = cell;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject bestTarget = null;
        int highestWaypoint = -1;
        float shortestDistanceToWaypoint = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (distanceToEnemy <= range)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    int enemyWaypoint = enemyScript.GetWaypointIndex();
                    float distanceToWP = enemyScript.GetDistanceToTarget();

                    if (enemyWaypoint > highestWaypoint)
                    {
                        highestWaypoint = enemyWaypoint;
                        shortestDistanceToWaypoint = distanceToWP;
                        bestTarget = enemy;
                    }
                    else if (enemyWaypoint == highestWaypoint)
                    {
                        if (distanceToWP < shortestDistanceToWaypoint)
                        {
                            shortestDistanceToWaypoint = distanceToWP;
                            bestTarget = enemy;
                        }
                    }
                }
            }
        }
        target = bestTarget != null ? bestTarget.transform : null;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        Vector3 scale = transform.localScale;

        if (direction.x > 0.05f) 
        {
            scale.x = Mathf.Abs(scale.x); 
            if (hoverUI != null) hoverUI.transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (direction.x < -0.05f) 
        {
            scale.x = -Mathf.Abs(scale.x); 
            if (hoverUI != null) hoverUI.transform.localScale = new Vector3(-1, 1, 1);
        }

        transform.localScale = scale;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject bulletGO = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        if (LevelGenerator.instance != null && LevelGenerator.instance.enemyParent != null)
            bulletGO.transform.SetParent(LevelGenerator.instance.enemyParent, true);
        else
            bulletGO.transform.SetParent(GetComponentInParent<Canvas>().transform, true);

        bulletGO.transform.SetAsLastSibling();
        
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Seek(target);
            bullet.damage = damage; 
        }

        if (AudioManager.instance != null) AudioManager.instance.PlayShoot();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayClick();

        if (hoverUI != null) 
        {
            hoverUI.SetActive(!hoverUI.activeSelf);
            if (rangeImage != null && hoverUI.activeSelf)
                rangeImage.sizeDelta = new Vector2(range * visualMultiplier, range * visualMultiplier);
        }
    }

    public void SellTower()
    {
        if (GameManager.instance != null) GameManager.instance.AddGold(towerPrice / 2);
        if (currentCell != null) currentCell.hasTower = false; 

        // НОВЕ: ЗВУК ПРОДАЖУ!
        if (AudioManager.instance != null) AudioManager.instance.PlaySell();
        
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}