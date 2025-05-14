using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int health = 100;

    public Queue<int> inputQ = new Queue<int>();
    public Queue<float> timeQ = new Queue<float>();

    private float timePressed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleInputQueues();


    }

    public void GetHit(int damage)
    {
        health -= damage;
        Debug.Log("Got hit. Current health: " + health);
    }

    private void handleInputQueues()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            timePressed += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RegisterKeypress(0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RegisterKeypress(1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RegisterKeypress(2);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RegisterKeypress(3);
        }

        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.D))
        {
            timeQ.Enqueue(timePressed);
            if (timeQ.Count > 4) { timeQ.Dequeue(); }
            timePressed = 0f;

            Debug.Log("TimeQ: " + timeQ.ToArray()[3]);
        }
    }


    private void RegisterKeypress(int key) 
    {
        inputQ.Enqueue(key);
        if (inputQ.Count > 4) { inputQ.Dequeue(); }

        Debug.Log("InputQ: " + inputQ.ToArray()[3]);
    }

    public int getHealth()
    {
        return health;
    }
    
}
