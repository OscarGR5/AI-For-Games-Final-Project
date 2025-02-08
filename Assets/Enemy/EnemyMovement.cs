using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    public Transform mainCharacter; // Reference to the main character's transform
    private float speed;            // Speed of the enemy
    public float stoppingDistance; // Distance at which the enemy will stop moving
    private Vector2 lastPosition;  // To track the last position of the enemy
    public Animator EnemyAnimator; // Reference to the Animator component

    private void Start() {
        // Store the initial position of the enemy
        lastPosition = transform.position;
        EnemyAnimator = GetComponent<Animator>(); // Get the Animator component
    }

    private void Update() {
        speed = Enemy.speed; // Get the speed of the enemy from the Enemy script
        if (mainCharacter != null) {
            // Calculate the distance between the enemy and the main character
            float distanceToMainCharacter =
                Vector2.Distance(transform.position, mainCharacter.position);

            // Only move towards the main character if the enemy is farther than the
            // stopping distance
            if (distanceToMainCharacter > stoppingDistance) {
                // Calculate direction towards the main character
                Vector2 direction =
                    (Vector2)mainCharacter.position - (Vector2)transform.position;
                direction.Normalize(); // Normalize the direction vector to ensure
                // consistent speed

                // Move the enemy towards the main character
                transform.position += (Vector3)(direction * speed * Time.deltaTime);

                // Determine the enemy's current movement direction by comparing its
                // current position with its last position
                Vector2 movementDirection = (Vector2)transform.position - lastPosition;

                // Flip the sprite based on the movement direction
                if (movementDirection.x < 0) // Moving left
                {
                    transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x),
                                                       transform.localScale.y);
                } else if (movementDirection.x > 0) // Moving right
                {
                    transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x),
                                                       transform.localScale.y);
                }

                // Update the last position for the next frame
                lastPosition = transform.position;

                // Play walking animation
                EnemyAnimator.SetBool("isWalking", true);
            } else {
                // Stop moving and reset animation parameters
                EnemyAnimator.SetBool("isWalking", false); // Stop the walking animation
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("Enemy hit the main character!");
        }
    }
}
