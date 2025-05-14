using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer, wallLayer;
    private GameManager gameManager;

    private CapsuleCollider attackHitbox;

    Animator animator;

    public float attackRange;
    public float attackExitRange;
    public bool playerInAttackRange;

    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        gameManager = GameManager.FindAnyObjectByType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackHitbox = GetComponent<CapsuleCollider>();


        attackHitbox.enabled = false;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        animator.SetFloat("animationOffset", Random.value);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!playerInAttackRange && !isAttacking) { chasePlayer(); }
        else { AttackPlayer(); }
    }

    private void Update()
    {
        float animationProgress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && 
            animationProgress > 0.3f &&
            animationProgress < 0.5f)
        {
            attackHitbox.enabled = true;
        }
        else { attackHitbox.enabled = false; }
    }

    private void chasePlayer()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        animator.SetBool("isAttacking", false);

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
        //isAttacking = true;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackExitRange, playerLayer);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isRunning", false);
        agent.SetDestination(transform.position);
        transform.LookAt(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            gameManager.GetHit(10);
        }
    }
}
