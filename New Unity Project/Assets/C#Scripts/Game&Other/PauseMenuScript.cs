using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour

{
    public PlayerHealth player;
    public GameUIHandler reset;
    GameObject[] PauseObjects;


    void Start()
    {
        PauseObjects = GameObject.FindGameObjectsWithTag("EditorOnly");
        hidePaused();  
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                hidePaused();
                InventoryUI.inventoryUI.hideInventory();
            }
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        set();
        hidePaused();
        Time.timeScale = 1;

    }

    public void PauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    public void showPaused()
    {
        foreach (GameObject g in PauseObjects)
            g.SetActive(true);
    }
    public void hidePaused()
    {
        foreach (GameObject g in PauseObjects)
            g.SetActive(false);
    }

    public void set()
    {
        player.currentHealth = player.maxHealth;
        reset.health = player.maxHealth;
    }
}

