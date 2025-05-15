using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealOverlay : MonoBehaviour
{
    private Image image;
    private Color freshHeal = new Color(0.8f, 0.9f, 0.5f, 1f);
    private Color fadedHeal = new Color(0.8f, 1f, 0.8f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.color = fadedHeal;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a > 0.01f)
        {
            image.color = Color.Lerp(image.color, fadedHeal, Time.deltaTime * 3);
        }
        else if (image.color.a > 0f)
        {
            image.color = fadedHeal;
        }
    }

    public void Heal()
    {
        image.color = freshHeal;
    }
}
