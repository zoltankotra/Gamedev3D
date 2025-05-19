using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
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

    private bool gameOver = false;

    private int numberOfTotalEnemies;
    private int enemiesKilled = 0;



    [SerializeField] private Player player;
    [SerializeField] private DamageOverlay damageOverlay;
    [SerializeField] private HealOverlay healOverlay;
    [SerializeField] private InputDisplay inputDisplay;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private GameObject killCounter;


    [SerializeField] private IceShock crystalInstance;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Fireball fireballPrefabScript;

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
        gameOverUI.gameObject.SetActive(false);
        Time.timeScale = 0f;
        startUI.SetActive(true);
        health = maxHealth;
        inputQ = new Queue<int>(defaultQueueArray);
        timeQ.Clear();
        for (int i = 0; i < 4; i++)
        {
            timeQ.Enqueue(0f);
        }
        numberOfTotalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        killCounter.GetComponent<TMP_Text>().text = enemiesKilled.ToString() + "/" + numberOfTotalEnemies.ToString();
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

        if (Input.GetButtonDown("Cancel") && !gameOver && !startUI.activeInHierarchy)
        {
            TogglePauseMenu();
        }
        
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
        gameplayUI.gameObject.SetActive(true);
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
        gameOver = true;
        Time.timeScale = 0f;
        gameplayUI.HideUI();
        Cursor.lockState = CursorLockMode.None;
        gameOverUI.gameObject.SetActive(true);

        if (health > 0)
        {
            gameOverUI.SetMessage("Glorious Victory");
        }
        else
        {
            gameOverUI.SetMessage("Defeat");
        }
    }

    public void IncrementKillcount()
    {
        enemiesKilled += 1;
        killCounter.GetComponent<TMP_Text>().text = enemiesKilled.ToString() + "/" + numberOfTotalEnemies.ToString();

        if (enemiesKilled >= numberOfTotalEnemies)
        {
            StartCoroutine(InititateVictory());
        }
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

        if (Input.GetButtonDown("Forward"))
        {
            RegisterKeypress(0);
        }

        if (Input.GetButtonDown("Left"))
        {
            RegisterKeypress(1);
        }

        if (Input.GetButtonDown("Backward"))
        {
            RegisterKeypress(2);
        }

        if (Input.GetButtonDown("Right"))
        {
            RegisterKeypress(3);
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
            SpellIceShock(summedTime);
            ResetInputQueue();
            inputDisplay.UpdateInputDisplay(inputQ);

            Debug.Log("Ice");
            return;
        }
        if (inputQ.SequenceEqual(spellHealArray0) || inputQ.SequenceEqual(spellHealArray1))
        {
            SpellHeal(summedTime);
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
        health = (int)(health + strength * 6 > maxHealth ? 100 : health + strength * 6);
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

    private void SpellIceShock(float strength)
    {
        Ray forwardRay = new Ray(player.transform.position + 1.5f * Vector3.up, player.transform.forward);
        Ray downRay = new Ray(player.transform.position + 1.5f * Vector3.up + player.transform.forward, Vector3.down);
        RaycastHit hit;
        if (!Physics.Raycast(forwardRay, 1.5f, obstacleMask) && Physics.Raycast(downRay, out hit, 4f, obstacleMask))
        {
            var summonedCrystal = Instantiate(crystalInstance, hit.point, player.transform.rotation);
            summonedCrystal.power = strength;
            summonedCrystal.count = 1;
        }
    }

    // Coroutines

    private IEnumerator InititateVictory()
    {
        yield return new WaitForSeconds(3);
        GameOver();
    }
}
