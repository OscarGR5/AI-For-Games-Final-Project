using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour {
  [SerializeField]
  private int health = 100;
  private float attackBuffer;
  public float startAttackBuffer;
  private int maxHealth = 100;
  public Transform attackPos;
  public LayerMask playerMask;
  public float attackRange;
  public int damage;
  public Animator EnemyAnimator; // Reference to the Animator component
  private float dazedTime;
  public float startDazedTime;
  public GameObject player;
  public static float speed = 3;

  private SpriteRenderer
      spriteRenderer; // Reference to SpriteRenderer for color changes
  public Color damageColor = Color.red; // Color to flash when damaged
  public float flashDuration = 0.1f;    // Duration of the flash

  void Start() {
    ResetHealth(); // Reset health to max when the game starts
    spriteRenderer =
        GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
  }

  void Update() {
    OutOfBound();

    if (dazedTime <= 0) {
      speed = 3;
      EnemyAnimator.SetBool("isWalking", true);
    } else {
      speed = 0;
      EnemyAnimator.SetBool("isWalking", false);
      dazedTime -= Time.deltaTime;
    }

    if (attackBuffer <= 0 && dazedTime <= 0) {
      // Detect players in the attack range
      Collider2D[] playerDamage = Physics2D.OverlapCircleAll(
          attackPos.position, attackRange, playerMask);

      // Trigger the attack animation

      // Deal damage to the players in the list
      for (int i = 0; i < playerDamage.Length; i++) {
        playerDamage[i].GetComponent<Player>().TakeDamage(damage);
      }

      if (playerDamage.Length != 0) {
        EnemyAnimator.SetTrigger("Attack");
        playerDamage[0].GetComponent<Player>().TakeDamage(damage);
      }

      // Reset attack buffer
      attackBuffer = startAttackBuffer;
    } else {
      attackBuffer -= Time.deltaTime;
    }
    if (health <= 0) {
      KillCounter.killCount++;
      Die();
    }
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPos.position, attackRange);
  }

  public void TakeDamage(int damageToTake) {
    dazedTime = startDazedTime;
    health -= damageToTake;
    // Trigger the flash effect
    StartCoroutine(FlashDamageEffect());
  }

  private IEnumerator FlashDamageEffect() {
    // Save the original color
    Color originalColor = spriteRenderer.color;

    // Wait for the specified delay before starting the flash
    yield return new WaitForSeconds(0.1f);

    // Change the color to indicate damage
    spriteRenderer.color = damageColor;

    // Wait for the flash duration
    yield return new WaitForSeconds(flashDuration);

    // Revert to the original color
    spriteRenderer.color = originalColor;
  }
  public void OutOfBound() {
    if (transform.position.y < -5) {
      Destroy(gameObject);
    }
  }
  public void ResetHealth() {
    health = maxHealth; // Reset health to max whenever called
  }

  private void Die() {
    Destroy(gameObject);
    player.GetComponent<Player>().AddHealth(25);
  }
}
