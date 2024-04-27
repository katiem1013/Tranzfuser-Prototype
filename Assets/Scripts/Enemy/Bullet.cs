using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Attacks")]
    public float damage;
    public PlayerHealth player;

    private void Start()
    {
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10); // shoots at the player
        Destroy(gameObject, 1); // destroys the bullet after 1 second
    }

    private void OnCollisionEnter(Collision collide)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>(); // gets the player object


        // deals damage to the player if hit, and destroys the bullet
        if (collide.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(damage); // deals damage
            Destroy(gameObject); // destroys the bullet
        }
    }
}
