using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject outerBall;
    [SerializeField] private GameObject innerBall;
    [SerializeField] private SphereCollider outerCollider;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private int damageMultiplier = 10;
    private float growth;
    private Material ballMaterial;
    private float aliveTime;
    private float lifetime;

    private Color startColor = new Color(1f, 0.25f, 0f, 1f);
    private Color endColor = new Color(1f, 0.7f, 0f, 0.1f);

    public float power;


    // Start is called before the first frame update
    void Start()
    {
        lifetime = power / 10 + 0.4f;
        aliveTime = 0f;
        ballMaterial = outerBall.gameObject.GetComponent<MeshRenderer>().material;
        outerBall.transform.localScale = Vector3.zero;
        StartCoroutine(KillFireball());
        outerCollider.enabled = false;
        StartCoroutine(EnableTrigger());
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;
        growth = Time.deltaTime * (power + 3);
        ballMaterial.SetColor("_Color",Color.Lerp(startColor,endColor,aliveTime/lifetime));
        ballMaterial.SetColor("_EmissionColor", Color.Lerp(startColor * 2f, endColor * 0.1f, aliveTime / lifetime));
        outerBall.transform.localScale += new Vector3(growth, growth, growth);
    }

    private void OnTriggerEnter(Collider enemy)
    {

        if (enemy.gameObject.tag == "Enemy")
        {
            enemy.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage((int)(power * damageMultiplier));

            if (Physics.CheckSphere(transform.position, 0.5f, enemyMask))
            {
                enemy.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage((int)(power * damageMultiplier));
            }
        }
    }


    private IEnumerator EnableTrigger()
    {
        yield return new WaitForSeconds(0.1f);
        outerCollider.enabled = true;
    }

    private IEnumerator KillFireball()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
