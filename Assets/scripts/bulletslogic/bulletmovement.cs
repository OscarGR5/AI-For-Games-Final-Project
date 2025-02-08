using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float bulletSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Optionally, set initial velocity if you want the bullet to move immediately
        // rb.velocity = new Vector2(bulletSpeed, 0); // Uncomment if needed
    }

    void Update()
    {
        // Check if the bullet has been fired
        if (Input.GetKeyDown(KeyCode.Escape)) // Change to your preferred key for firing
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        // Set the bullet's velocity to move in a specified direction
        rb.velocity = new Vector2(bulletSpeed, 0); // Change direction as needed
    }

    // This method is called when the bullet collides with another collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the bullet and the object it collides with
        Destroy(collision.gameObject); // Destroy the object it collides with
        Destroy(gameObject); // Destroy the bullet itself
    }
}
