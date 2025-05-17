using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageInterface
{
    private int health;
    public int maxHealth;

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
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        animator.SetFloat("animationOffset", Random.value);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerInAttackRange && canChase && alive) { ChasePlayer(); }
        else if (playerInAttackRange && alive) { AttackPlayer(); }
    }

    private void Update()
    {
        
    }

    private void ChasePlayer()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        animator.SetBool("isAttacking", false);
        animator.SetBool("isHurting", false);

        Ray ray = new Ray(transform.position + Vector3.up, player.position - transform.position);
        float rayLength = (player.position - transform.position).magnitude;
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, rayLength, wallLayer))
        {
            agent.SetDestination(player.position);
            animator.SetBool("isRunning", true);

            Debug.DrawRay(transform.position + Vector3.up, player.position - transform.position, Color.green);
        }
        else if (agent.remainingDistance < attackRange)
        {
            agent.SetDestination(transform.position);
            animator.SetBool("isRunning", false);
            Debug.DrawRay(transform.position + Vector3.up, player.position - transform.position, Color.yellow);
        }
        else { Debug.DrawRay(transform.position + Vector3.up, player.position - transform.position, Color.red); }
        
    }

    private void AttackPlayer()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackExitRange, playerLayer);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isRunning", false);
        agent.SetDestination(transform.position);
        transform.LookAt(player.position);

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
            gameManager.GetHit(10);
        }
    }

    public void TakeDamage(int damage)
    {
        agent.SetDestination(transform.position);
        health -= damage;
        if (health <= 0)
        {
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
