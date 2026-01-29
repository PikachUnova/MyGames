using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI inventoryUI;

    private GameObject[] InventoryObjects;
    public Image [] itemIcons;

    public Sprite [] imageIcons;
    private int [] itemTypesOwned = new int[10];
    private int itemType = 0;

    private int itemCount = 0;

    public GameObject descriptionDisplay;
    [SerializeField] private Text ItemNameText;
    [SerializeField] private Text ItemDescriptionText;

    void Start()
    {
        inventoryUI = this;

        InventoryObjects = GameObject.FindGameObjectsWithTag("Inventory");
        hideInventory();
        descriptionDisplay.SetActive(false); // Hide the text
    }

    public void AddInventory()
    {
        for (int i = 0; i < 10; i++)
        {
            if (itemIcons[i].sprite == null)
            {
                itemIcons[i].sprite = imageIcons[itemType];
                itemTypesOwned[i] = itemType;
                break;
            }
        }
        itemCount++;
    }

    public void UseInventory(int order)
    {
        // Can't use anything if blank
        if (itemIcons[order].sprite == null)
            return;

        itemIcons[order].sprite = null;

        if (itemTypesOwned[order] <= 2)
            Item.item.Heal();
        else
            Item.item.PowerUp(itemTypesOwned[order]);
        
        itemCount--;
    }

    public void hideInventory()
    {
        foreach (GameObject g in InventoryObjects)
            g.SetActive(false);      
    }

    public void showInventory()
    {
        foreach (GameObject g in InventoryObjects)
            g.SetActive(true);
    }

    public int GetItemCount()
    {
        return itemCount;
    }

    public void SetItemType(int type)
    {
        itemType = type;
    }



    public void OnPointerEnter(int order)
    {
        // Can't describe anything if blank
        if (itemIcons[order].sprite == null)
            return;

        if (itemTypesOwned[order] == 0)
        {
            ItemNameText.text = "Heart Apple";
            ItemDescriptionText.text = "A fruit rich in nutrients that quickly heal wounds.";
        }
        else if (itemTypesOwned[order] == 1)
        {
            ItemNameText.text = "Life Apple";
            ItemDescriptionText.text = "A fruit that resurrects the user.";
        }
        else if (itemTypesOwned[order] == 2)
        {
            ItemNameText.text = "Gold Heart Apple";
            ItemDescriptionText.text = "A rare fruit that completely eliminates wounds once consumed.";
        }
        else if (itemTypesOwned[order] == 3)
        {
            ItemNameText.text = "Fire Breathing Fruit";
            ItemDescriptionText.text = "A sizzling cinnamon-spiced dragon fruit that allows the user to launch fireballs.";
        }
        else if (itemTypesOwned[order] == 4)
        {
            ItemNameText.text = "Ice Breathing Fruit";
            ItemDescriptionText.text = "A chilly dragon fruit that that allows the user to launch barrages of iceballs.";
        }
        else if (itemTypesOwned[order] == 5)
        {
            ItemNameText.text = "Voltage Fruit";
            ItemDescriptionText.text = "A fruit that makes you shoot more lightning balls.";
        }
        descriptionDisplay.SetActive(true);
    }

    public void OnPointerExit()
    {
        descriptionDisplay.SetActive(false); // Hide the text
    }

}
