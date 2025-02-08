using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField]
    private LayerMask platformMask;
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    [SerializeField]
    float movementSpeed = 7f;
    [SerializeField]
     public float jumpforce = 5f;
    public Animator animator;  // Reference to the Animator
    private Vector3 originalScale; // To store the initial size of the player

    void Start()
    {
        originalScale = transform.localScale; // Store the original size of the player
    }

    void Update()
    {

        // Jumping logic
        if (isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.up * jumpforce;
        } 


        float horizontalInput = 0;

        // Check for input from "A" or "D" keys
        if (Input.GetKey(KeyCode.D)) // Right movement 
        {
            horizontalInput = 1f; // Move right
            animator.SetBool("isWalking", true);  // Trigger walking animation
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z); // Face right
        }
        else if (Input.GetKey(KeyCode.A)) // Left movement 
        {
            horizontalInput = -1f; // Move left
            animator.SetBool("isWalking", true);  // Trigger walking animation
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // Flip left
        } else
        {
            // Stop the walking animation when no key is pressed
            animator.SetBool("isWalking", false);
        }


        // Apply movement to the Rigidbody
        rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y);

        // Stop horizontal movement when no keys are pressed
        if (horizontalInput == 0)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // Set horizontal velocity to 0
        }
    }

    // Ground logic
    private bool isGrounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(
            bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, platformMask);
        //Debug.Log(raycast.collider);
        return raycast.collider != null;
    }
}
