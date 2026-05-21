using UnityEngine;

// Цей рядок додасть нове меню в Unity, щоб ми могли створювати ці "картки" прямо в папці!
[CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName; // Назва вежі
    public int cost; // Ціна вежі
    public GameObject towerPrefab; // Префаб, який будемо будувати
}