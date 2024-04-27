using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [Header("Knockback")]
    public Collider[] objectsInKBRange;
    public float damage = 5, KBRange, KnockbackForce = 250;
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        objectsInKBRange = Physics.OverlapSphere(gameObject.transform.position, KBRange);
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Collider i in objectsInKBRange)
            {
                var enemy = i.gameObject;

                var enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemy.transform.position += transform.forward * Time.deltaTime * KnockbackForce;
                    enemyHealth.TakeDamage(damage, 0);
                }
            }
        }
    }

    private void Knockback()
    {
        
    }
}
