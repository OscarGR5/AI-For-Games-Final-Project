using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointandshoot : MonoBehaviour
{
    public GameObject crosshairs;          // Public field for crosshairs
    private Vector3 target;                 // Private field for target
    public GameObject gun;                  // Public field for gun
    public GameObject bulletPrefab;         // Public field for bullet prefab
    public float bulletSpeed = 60.0f;       // Public field for bullet speed
    public int currentAmmo = 2;             // Current ammo
    public int maxAmmo = 15;                // Maximum ammo

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        crosshairs.transform.position = new Vector2(target.x, target.y);
        Vector3 difference = target - gun.transform.position;
        float rotationZ = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0) // Check if left mouse button is pressed and ammo is available
        {
            float distance = difference.magnitude;
            Vector2 direction = difference / distance;
            direction.Normalize();
            fireBullet(direction, rotationZ);
            ammoUsed(1); // Use ammo
        }

      //Debug.Log("Current Ammo in Update: " + currentAmmo); // Debug current ammo count
    }

    void fireBullet(Vector2 direction, float rotationZ)
    {
        GameObject bullet = Instantiate(bulletPrefab); // Instantiate bullet prefab
        bullet.transform.position = gun.transform.position;
        bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed; // Set bullet velocity
    }

    public void AmmoReceived(int ammo) // Add ammo
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo += ammo;
            if (currentAmmo > maxAmmo)
            {
                currentAmmo = maxAmmo; // Ensure we don't exceed max ammo
            }
            Debug.Log("Picked up bullets. Current ammo: " + currentAmmo);
        }
        else
        {
            Debug.Log("Character has max ammo");
        }
    }

    public int ammoUsed(int ammo) // Use ammo
    {
        if (currentAmmo > 0)
        {
            currentAmmo -= ammo;
            Debug.Log("Ammo used. Current ammo: " + currentAmmo);
        }
        else
        {
            Debug.Log("Character has no ammo");
        }
        return currentAmmo;
    }
}
