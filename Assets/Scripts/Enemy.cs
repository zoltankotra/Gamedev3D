using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour, IDamageInterface
{
    public int damage = 6;
    private int health;
    public int maxHealth = 50;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer, wallLayer;
    private GameManager gameManager;

    [SerializeField] private CapsuleCollider attackHitbox;
    [SerializeField] private CapsuleCollider bodyCollider;

    Animator animator;

    public float attackRange;
    public float attackExitRange;
    public bool playerInAttackRange;

    private bool canChase = true;
    private bool alive = true;
    private Vector3 playerLastSeenPosition;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;


        player = GameObject.Find("Player").transform;
        gameManager = GameManager.FindAnyObjectByType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //attackHitbox = GetComponent<CapsuleCollider>();


        attackHitbox.enabled = false;
        playerInAttackRange = Physics.CheckSphere(transform.position + Vector3.up * 1.5f, attackRange, playerLayer);
        animator.SetFloat("animationOffset", Random.value);

        agent.autoTraverseOffMeshLink = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInAttackRange && canChase && alive) { ChasePlayer(); }
        else if (playerInAttackRange && alive) { AttackPlayer(); }
    }

    private void ChasePlayer()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position + Vector3.up * 1.5f, attackRange, playerLayer);

        animator.SetBool("isAttacking", false);
        animator.SetBool("isHurting", false);

        Ray ray = new Ray(transform.position + Vector3.up, player.position - transform.position);
        float rayLength = (player.position - transform.position).magnitude;
        if (!Physics.Raycast(ray, rayLength, wallLayer))
        {
            var path = new NavMeshPath();
            agent.CalculatePath(player.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(player.position);
            }
            

            

            if (!agent.hasPath)
            {
                Debug.Log(agent.hasPath + " " + agent.velocity.magnitude + " " + path.status);
            }


            
            animator.SetBool("isRunning", true);
        }
        else if (agent.remainingDistance < attackRange)
        {
            agent.SetDestination(transform.position);
            animator.SetBool("isRunning", false);
        }
        
    }

    private void AttackPlayer()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position + Vector3.up * 1.5f, attackExitRange, playerLayer);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isRunning", false);
        agent.SetDestination(transform.position);
        Vector3 targetPostition = new Vector3(player.position.x,
                                              transform.position.y,
                                              player.position.z);
        transform.LookAt(targetPostition);

        float animationProgress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            animationProgress > 0.38f &&
            animationProgress < 0.45f)
        {
            attackHitbox.enabled = true;
        }
        else { attackHitbox.enabled = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            gameManager.GetHit(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        agent.SetDestination(transform.position);
        health -= damage;
        if (health <= 0 && alive)
        {
            attackHitbox.enabled = false;
            animator.CrossFade("Death", 0.2f);
            alive = false;
            bodyCollider.enabled = false;
            gameManager.IncrementKillcount();
            StartCoroutine(DeleteDead());
            return;
        }

        animator.CrossFade("Hurt",0.2f);
        StartCoroutine(FinishAnimation());
    }

    public void SetHealthAndAttack(int healthToBeSet, int attackToBeSet)
    {
        health = healthToBeSet;
        damage = attackToBeSet;
        maxHealth = healthToBeSet;
    }

    public void SetDifficulty(int diff)
    {
        Debug.Log("Difficulty set to" + diff);
        damage = 7 + diff * 5;
        maxHealth = 50 + diff * 50;
        health = 50 + diff * 50;
        Debug.Log($"Damage: {damage}, health: {health}");

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.gameObject.GetComponent<IDamageInterface>().SetHealthAndAttack(health, damage);
        }
    }

    IEnumerator FinishAnimation() // Switch to Animation event. This is causing issues when it takes damage twice within the specified time.
    {
        canChase = false;
        yield return new WaitForSeconds(0.6f);
        canChase = true;
    }

    IEnumerator DeleteDead()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

}
