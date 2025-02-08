using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject[] myObjects;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            int randomIndex = Random.Range(0, myObjects.Length);
            Vector3 randomSpawnPosition = new Vector2(Random.Range(10, 50), Random.Range(9,12));

            Instantiate(myObjects[randomIndex],randomSpawnPosition,Quaternion.identity);
        }
    }
}
