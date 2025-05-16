using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    [SerializeField] Image arrow1;
    [SerializeField] Image arrow2;
    [SerializeField] Image arrow3;
    [SerializeField] Image arrow4;
    [SerializeField] Image strengthBar;     // red
    [SerializeField] Image potentialBar;    // green
    [SerializeField] Image lossBar;         // white


    private int[] inputArray = new int[4];
    private Image[] arrowArray = new Image[4];
    private int currentWidth;
    private int potentialWidth;
    private int lossWidth;


    // Start is called before the first frame update
    void Start()
    {
        arrowArray[0] = arrow1;
        arrowArray[1] = arrow2;
        arrowArray[2] = arrow3;
        arrowArray[3] = arrow4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInputDisplay(Queue<int> inputQ)
    {
        inputQ.CopyTo(inputArray,0);

        for (int i = 0; i < 4; i++)
        {
            if (inputArray[i] >= 0)
            {
                arrowArray[i].transform.SetPositionAndRotation(arrowArray[i].transform.position, Quaternion.Euler(0, 0, inputArray[i] * 90));
            }
            else
            {
                arrowArray[i].transform.SetPositionAndRotation(arrowArray[i].transform.position, Quaternion.Euler(90, 0, 0));
            }
        }
    }

    public void UpdateStrengthDisplay(float strength, float potential, float loss) 
    {
        currentWidth = (int)(strength * 50 + 14);
        potentialWidth = (int)((strength + potential - loss) * 50 + 14);
        potentialWidth = potentialWidth > 164 ? 164 : potentialWidth;
        lossWidth = currentWidth >= potentialWidth ? potentialWidth : currentWidth;


        strengthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
        potentialBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, potentialWidth);
        lossBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lossWidth);

        if (!(Input.GetKey(KeyCode.W) ||
              Input.GetKey(KeyCode.A) ||
              Input.GetKey(KeyCode.S) ||
              Input.GetKey(KeyCode.D)))
        {
            lossBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
        }
    }
}
