using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombs : MonoBehaviour
{
    public GameObject Player;
    public GameObject nuke;


    // Start is called before the first frame update
    void Start()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player"); // Ensure your Player object has the "Player" tag
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject == Player)
        {
            Destroy(gameObject);
            Player.GetComponent<Player>().BombCount += 1;
        }
    }
}
