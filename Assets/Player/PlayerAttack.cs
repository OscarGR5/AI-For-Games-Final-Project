using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    private float attackBuffer;
    public float startAttackBuffer;

    public Transform attackPos;
    public LayerMask enemyMask;
    public float attackRange;
    public float damage;

    public Animator animator; // Reference to the Animator component

    void Update() {
        if (attackBuffer <= 0) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                // Trigger the attack animation
                animator.SetTrigger("Attack");

                // Detect enemies in the attack range
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(
                                                   attackPos.position, attackRange, enemyMask);

                // Apply damage logic here
                foreach (Collider2D enemy in enemiesToDamage) {
                    // Example: enemy.GetComponent<Enemy>().TakeDamage(damage);
                }

                // Reset the attack buffer so the player can't attack again immediately
                attackBuffer = startAttackBuffer;
            }

        } else {
            attackBuffer -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
