using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance; 

    private TowerData towerToBuild; 

    void Awake()
    {
        instance = this;
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        towerToBuild = tower;
        
        // Звук кліку при виборі вежі в магазині
        if (AudioManager.instance != null) AudioManager.instance.PlayClick();
    }

    public void BuildTowerOn(Cell cell)
    {
        if (towerToBuild == null) return;

        if (GameManager.instance.SpendGold(towerToBuild.cost))
        {
            GameObject newTower = Instantiate(towerToBuild.towerPrefab, cell.transform.position, Quaternion.identity);
            
            Tower towerScript = newTower.GetComponent<Tower>();
            if (towerScript != null)
            {
                towerScript.SetCell(cell); 
            }

            cell.hasTower = true; 
            newTower.transform.SetParent(cell.transform, true); 
            newTower.transform.localScale = Vector3.one; 

            // ЗВУК: Будівництво вежі!
            if (AudioManager.instance != null) AudioManager.instance.PlayBuild();
        }
    }
}