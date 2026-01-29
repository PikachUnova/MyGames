using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip getSound;
    public int itemType = 2;

    public static PowerUpItem powerUpItem;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        powerUpItem = this;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && InventoryUI.inventoryUI.GetItemCount() < 10)
        {
            InventoryUI.inventoryUI.SetItemType(itemType);
            InventoryUI.inventoryUI.AddInventory();
            AudioSource.PlayClipAtPoint(getSound, transform.position, 100f);
            Destroy(this.gameObject);
        }
    }

    public void Utilize(int type)
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
