using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerTransform;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    [Header("Attacks")]
    private float bulletTime;
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject enemyBullet;

    [Header("Ranges and Spawners")]
    public Transform spawnPoint;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();

        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();

        if (playerInSightRange && playerInAttackRange)
            AttackPlayer();

        else
            print("No Current State");
    }

    private void Patrol()
    {
        // when the walk point is false searches for the next position to walk to
        if (!walkPointSet)
            SearchWalkPoint();

        // once a position has been found the enemy will move to it
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // calculating distance
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // when walkPoint is reached sets walk point to false
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // finds random points in range
        float randomz = Random.Range(-walkPointRange, walkPointRange);
        float randomx = Random.Range(-walkPointRange, walkPointRange);

        // sets the position for the enemy to move to 
        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        // checks if the enemy is on the ground 
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        // enemy chases the player
        agent.SetDestination(playerTransform.position);
    }

    private void AttackPlayer()
    {
        // keeps the player chasing the player and has them face the player to attack
        agent.SetDestination(playerTransform.position);
        transform.LookAt(new Vector3(playerTransform.position.x, Vector3.forward.y, playerTransform.position.z));

        // if the enemy hasn't attacked, start attacking
        if (!alreadyAttacked)
        {
            bulletTime -= Time.deltaTime;
            if (bulletTime > 0)
                return;

            bulletTime = timeBetweenAttacks;

            GameObject bulletObject = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
            Rigidbody bulletRB = bulletObject.GetComponent<Rigidbody>();
            bulletRB.AddForce(bulletRB.transform.forward * 1000);
            Destroy(bulletObject, 2f);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); // resets the attack
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
