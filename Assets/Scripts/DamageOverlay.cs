using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    private Image image;
    private Color freshBlood = new Color(0.5f, 0.1f, 0f, 1f);
    private Color fadedBlood = new Color(0.2f, 0.15f, 0.1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.color = fadedBlood;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a > 0.2f)
        {
            image.color = Color.Lerp(image.color, fadedBlood, Time.deltaTime);
        }
        else if (image.color.a > 0f)
        {
            image.color = fadedBlood;
            Debug.Log("Overlay alpha set to 0");
        }
    }

    public void getHit()
    {
        image.color = freshBlood;
    }

}
