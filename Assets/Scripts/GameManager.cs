using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private readonly int maxHealth = 100;
    private int health;

    private int[] defaultQueueArray = new int[4] { -1, -1, -1, -1 };
    public Queue<int> inputQ;
    public Queue<float> timeQ = new Queue<float>();

    private float timePressed = 0f;
    private float summedTime = 0f;

    [SerializeField] private DamageOverlay damageOverlay;
    [SerializeField] private HealOverlay healOverlay;
    [SerializeField] private InputDisplay inputDisplay;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject startUI;

    [SerializeField] private Fireball fireballPrefabScript;

    [SerializeField] private Animator wandAnimator;

    // Spells start here
    private readonly int[] spellFireballArray0 = new int[4] { 0, 0, 2, 0 };
    private readonly int[] spellHealArray0 =     new int[4] { 1, 3, 3, 1 };
    private readonly int[] spellHealArray1 =     new int[4] { 3, 1, 1, 3 };
    private readonly int[] spellIceArray0 =      new int[4] { 1, 2, 3, 2 };
    private readonly int[] spellIceArray1 =      new int[4] { 3, 2, 1, 2 };

    // Spells end here

    // Start is called before the first frame update
    void Start()
    {
        pauseUI.gameObject.SetActive(false);
        Time.timeScale = 0f;
        startUI.SetActive(true);
        health = maxHealth;
        inputQ = new Queue<int>(defaultQueueArray);
        timeQ.Clear();
        for (int i = 0; i < 4; i++)
        {
            timeQ.Enqueue(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleInputQueues();

            if (Input.GetButtonDown("Fire1") &&
                    !(Input.GetKey(KeyCode.W) ||
                      Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) ||
                      Input.GetKey(KeyCode.D)))
            {
                CastSpell();
            }
        }

        inputDisplay.UpdateStrengthDisplay(summedTime, timePressed, timeQ.Peek());

        if (Input.GetButtonDown("Cancel") && health > 0 && !startUI.activeInHierarchy)
        {
            TogglePauseMenu();
        }
        
    }

    private void FixedUpdate()
    {
        
    }

    // My functions

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        startUI.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
            gameplayUI.gameObject.SetActive(true);
            pauseUI.gameObject.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            gameplayUI.gameObject.SetActive(false);
            pauseUI.gameObject.SetActive(true);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public void GetHit(int damage)
    {
        health -= damage;
        damageOverlay.GetHit();
        if (health <= 0) { GameOver(); }
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        gameplayUI.HideUI();
        Cursor.lockState = CursorLockMode.None;
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
            wandAnimator.CrossFade("Up", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RegisterKeypress(1);
            wandAnimator.CrossFade("Left", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RegisterKeypress(2);
            wandAnimator.CrossFade("Down", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RegisterKeypress(3);
            wandAnimator.CrossFade("Right", 0.1f);
        }

        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.D))
        {
            UpdateTimeQ();
        }
    }

    private void UpdateTimeQ()
    {
        timePressed = timeQ.Sum() + timePressed - timeQ.Peek() > 3f ? 3f - timeQ.Sum() + timeQ.Peek() : timePressed;
        timeQ.Enqueue(timePressed);
        if (timeQ.Count > 4) { timeQ.Dequeue(); }
        timePressed = 0f;

        summedTime = timeQ.Sum();
    }

    private void RegisterKeypress(int key) 
    {
        inputQ.Enqueue(key);
        if (inputQ.Count > 4) { inputQ.Dequeue(); }

        inputDisplay.UpdateInputDisplay(inputQ);
    }

    private void DamageTarget()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(hit.transform.name + " hit.");

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hit.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage(1);
            }
        }
    }

    private void CastSpell()
    {
        if (inputQ.SequenceEqual(spellFireballArray0))
        {
            SpellFireball(summedTime);
            ResetInputQueue();
            inputDisplay.UpdateInputDisplay(inputQ);

            Debug.Log("Fireball");
            return;
        }
        if (inputQ.SequenceEqual(spellIceArray0) || inputQ.SequenceEqual(spellIceArray1))
        {

            ResetInputQueue();
            inputDisplay.UpdateInputDisplay(inputQ);

            Debug.Log("Ice");
            return;
        }
        if (inputQ.SequenceEqual(spellHealArray0) || inputQ.SequenceEqual(spellHealArray1))
        {
            SpellHeal(summedTime * 5);
            ResetInputQueue();
            inputDisplay.UpdateInputDisplay(inputQ);

            Debug.Log("Heal");
            return;
        }

        DamageTarget();
        return;
    }

    private void ResetInputQueue()
    {
        timePressed = 0f;
        summedTime = 0f;

        inputQ.Clear();
        foreach (int arrow in defaultQueueArray)
        {
            inputQ.Enqueue(arrow);
        }

        timeQ.Clear();
        for (int i = 0; i < 4; i++)
        {
            timeQ.Enqueue(0f);
        }
    }

    // Spell Functions

    private void SpellHeal(float strength)
    {
        health = (int)(health + strength > maxHealth ? 100 : health + strength);
        healOverlay.Heal();
    }
    
    private void SpellFireball(float strength)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            var summonedFireball = Instantiate(fireballPrefabScript, hit.point, Quaternion.identity);
            summonedFireball.power = strength;
        }
    }
}
