
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class L04_ShopGameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text feedbackText;

    [Header("Interaction Settings")]
    public float interactRange = 10f;

    private class ShopItem
    {
        public string spanish;
        public string english;
        public string shopId;

        public ShopItem(string spanish, string english, string shopId)
        {
            this.spanish = spanish;
            this.english = english;
            this.shopId = shopId;
        }
    }

    private List<ShopItem> allItems = new List<ShopItem>();
    private List<ShopItem> questions = new List<ShopItem>();
    private List<L04_ShopTarget> shops = new List<L04_ShopTarget>();

    private int currentIndex = 0;
    private bool levelFinished = false;

    void Start()
    {
        // Fruit shop
        AddItem("La Manzana", "Apple", "fruit");
        AddItem("El Plátano", "Banana", "fruit");
        AddItem("La Naranja", "Orange", "fruit");
        AddItem("La Fresa", "Strawberry", "fruit");
        AddItem("Las Uvas", "Grapes", "fruit");

        // Vegetable shop
        AddItem("El Tomate", "Tomato", "vegetables");
        AddItem("La Patata", "Potato", "vegetables");
        AddItem("La Cebolla", "Onion", "vegetables");

        // Bakery
        AddItem("El Pan", "Bread", "bakery");
        AddItem("El Cruasán", "Croissant", "bakery");
        AddItem("El Pastel", "Cake", "bakery");
        AddItem("La Galleta", "Cookie", "bakery");

        // Dairy shop
        AddItem("La Leche", "Milk", "dairy");
        AddItem("El Queso", "Cheese", "dairy");
        AddItem("El Yogur", "Yogurt", "dairy");
        AddItem("Los Huevos", "Eggs", "dairy");

        // Cereals
        AddItem("El Arroz", "Rice", "cereals");
        AddItem("La Avena", "Oats", "cereals");
        AddItem("La Harina", "Flour", "cereals");
        AddItem("La Granola", "Granola", "cereals");
        AddItem("Los Copos de maíz", "Cornflakes", "cereals");

        // Butcher
        AddItem("El Pollo", "Chicken", "butcher");
        AddItem("El Cerdo", "Pork", "butcher");
        AddItem("La Salchicha", "Sausage", "butcher");

        // Spices
        AddItem("La Canela", "Cinnamon", "spices");

        // Grocery staples
        AddItem("Las Lentejas", "Lentils", "staples");
        AddItem("La Pasta", "Pasta", "staples");
        

        // Drinks
        AddItem("El Agua", "Water", "drinks");
        AddItem("El Zumo de naranja", "Orange juice", "drinks");
        AddItem("El Refresco", "Soda", "drinks");
        AddItem("El Café", "Coffee", "drinks");
        AddItem("El Té", "Tea", "drinks");
        AddItem("La Sangria", "Alcohol", "drinks");

        // Fish shop
        AddItem("El Salmón", "Salmon", "fish");
        AddItem("El Atún", "Tuna", "fish");
        AddItem("El Cangrejo", "Crab", "fish");

        L04_ShopTarget[] foundShops =
            FindObjectsByType<L04_ShopTarget>(
                FindObjectsSortMode.None);

        foreach (L04_ShopTarget shop in foundShops)
        {
            if (shop != null)
                shops.Add(shop);
        }

        if (player == null)
        {
            Debug.LogError("Assign the Player in the Inspector.");
            enabled = false;
            return;
        }

        if (shops.Count == 0)
        {
            Debug.LogError("No L04_ShopTarget components found.");
            enabled = false;
            return;
        }

        // Shuffle items randomly.
        for (int i = allItems.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            ShopItem temp = allItems[i];
            allItems[i] = allItems[j];
            allItems[j] = temp;
        }

        questions = allItems.GetRange(0, 5);

        ShowCurrentItem();
    }

    void AddItem(string spanish, string english, string shopId)
    {
        allItems.Add(new ShopItem(spanish, english, shopId));
    }

    void Update()
    {
        if (levelFinished || !Input.GetKeyDown(KeyCode.E))
            return;

        if (player == null)
            return;

        L04_ShopTarget nearestShop = null;
        float nearestDistance = float.MaxValue;

        foreach (L04_ShopTarget shop in shops)
        {
            if (shop == null)
                continue;

            Collider2D col = shop.GetComponent<Collider2D>();

            Vector2 closestPoint = col != null
                ? col.ClosestPoint(player.position)
                : (Vector2)shop.transform.position;

            float distance = Vector2.Distance(
                player.position, closestPoint);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestShop = shop;
            }
        }

        if (nearestShop == null || nearestDistance > interactRange)
        {
            if (feedbackText != null)
                feedbackText.text =
                    "Acércate a una tienda y pulsa E.";

            Debug.Log(
                "No shop in range. Distance: " + nearestDistance +
                ", Interaction range: " + interactRange);

            return;
        }

        if (currentIndex >= questions.Count)
            return;

        ShopItem currentItem = questions[currentIndex];

        if (nearestShop.shopId == currentItem.shopId)
        {
            currentIndex++;

            if (currentIndex >= questions.Count)
            {
                levelFinished = true;
                StartCoroutine(FinishLevel());
                return;
            }

            if (feedbackText != null)
                feedbackText.text =
                    "¡Correcto! " + currentItem.english;

            ShowCurrentItem();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text =
                    "¡Tienda incorrecta! Busca: " +
                    currentItem.spanish;
        }
    }

    void ShowCurrentItem()
    {
        if (currentIndex >= questions.Count)
            return;

        if (objectiveText != null)
            objectiveText.text =
                "BUSCA: " + questions[currentIndex].spanish;

        if (feedbackText != null)
            feedbackText.text =
                "Artículo " + (currentIndex + 1) + " de 5";
    }

    IEnumerator FinishLevel()
    {
        if (objectiveText != null)
            objectiveText.text = "¡NIVEL COMPLETADO!";

        if (feedbackText != null)
            feedbackText.text =
                "¡Excelente! Has encontrado los 5 artículos.";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}
