using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI keyText;

    private PlayerInventory inventory;
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        coinText.text = inventory.coin.ToString();
        heartText.text = inventory.heart.ToString();
        keyText.text = inventory.key.ToString();
    }
}
