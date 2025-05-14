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

    [SerializeField] private DamageOverlay damageOverlay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputQueues();
        DamageTarget();
    }

    private void FixedUpdate()
    {
        
    }

    // My functions

    

    public void GetHit(int damage)
    {
        health -= damage;
        damageOverlay.getHit();
        Debug.Log("Got hit. Current health: " + health);
    }

    private void HandleInputQueues()
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

    private void DamageTarget()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.name + " hit.");

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    hit.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage(10);
                }
            }
        }
    }

    public int GetHealth()
    {
        return health;
    }
    
}
