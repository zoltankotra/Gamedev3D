using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Player player;

    public void UpdateMouseSensitivity()
    {
        player.SetMouseSensitivity(slider.value);
    }
}
