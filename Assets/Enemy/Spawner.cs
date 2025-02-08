using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
  [SerializeField]
  private GameObject enemyPrefab;
  private float enemyInterval = 0.5f;
  public Transform mainCharacter; // Assign your main character in the Inspector
  public Vector3 offset;          // Set an offset if desired
  // Start is called before the first frame update
  void Start() { StartCoroutine(spawner(enemyInterval, enemyPrefab)); }

  // Update is called once per frame
  void Update() {
    // Make the spawner follow the main character with the specified offset
    transform.position = mainCharacter.position + offset;
  }

  public int enemyCount() {
    int count = (GameObject.FindGameObjectsWithTag("Enemy")).Length;
  //  Debug.Log(count);
    return count;
  }

  private IEnumerator spawner(float interval, GameObject enemyPrefab) {
    if (enemyCount() < 5) {
      yield return new WaitForSeconds(interval);

      // Define a spawn offset range
      float spawnOffsetX = Random.Range(-5f, 5f); // Range of spawn area
      float spawnY = 5;

      // Spawn the enemy relative to the main character's position
      Vector3 spawnPosition =
          new Vector3(mainCharacter.position.x + spawnOffsetX, spawnY, 0);
      GameObject newEnemy =
          Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
      newEnemy.GetComponent<Enemy>().ResetHealth();
    }

    yield return new WaitForSeconds(interval);
    StartCoroutine(spawner(enemyInterval, enemyPrefab));
  }
}
