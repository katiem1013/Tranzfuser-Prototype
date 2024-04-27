using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("References")]
    private GameObject player;
    private GameObject playerDirection, playerHand, enemy;
    private EnemyHealth enemyHealth;
    public LineRenderer lineRenderer;
    public Rigidbody rb;

    [Header("Hook")]
    public bool isHooking;
    public bool wasEnemyHooked;
    public float hookDistance;
    public Vector3 originalPos;
    public float damage = 5;

    private void Awake()
    {
        // starting variables
        player = GameObject.FindGameObjectWithTag("Player");
        playerDirection = player.transform.GetChild(1).gameObject;
        playerHand = GameObject.FindGameObjectWithTag("Hand");
        lineRenderer = GetComponent<LineRenderer>();
        isHooking = false;
        wasEnemyHooked = false;
        hookDistance = 0;
        rb = GetComponent<Rigidbody>();
        originalPos = new Vector3(playerHand.transform.position.x + Constants.X_OFFSET, playerHand.transform.position.y - Constants.Y_OFFSET, playerHand.transform.position.z);
    }

    private void Update()
    {
        originalPos = new Vector3(playerHand.transform.position.x + Constants.X_OFFSET, playerHand.transform.position.y - Constants.Y_OFFSET, playerHand.transform.position.z);
        lineRenderer.SetPosition(0, originalPos);
        lineRenderer.SetPosition(1, transform.position);

        // should probably change this later so that it can't happen while moving
        if (Input.GetMouseButtonDown(1) && !isHooking && !wasEnemyHooked)
            Invoke(nameof(StartHooking), 0.4f);

        ReturnHook();
        BringEnemyToPlayer();
    }

    private void StartHooking()
    {
        isHooking = true;

        // shoots hook
        rb.isKinematic = false;
        rb.AddForce(playerDirection.transform.forward * Constants.HOOK_SPEED);
    }

    private void ReturnHook()
    {
        if (isHooking)
        {
            // returns hook to starting position
            hookDistance = Vector3.Distance(transform.position, originalPos);
            if (hookDistance > Constants.MAX_HOOK_DISTANCE || wasEnemyHooked)
            {
                rb.isKinematic = true;
                transform.position = originalPos;
                isHooking = false;
            }
        }
    }

    private void BringEnemyToPlayer()
    {
        if (wasEnemyHooked)
        {
            // brings enemy to just in front of player
            Vector3 finalPos = new Vector3(originalPos.x, enemy.transform.position.y, originalPos.z + Constants.Z_OFFSET);
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, finalPos, Constants.ENEMY_Z_OFFSET);
            wasEnemyHooked = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if enemy was hit
        if(other.gameObject.tag.Equals("Enemy"))
        {
            wasEnemyHooked = true;
            enemy = other.gameObject;
            enemyHealth = enemy.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage, 0);
        }

        // later could add using the hook on objects and stuff
    }
}
