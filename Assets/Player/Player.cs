using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

  [SerializeField]
  private LayerMask platformMask;
  public Rigidbody2D rb;
  public BoxCollider2D bc;
  [SerializeField]
  float movementSpeed = 7f;
  [SerializeField]
  float jumpforce = 5f;
  private float attackBuffer;
  public float startAttackBuffer;
  public Transform attackPos;
  public LayerMask enemyMask;
  public float attackRange;
  public int damage;
  [SerializeField]
  public int health;
  public Animator animator;
  private Vector3 originalScale;
  public HealthBar healthBar;
  public int BombCount = 0;
  public Controller controller;
  [SerializeField]
  private SpriteRenderer spriteRenderer;

  void Start() {
    originalScale =
        transform.localScale;    // Store the original size of the player
    healthBar.SetMaxHealth(100); // Set the max health of the player
  }

  void Update() {
    // Check if player fell to infinity
    if (transform.position.y < -10) {
      healthBar.SetHealth(0);
      controller.GameOver();
      spriteRenderer.enabled = false;
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    // Jumping logic
    if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
      rb.velocity = Vector2.up * jumpforce;
    }

    float horizontalInput = 0;

    // Check for input from "A" or "D" keys
    if (Input.GetKey(KeyCode.D)) // Right movement
    {
      horizontalInput = 1f;                // Move right
      animator.SetBool("isWalking", true); // Trigger walking animation
      transform.localScale = new Vector3(originalScale.x, originalScale.y,
                                         originalScale.z); // Face right
    } else if (Input.GetKey(KeyCode.A))                    // Left movement
    {
      horizontalInput = -1f;               // Move left
      animator.SetBool("isWalking", true); // Trigger walking animation
      transform.localScale = new Vector3(-originalScale.x, originalScale.y,
                                         originalScale.z); // Flip left
    } else {
      // Stop the walking animation when no key is pressed
      animator.SetBool("isWalking", false);
    }

    // Apply movement to the Rigidbody
    rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y);

    // Stop horizontal movement when no keys are pressed
    if (horizontalInput == 0) {
      rb.velocity =
          new Vector2(0f, rb.velocity.y); // Set horizontal velocity to 0
    }

    // Attack logic
    if (attackBuffer <= 0) {
      if (Input.GetKeyDown(KeyCode.Mouse0)) {
        // Trigger the attack animation
        animator.SetTrigger("Attack");

        // Detect enemies in the attack range
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
            attackPos.position, attackRange, enemyMask);

        // Apply damage logic here
        // foreach (Collider2D enemy in enemiesToDamage) {
        for (int i = 0; i < enemiesToDamage.Length; i++) {
          // Enemy.GetComponent<Enemy>().TakeDamage(damage);
          enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        }

        // Reset the attack buffer so the player can't attack again immediately
        attackBuffer = startAttackBuffer;
      }
    } else {
      attackBuffer -= Time.deltaTime;
    }
  }

  // Ground logic
  private bool IsGrounded() {
    RaycastHit2D raycast = Physics2D.BoxCast(
        bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, platformMask);
    // Debug.Log(raycast.collider);
    return raycast.collider != null;
  }

  // Red circle to show the attack range
  void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPos.position, attackRange);
  }

  public void TakeDamage(int damage) {
    if (health > 0) {
      health -= damage;
      healthBar.SetHealth(health); // Update the health bar
      // healthSlider.value = health; // Update the slider value after taking
      // damage
    } else {
      healthBar.SetHealth(0); // Update the health bar
      controller.GameOver();
      spriteRenderer.enabled = false;
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
  }

  public void AddHealth(int heals) // heals refers to the amount of additional
  // health we receive from a medkit
  {
    if (health < 100) // Assuming max health is 100
    {
      health += heals;
      healthBar.SetHealth(health); // Update the health bar

      Debug.Log("Health = " + health);
    }
  }

  public int GetHealth() { return health; }
}
