using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler gameUIHandler;
    // Start is called before the first frame update

    public Text scoreText;
    public int score = 0;

    // Health Bar
    public Text healthText;
    public int health = 100;
    public Slider healthBar;

    public Gradient healthGradient;
    public Image healthFill;


    // Air Bar (when underwater)
    public GameObject airGUIDisplay;
    public float air = 100f;
    public Slider airBar;

    public Gradient airGradiant;
    public Image airFill;

    void Start()
    {
        healthBar.maxValue = 100;
        healthFill.color = healthGradient.Evaluate(1f);

        airBar.maxValue = 100f;
        airFill.color = airGradiant.Evaluate(1f);

        if (GameUIHandler.gameUIHandler != null)
        {
            Destroy(this.gameObject);
            return;
        }
        gameUIHandler = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
        healthFill.color = healthGradient.Evaluate(healthBar.normalizedValue);
        scoreText.text = "Tokens: " + score;
        healthText.text = "Rex: " + health;


        airBar.value = air;
        airFill.color = airGradiant.Evaluate(airBar.normalizedValue);

        // Show Air Bar if in water
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().IsInWater())
            airGUIDisplay.SetActive(true);
        else
            airGUIDisplay.SetActive(false);
    }
}
