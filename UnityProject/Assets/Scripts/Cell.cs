using UnityEngine;
using UnityEngine.UI; // Обов'язково для роботи з UI Image

// Цей рядок автоматично додасть компонент Button (Кнопка) до клітинки!
[RequireComponent(typeof(Button))] 
public class Cell : MonoBehaviour
{
    public bool isPath = false;
    public bool hasTower = false; // Чи стоїть тут вже вежа?

    [Header("Налаштування спрайтів")]
    public Sprite defaultGrassSprite; // Звичайний спрайт трави (для фону)
    public Sprite pathRoadSprite;      // Спрайт дороги (для шляху ворогів)

    private Image cellImage; // Посилання на Image цієї клітинки

    void Awake()
    {
        cellImage = GetComponent<Image>();
    }

    void Start()
    {
        // Кажемо кнопці: "Коли на тебе клікнуть, запусти функцію OnCellClicked"
        GetComponent<Button>().onClick.AddListener(OnCellClicked);

        // На старті встановлюємо спрайт трави
        if (!isPath && defaultGrassSprite != null)
        {
            cellImage.sprite = defaultGrassSprite;
        }
    }

    // Ця функція перетворює клітинку на дорогу
    public void SetAsPath()
    {
        isPath = true;
        
        // Встановлюємо спрайт дороги!
        if (cellImage == null) cellImage = GetComponent<Image>(); // Про всяк випадок
        if (pathRoadSprite != null)
        {
            cellImage.sprite = pathRoadSprite;
        }
        else
        {
            cellImage.color = new Color(0.6f, 0.4f, 0.2f); // Запасний колір
        }
        
        // Дорога не може бути натиснута, тому вимикаємо кнопку
        GetComponent<Button>().interactable = false; 
    }

    // Ця функція спрацює при кліку
    void OnCellClicked()
    {
        // Перевіряємо, чи є вже вежа
        if (hasTower)
        {
            Debug.Log("Тут вже стоїть вежа!");
            return; 
        }

        // Перевіряємо, чи це не дорога
        if (isPath)
        {
            Debug.Log("Тут не можна будувати, це дорога!");
            return;
        }

        // Якщо все ок - просимо BuildManager побудувати вежу!
        if (BuildManager.instance != null)
        {
            BuildManager.instance.BuildTowerOn(this);
        }
    }
}