using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMessage;
    [SerializeField] private GameObject gameOverMessageShadow;


    public void SetMessage(string message)
    {
        gameOverMessage.GetComponent<TMP_Text>().text = message;
        gameOverMessageShadow.GetComponent<TMP_Text>().text = message;
    }
}
