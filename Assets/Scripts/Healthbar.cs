using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public GameManager gameManager;
    private int health;
    private RectTransform rectTransform;
    private Image image;
    private int healthBarWidth;
    private Color lowHealth = new Color(1f, 0.6f, 0.2f);
    private Color minimalHealth = new Color(0.8f, 0.1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        health = gameManager.getHealth();
        healthBarWidth = health > 10 ? health + 6 : 16;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidth);

        if (health <= 30 && health > 10)
        {
            image.color = lowHealth;
        }
        else if (health <= 10)
        {
            image.color = minimalHealth;
        }
        else
        {
            image.color = Color.white;
        }

    }
}
