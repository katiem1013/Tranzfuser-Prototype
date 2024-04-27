using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Referneces")]
    public GameObject weapon;

    [Header("Attacks")]
    public float damage;
    public Collider[] objectsInAttackRange;
    public float attackRange, weaponDelay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        objectsInAttackRange = Physics.OverlapSphere(weapon.transform.position, attackRange);

        foreach (Collider i in objectsInAttackRange)
        {
            var colliderObject = i.gameObject;

            var enemyHealth = colliderObject.GetComponent<EnemyHealth>();
            if (Input.GetMouseButtonDown(0))
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage, weaponDelay);
                
        }
    }
}
