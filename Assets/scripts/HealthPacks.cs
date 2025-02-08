using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPacks : MonoBehaviour
{
    public int heal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        {
            if (player.GetHealth() < 100)
            {
                if (collision.gameObject.CompareTag("Player") == true)
                {
                    player.AddHealth(heal);
                    Destroy(gameObject);
                }
               
            }
            else
            {
                if (collision.gameObject.CompareTag("Player") == true)
                    Debug.Log("Character is already at max health");
            }
        }
    }
}
