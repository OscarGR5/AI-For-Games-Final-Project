using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            // Get the pointandshoot component attached to the camera
            pointandshoot shooting = FindObjectOfType<pointandshoot>();
            if (shooting != null)
            {
                shooting.AmmoReceived(3); // Give the player 3 ammo
                Debug.Log("Ammo received! Current ammo: " + shooting.currentAmmo);
            }

            // Destroy this ammo object after it is received
            Destroy(gameObject); // Destroy this ammo object
        }
    }
}
