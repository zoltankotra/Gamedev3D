using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject outerBall;
    [SerializeField] private GameObject innerBall;
    [SerializeField] private SphereCollider outerCollider;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private int damageMultiplier = 10;
    private float growth;

    public float power;


    // Start is called before the first frame update
    void Start()
    {
        outerBall.transform.localScale = Vector3.zero;
        StartCoroutine(KillFireball());
        outerCollider.enabled = false;
        StartCoroutine(EnableTrigger());
    }

    // Update is called once per frame
    void Update()
    {
        growth = Time.deltaTime * (power + 3);
        outerBall.transform.localScale += new Vector3(growth, growth, growth);
    }

    private void OnTriggerEnter(Collider enemy)
    {
        Debug.Log("Enemy inside fireball");

        enemy.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage((int)(power * damageMultiplier));

        if (Physics.CheckSphere(transform.position, 0.5f, enemyMask))
        {
            enemy.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage((int)(power * damageMultiplier));
        }
    }


    private IEnumerator EnableTrigger()
    {
        yield return new WaitForSeconds(0.1f);
        outerCollider.enabled = true;
    }

    private IEnumerator KillFireball()
    {
        yield return new WaitForSeconds(power/10 + 0.4f);
        Destroy(gameObject);
    }
}
