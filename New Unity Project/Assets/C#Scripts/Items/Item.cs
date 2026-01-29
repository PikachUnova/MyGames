using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip getSound;
    [SerializeField] private int itemType = 0;
    [SerializeField] private int healthValue = 0;

    public static Item item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        item = this;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (itemType <= 2)
            {
                if (InventoryUI.inventoryUI.GetItemCount() == 10) // basically heal the player whose inventory is full
                {
                    Heal();
                }
                else // Add item to inventory
                {
                    InventoryUI.inventoryUI.SetItemType(itemType);
                    InventoryUI.inventoryUI.AddInventory();
                }
            }
            else if (itemType > 2 && itemType <= 6)
            {
                if (InventoryUI.inventoryUI.GetItemCount() == 10) // basically heal the player whose inventory is full
                {
                    PowerUp(itemType);
                }
                else // Add item to inventory
                {
                    InventoryUI.inventoryUI.SetItemType(itemType);
                    InventoryUI.inventoryUI.AddInventory();
                }
            }
            else
                Destroy(this.gameObject);
        }

        AudioSource.PlayClipAtPoint(getSound, transform.position, 100f);
        Destroy(this.gameObject);
    }


    public void Heal()
    {
        int temp = PlayerHealth.playerHealth.maxHealth;

        if (PlayerHealth.playerHealth.currentHealth + healthValue >= PlayerHealth.playerHealth.maxHealth) // current health exceeded the maximum 
        {
            PlayerHealth.playerHealth.currentHealth = temp;
            GameUIHandler.gameUIHandler.health = temp;
        }
        else // normally restore health
        {
            PlayerHealth.playerHealth.currentHealth += healthValue;
            GameUIHandler.gameUIHandler.health += healthValue;
        }
    }

    public void PowerUp(int type)
    {
        if (type == 3)
        {
            //Does nothing if reaches to the maximum upgrade limit
            if (PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.fire] < PlayerShooter.upgradeShooter.maxUpgradeLevel)
                PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.fire] += 1;
        }
        else if (type == 4)
        {
            //Does nothing if reaches to the maximum upgrade limit
            if (PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.ice] < PlayerShooter.upgradeShooter.maxUpgradeLevel)
                PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.ice] += 1;
        }
        else if (type == 5)
        {
            //Does nothing if reaches to the maximum upgrade limit
            if (PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.electric] < PlayerShooter.upgradeShooter.maxUpgradeLevel)
                PlayerShooter.upgradeShooter.projectileUpgrades[(int)PlayerShooter.projectileType.electric] += 1;
        }
    }

}
