using Assets.Scripts.Unity.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class co : Tilemapper {
  public Transform Enemy;
  private bool jumpUsed = false; // Track if the enemy has used a jump
  public float stopRangeX =
      20f; // The range in the X axis within which the enemy should stop
  public float stopRangeY =
      20f; // The range in the Y axis within which the enemy should stop
  float speed = 20f; // Set the desired speed for movement

  // Start is called before the first frame update
  void Start() { StartCoroutine(EnemyMovement()); }

  // Update is called once per frame
  void Update() {}

  public IEnumerator EnemyMovement() {
    while (true) {  // Infinite loop for continuous pathfinding and movement
                    // If enemy x position is less than -50, freeze in place
            if (Enemy.position.x < -50)
            {
                break;
            }
                Pathfinder(); // Find the path to the target
      if (path.Count != 0) {
        // Move along the path
        for (int i = 0; i < path.Count; i++) {
          yield return 0;

          Vector3Int gridPosition =
              tilemap.WorldToCell(new Vector3(path[i].x, path[i].y, 0));
          Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);

          float enemyHeight = enemy.transform.localScale.y - 0.2f;
          Vector3 move =
              new Vector3(worldPosition.x, worldPosition.y + (2 * enemyHeight),
                          enemy.transform.position.z);

          // Calculate the direction the enemy is moving in the X axis
          float directionX = move.x - enemy.transform.position.x;

          // Flip the sprite based on movement direction
          if (directionX > 0) // Moving right
          {
            // Flip to the right
            enemy.transform.localScale = new Vector3(
                Mathf.Abs(enemy.transform.localScale.x),
                enemy.transform.localScale.y, enemy.transform.localScale.z);
          } else if (directionX < 0) // Moving left
          {
            // Flip to the left
            enemy.transform.localScale = new Vector3(
                -Mathf.Abs(enemy.transform.localScale.x),
                enemy.transform.localScale.y, enemy.transform.localScale.z);
          }

          // Now update the position
          /*enemy.transform.position = move;*/
          /*enemy.GetComponent<Rigidbody>().AddForce(move);*/
          enemy.transform.position = Vector3.MoveTowards(
              enemy.transform.position, move, speed * Time.deltaTime);

          // Check if the enemy is within the stopping range of the player
          if (IsWithinRangeOfPlayer()) {
            //Debug.Log("Enemy reached target range!");
          }
        }
      }

      jumpUsed = false; // Reset jump status after each path traversal
      yield return 0;
    }
  }

  public void Pathfinder() {
    target =
        new Vector2Int(GetPlayerTilePosition().x, GetPlayerTilePosition().y);
    start = new Vector2Int(GetEnemyTilePosition().x, GetEnemyTilePosition().y);

    List<Vector3Int> tilePositions = ReturnTiles();
    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    List<Vector2Int> visited = new List<Vector2Int>();
    Dictionary<Vector2Int, Vector2Int> cameFrom =
        new Dictionary<Vector2Int, Vector2Int>();

    queue.Enqueue(start);
    visited.Add(start);
    cameFrom[start] = start;

    Vector2Int[] directions = new Vector2Int[] {
      new Vector2Int(1, 0),  // Right
      new Vector2Int(-1, 0), // Left
      new Vector2Int(0, 1),  // Up
      new Vector2Int(0, -1)  // Down
    };

    while (queue.Count > 0) {
      Vector2Int current = queue.Dequeue();

      if (current == target) {
        //Debug.Log("Path found!");
        ReconstructPath(cameFrom, start, target);
        return;
      }

      foreach (var direction in directions) {
        Vector2Int neighbor = current + direction;
        Vector3Int neighbor3D = new Vector3Int(neighbor.x, neighbor.y, 0);

        if (IsValidNeighbor(neighbor, current, tilePositions) &&
            !visited.Contains(neighbor)) {
          queue.Enqueue(neighbor);
          visited.Add(neighbor);
          cameFrom[neighbor] = current;
        }
      }
    }
    //Debug.Log("Path not found");
  }

  private bool IsValidNeighbor(Vector2Int neighbor, Vector2Int current,
                               List<Vector3Int> tilePositions) {
    Vector3Int currentTile3D = new Vector3Int(current.x, current.y, 0);
    Vector3Int neighbor3D = new Vector3Int(neighbor.x, neighbor.y, 0);

    if (neighbor.x < tilemap.cellBounds.min.x ||
        neighbor.x >= tilemap.cellBounds.max.x ||
        neighbor.y < tilemap.cellBounds.min.y ||
        neighbor.y >= tilemap.cellBounds.max.y ||
        tilePositions.Contains(neighbor3D)) {
      return false;
    }

    int heightDifference = neighbor.y - current.y;

    if (heightDifference > 3 && !jumpUsed) {
      jumpUsed = true;
    } else if (heightDifference > 0 && jumpUsed) {
      return false;
    } else if (heightDifference > 3) {
      return false;
    }

    return true;
  }

  private bool IsWithinRangeOfPlayer() {
    Vector2 enemyPosition =
        new Vector2(enemy.transform.position.x, enemy.transform.position.y);
    Vector2 playerPosition =
        new Vector2(GetPlayerTilePosition().x, GetPlayerTilePosition().y);

    // Calculate the distance between the enemy and player on both axes
    float distanceX = Math.Abs(enemyPosition.x - playerPosition.x);
    float distanceY = Math.Abs(enemyPosition.y - playerPosition.y);

    // Check if the enemy is within the stopping range for each axis
    // independently
    bool withinRangeX = distanceX <= stopRangeX;
    bool withinRangeY = distanceY <= stopRangeY;

    return withinRangeX &&
           withinRangeY; // The enemy stops only when both conditions are met
  }

  public void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom,
                              Vector2Int start, Vector2Int target) {
    path.Clear();
    Vector2Int current = target;

    while (current != start) {
      path.Add(current);
      current = cameFrom[current];
    }
    path.Add(start);
    path.Reverse();

    string pathString = "Path found: ";
    foreach (var tile in path) {
      pathString += $"({tile.x}, {tile.y}) ";
    }

    //Debug.Log(pathString);
  }
}
