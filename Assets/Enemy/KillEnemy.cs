using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    int points = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // This is triggered when two colliders with Is Trigger enabled interact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject); // Destroy the enemy
            Destroy(gameObject); // Destroy the bullet
            points++;
            Debug.Log(points);
        }
    }
}
