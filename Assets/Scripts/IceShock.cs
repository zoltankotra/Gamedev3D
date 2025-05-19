using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShock : MonoBehaviour
{
    [SerializeField] private IceShock crystalInstance;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject gfx;

    public float power;
    public float count;

    private float growth;
    private float stopThreshold;
    private Vector3 randomRotation;

    // Start is called before the first frame update
    void Start()
    {
        randomRotation = new Vector3(Random.Range(-100f, -80f), Random.Range(-180f, 180f), Random.Range(-10f, 10f));
        gfx.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(randomRotation));
        gfx.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Debug.Log("Ice spell insanced");
        stopThreshold = power * 4 + 1 - count;
        StartCoroutine(SummonAndKillCrystal());
        crystalInstance.gameObject.GetComponent<Collider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        growth = Time.deltaTime * 4.5f;
        gfx.transform.localScale += new Vector3(growth, growth, growth);
    }

    private IEnumerator SummonAndKillCrystal()
    {
        yield return new WaitForSeconds(0.1f);
        Ray forwardRay = new Ray(transform.position + 1.5f * Vector3.up, transform.forward);
        Ray downRay = new Ray(transform.position + 1.5f * Vector3.up + transform.forward, Vector3.down);
        RaycastHit hit;
        if (!Physics.Raycast(forwardRay, 1.5f,obstacleMask) && Physics.Raycast(downRay, out hit, 4f, obstacleMask) && stopThreshold > 1f)
        {
            var summonedCrystal = Instantiate(crystalInstance, hit.point, transform.rotation);
            summonedCrystal.power = power;
            summonedCrystal.count = count + 1;
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.gameObject.tag == "Enemy")
        {
            enemy.transform.gameObject.GetComponent<IDamageInterface>().TakeDamage((int)(40f / count + 10));
        }
        
    }
}
