using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Unity.Tilemaps {
public class Tilemapper : MonoBehaviour {
  public int currentKillCount;
  public GameObject Canvas;
  public GameObject enemy;
  public Tilemap tilemap;
  public GameObject player;
  public int[][] tileData;
  public Vector2Int target;
  public Vector2Int start;
  public Tile Tile;
  public List<Vector3Int> tilePositions;
  public int maxX;
  public int maxY;
  public GameObject boss;
  public int minX;
  public int minY;
  public Tile Tile2;
  public Tile Vine;
  private float spawnCooldownTimer = 2f;
  private float spawnCooldown = 5f;
  private Vector3 lastPlayerPosition;
  private float movementThreshold = 40f; // Distance in units to trigger spawn
  public bool floortrue = true;
  public GameObject nuke;
  public int startpos = 5000;
  public int stoppos = 5182;
  public int bombCount;
  public bool platformFieldActive = true;
  public GameObject Healthpack1;
  public GameObject Healthpack2;
  public List<Vector2Int> path = new List<Vector2Int>();

  public List<Vector3Int> ReturnTiles() {
    tilePositions.Clear();

    var bounds = tilemap.cellBounds;

    for (int x = bounds.min.x; x < bounds.max.x; x++) {
      for (int y = bounds.min.y; y < bounds.max.y; y++) {
        Vector3Int cellPosition = new Vector3Int(x, y, 0);
        TileBase tile =
            tilemap.GetTile(cellPosition); // Get the tile at the cell position

        if (tile != null) // Check if the tile exists
        {
          tilePositions.Add(cellPosition); // Add the position to the list
        }
      }
    }

    // Print out the collected tile positions
    foreach (var tilePos in tilePositions) {
      // Debug.Log(tilePos); // Print each tile position
    }

    return tilePositions; // Return the list of tile positions
  }

  public Vector3Int GetPlayerTilePosition() {
    Vector3Int playerposition = tilemap.WorldToCell(player.transform.position);

    // Rounding up each component
    playerposition.x = Mathf.CeilToInt(playerposition.x);
    playerposition.y =
        Mathf.CeilToInt(playerposition.y - 2 * player.transform.localScale.y);

    // Return the rounded-up coordinates
    return playerposition;
  }

  public Vector3Int GetEnemyTilePosition() {
    Vector3Int enemyposition = tilemap.WorldToCell(enemy.transform.position);

    // Rounding up each component
    enemyposition.x = Mathf.CeilToInt(enemyposition.x);
    enemyposition.y =
        Mathf.CeilToInt(enemyposition.y - 2 * enemy.transform.localScale.y);

    // Return the rounded-up coordinates
    return enemyposition;
  }
  public Vector3Int GetBossTilePosition() {
    Vector3Int bossposition = tilemap.WorldToCell(boss.transform.position);

    // Rounding up each component
    bossposition.x =
        Mathf.CeilToInt(bossposition.x + boss.transform.localScale.x / 3);
    bossposition.y = Mathf.CeilToInt(bossposition.y);

    // Debug.Log("X " + bossposition.x + ", Y " + bossposition.y);
    //  Return the rounded-up coordinates
    return bossposition;
  }
  //private float spawnTimer = 0f;   // Timer for controlling spawn frequency
  public float spawnInterval = 5f; // Time interval in seconds between spawns

  public int GetXBounds() {
    var bounds = tilemap.cellBounds;
    return bounds.size.x; // Return the total size on X axis
  }

  public int GetYBounds() {
    var bounds = tilemap.cellBounds;
    return bounds.size.y; // Return the total size on Y axis
  }

  private void Start() {
    // Initialize the 2D array based on the bounds of the tilemap
    int xSize = GetXBounds();
    int ySize = GetYBounds();

    tileData = new int [xSize][]; // Create the jagged array
    for (int i = 0; i < xSize; i++) {
      tileData[i] = new int[ySize]; // Initialize each row
    }
  }

  public void PopulateArray() {
    // Get all the tile positions from the tilemap
    List<Vector3Int> tilePositions = ReturnTiles();

    // Loop through the bounds of the 2D array
    for (int i = 0; i < tileData.Length; i++) {
      for (int j = 0; j < tileData[i].Length; j++) {
        // Convert i and j into tilemap coordinates
        Vector3Int position = new Vector3Int(i + tilemap.cellBounds.min.x,
                                             j + tilemap.cellBounds.min.y, 0);

        // Check if the current position exists in the tilePositions list
        if (tilePositions.Contains(position)) {
          // Set value in the array to 1 if the tile exists at that position
          tileData[i][j] = 1;
        } else {
          // Otherwise set it to 0
          tileData[i][j] = 0;
        }

        target = new Vector2Int(GetPlayerTilePosition().x,
                                GetPlayerTilePosition().y);
        // Debug.Log(start.ToString());

        start =
            new Vector2Int(GetEnemyTilePosition().x, GetEnemyTilePosition().y);
        // Debug.Log(target.ToString());
      }
    }
  }

  public void PrintArray() {
    for (int i = 0; i < tileData.Length; i++) {
      string row = ""; // Create an empty string to hold the current row
      for (int j = 0; j < tileData[i].Length; j++) {
        row += tileData[i][j] +
               " "; // Append each element to the row string with a space
      }
      Debug.Log(row); // Print the row to the console
    }
  }

  void Update() { // Other methods
    CreateFloor();
    // EnemyMovement();
    DestroyTile();

    bombCount = boss.GetComponent<BossDecisionMaking>().bombCount;
    // Update the cooldown timer
    spawnCooldownTimer -= Time.deltaTime;
    //  Debug.Log(platformFieldActive);
    // Calculate the distance the player has moved since the last recorded
    // position
    float playerMovement =
        Vector3.Distance(lastPlayerPosition, player.transform.position);

    // Check if the player has moved the required distance and the cooldown
    // timer has elapsed
    if (playerMovement >= movementThreshold && spawnCooldownTimer <= 0f) {
      bool spawnRandomObject =
          Random.Range(0, 100) < 50; // Adjust probability as needed

      // Spawn random object if no platform field is active and floortrue
      // condition is met
      if (spawnRandomObject && bombCount != 2) {

        randSpawner();

        Debug.Log("Spawning something");
        spawnCooldownTimer = spawnCooldown; // Reset the cooldown timer
        lastPlayerPosition =
            player.transform.position; // Update last position after spawning
      }
      // Spawn platform field if within the probability and no active platform
      // field exists
      else if (bombCount == 2 && platformFieldActive) {

        int count = 1;
        for (int i = 0; i < count; i++) {

          generatePlatformField(maxX, maxY);
        }
        platformFieldActive = false;
      } else if (bombCount != 2) {
        bool spawnRand = Random.Range(0, 100) < oddspawns();
        if (spawnRand) {
          Debug.Log("NUKE");
          CreateNukeHolder(maxX - 10, maxY);

          currentKillCount = 0;
        }
      }
    }

    // Reset conditions based on player's position
    if (player.transform.position.x < startpos &&
        player.transform.position.x > stoppos) {
      floortrue = true;
      platformFieldActive = false; // Allow spawning of new platform fields
                                   // outside the designated area
    }

    if (Input.GetKeyDown(KeyCode.P)) {
      tilebreaker();
    }
  }

  public void CreateTile() {
    maxX = extreme().x;
    maxY = extreme().y;
    Debug.Log("Max x is " + maxX);
    Debug.Log("Max y is " + maxY);

    // Remove the inner loop to avoid changing the Y axis
    // We're only going to place tiles in a straight line along the X axis
    Vector3 worldPosition = new Vector3(maxX, maxY, 0);
    Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

    // Place the tiles in a horizontal line
    for (int i = 0; i < 4; i++) // 4 tiles in a line
    {
      Vector3Int newPosition =
          cellPosition + new Vector3Int(i, 0, 0); // Only changing X axis
      tilemap.SetTile(newPosition, Tile);
    }
  }

  public void DestroyTile() {
    minX = Minima().x;
    minY = Minima().y;
    // Debug.Log(Minima().x + " ldldld " + Minima().y);
    Vector3 worldPosition = new Vector3(minX, minY, 0);
    // Convert world position to cell position
    Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            // Set the tile at the calculated position (replace 'yourTile' with your
            // actual tile reference)
            //Debug.Log("cell --------" + cellPosition.x);
    if (GetBossTilePosition().x > cellPosition.x - 10) {
      Vector3Int newPosition = cellPosition;
      tilemap.SetTile(newPosition, null);
    }
  }
  public Vector2Int Minima() {
    ReturnTiles();
    // Debug.Log(tilePositions.Count);

    // Initialize minX to a large positive number
    int minX = int.MaxValue;
    int minY = 0; // To store the corresponding y value

    // Find the minimum x and the corresponding y value
    foreach (var position in tilePositions) {
      if (position.x < minX) {
        minX = position.x; // Update minX
        minY = position.y; // Update corresponding y when minX changes
      }
    }

    // Return the minimum x and its corresponding y
    return new Vector2Int(minX, minY);
  }

  public Vector2Int extreme() {
    ReturnTiles();
    // Debug.Log(tilePositions.Count);
    int maxX = 0;               // Initialize to minimum integer value
    int correspondingY = 0 - 2; // To store the y value corresponding to maxX

    // Find the maximum x and the corresponding y value
    foreach (var position in tilePositions) {
      if (position.x > maxX) {
        maxX = position.x;   // Update maxX
        correspondingY = -2; // Update corresponding y when maxX changes
      }
    }

    return (new Vector2Int(maxX, correspondingY));
  }

  public void CreateFloor() {
    maxX = extreme().x;
    maxY = extreme().y;
    // Debug.Log("Max x is " + maxX);
    // Debug.Log("Max y is " + maxY);
    if (GetPlayerTilePosition().x > maxX - 190) {

      Vector3 worldPosition = new Vector3(maxX, maxY, 0);
      Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

      // Place the tiles in a horizontal line
      for (int i = 0; i < 2; i++) // 4 tiles in a line
      {
        Vector3Int newPosition =
            cellPosition + new Vector3Int(1, i, 0); // Only changing X axis
        tilemap.SetTile(newPosition, Tile);
        Vector3Int newPosition2 =
            cellPosition + new Vector3Int(1, i + 28, 0); // Only changing X axis
        tilemap.SetTile(newPosition2, Tile2);
      }
    }
  }

  public void CreateBlock(int maxX, int maxY) {

    int steps = 5; // Number of steps
    int width = 7; // Width of each step

    for (int i = 0; i < steps; i++) // Loop for each stair level
    {
      for (int j = 0; j < width;
           j++) // Loop to place tiles for the width of each step
      {
        // Set the tile based on current step and width position
        tilemap.SetTile(new Vector3Int(maxX + j - 6, maxY + i + 2, 0), Tile2);
      }
    }
  }
  public void Createplatform(int maxX, int maxY) {
    for (int i = 0; i < 5; i++) {
      tilemap.SetTile(new Vector3Int(maxX + i, maxY, 0), Tile2);
    }
  }
  public void CreateHalfPyramid(int maxX, int maxY) {

    int baseWidth = 7; // Width of the base of the pyramid
    int height = 5;    // Height of the pyramid

    for (int i = 0; i < height; i++) // Loop through each level of the pyramid
    {
      int rowWidth = baseWidth - i * 2; // Decrease the width for each level up
      int startX = maxX + i; // Adjust the starting X position to center the row

      for (int j = 0; j < baseWidth; j++) {
        if (j < i || j >= baseWidth - i) {
          // Set outer tiles on each row to null to create the pyramid shape
          tilemap.SetTile(new Vector3Int(startX + j - 6, maxY + i + 2, 0),
                          null);
        } else {
          // Set tile in the middle of the row to create the pyramid shape
          tilemap.SetTile(new Vector3Int(startX + j - 6, maxY + i + 2, 0),
                          Tile2);
        }
      }
    }
  }
  public void CreateUpsideDownPyramid(int maxX, int maxY) {
    ClearTilesForRand(maxX - 5, maxX - 1);

    int height = 5; // Height of the pyramid

    for (int i = 0; i < height; i++) // Loop through each level of the pyramid
    {
      // Calculate the starting X position for each row to center the pyramid
      int startX = maxX - i;
      int rowWidth = 2 * i + 1; // Row width increases as we go down

      for (int j = 0; j < rowWidth; j++) // Place tiles in the row
      {
        // Set tile at calculated position
        tilemap.SetTile(new Vector3Int(startX + j - 4, maxY + i + 2, 0), Tile2);
      }
    }
  }

  public void randSpawner()

  {
    extreme();
    // CreateBlock(); = 1
    // CreateFullPyramid(); =2
    // CreateHalfPyramid(); =3  needs work
    // CreateUpsideDownPyramid(); =4
    // CreatePlatform
    maxX = maxX - 10;
    int rng = Random.Range(1, 9);
    Debug.Log(rng);
    switch (rng) {
    case 1:
      ClearTilesForRand(maxX - 5, maxX - 1);
      CreateBlock(maxX, maxY);
      generateBlock(maxX - 7, maxY + 3);

      break;
    case 2:
      ClearTilesForRand(maxX - 60, maxX - 8);
      CreateAnnoyingStairsLeft(maxX - 22, maxY);
      CreateBlock(maxX - 24 - 2, maxY + 17);
      CreateBlock(maxX - 20 - 2, maxY + 17);
      CreateBlock(maxX - 24 - 2, maxY + +22);
      CreateBlock(maxX - 20 - 2, maxY + +22);
      CreateAnnoyingStairs(maxX - 30, maxY);
      CreateBlock(maxX - 24 - 30, maxY + 17);
      CreateBlock(maxX - 20 - 30, maxY + 17);
      CreateBlock(maxX - 24 - 30, maxY + +22);
      CreateBlock(maxX - 20 - 30, maxY + +22);
      CreateBlock(maxX - 13 - 30, maxY);
      CreateBlock(maxX - 8 - 30, maxY);
      CreateBlock(maxX - 3 - 30, maxY);

      break;
    case 3:
      CreateHalfPyramid(maxX, maxY);

      break;
    case 4:
      int rng2 = Random.Range(3, 9);
      Createplatform(maxX, maxY + rng2 + 1);

      break;
    case 5:

      Createplatform(maxX - 4, maxY + 3);
      CreateBlock(maxX, maxY);

      break;
    case 6:
      CreateHalfPyramid(maxX - 8, maxY);
      CreateBlock(maxX - 1, maxY);

      break;
    case 7:
      int rng3 = Random.Range(3, 9);
      Createplatform(maxX, maxY + rng3 + 1);

      break;

    case 8: // Maybe spawn in a bunch of bad guys here
      ClearTilesForRand(maxX - 32, maxX);
      createPillar(maxX, maxY);
      createPillar(maxX - 30, maxY);
      Instantiate(enemy, new Vector3(maxX, maxY, 0), Quaternion.identity);
      Debug.Log("PILLAR");
      break;

    default:
      CreateUpsideDownPyramid(maxX, maxY);
      break;
    }
  }
  public void setNull(int start, int finish) {
    Vector3Int startingPosition = new Vector3Int(start, 0, 0);
    for (int i = start; i < finish; i++) {
      for (int j = 0; j < 26; j++) {
        Vector3Int tilePosition =
            new Vector3Int(startingPosition.x + i, startingPosition.y + j, 0);
        tilemap.SetTile(tilePosition, null);
      }
    }
  }

  public void CreateNukeHolder(int maxX, int maxY) {
    ClearTilesForRand(maxX - 25, maxX - 1);
    generateBlock(maxX - 6, maxY);
    generateBlock(maxX - 5, maxY);
    generateBlock(maxX - 4, maxY);
    generateBlock(maxX - 3, maxY);
    generateBlock(maxX - 2, maxY);
    generateBlock(maxX - 1, maxY);
    generateBlock(maxX - 6, maxY + 1);
    generateBlock(maxX - 6, maxY + 2);
    generateBlock(maxX - 6, maxY + 3);
    generateBlock(maxX - 1, maxY + 1);
    generateBlock(maxX - 1, maxY + 2);
    generateBlock(maxX - 1, maxY + 3);
    Instantiate(nuke, new Vector3(maxX - 3, maxY + 4, 0), Quaternion.identity);
  }
  public void createPillar(int maxX, int maxY) {
    CreateBlock(maxX, maxY + 5);
    CreateBlock(maxX, maxY + 10);
    CreateBlock(maxX, maxY + 15);
    CreateBlock(maxX, maxY + 20);
    Createplatform(maxX - 4, maxY + 27);
    Createplatform(maxX - 6, maxY + 27);
  }
  public void CreateAnnoyingStairs(int maxX, int maxY) { // Bottom part

    maxX = maxX - 20;
    generateBlock(maxX, maxY);
    generateBlock(maxX, maxY + 1);
    generateBlock(maxX, maxY + 2);
    generateBlock(maxX, maxY + 3);
    generateBlock(maxX, maxY + 4);

    generateBlock(maxX - 1, maxY);
    generateBlock(maxX - 1, maxY + 1);
    generateBlock(maxX - 1, maxY + 2);
    generateBlock(maxX - 1, maxY + 3);
    generateBlock(maxX - 1, maxY + 4);
    generateBlock(maxX - 2, maxY);
    generateBlock(maxX - 2, maxY + 1);
    generateBlock(maxX - 2, maxY + 2);
    generateBlock(maxX - 2, maxY + 3);
    generateBlock(maxX - 3, maxY);
    generateBlock(maxX - 3, maxY + 1);
    generateBlock(maxX - 3, maxY + 2);
    generateBlock(maxX - 4, maxY);
    generateBlock(maxX - 4, maxY + 1);
    generateBlock(maxX - 5, maxY);
    generateBlock(maxX - 5, maxY + 1);
    generateBlock(maxX - 6, maxY);
    generateBlock(maxX - 7, maxY);
    generateBlock(maxX - 8, maxY);
    generateBlock(maxX - 9, maxY);
    generateBlock(maxX - 10, maxY);
    // Top part
    generateBlock(maxX - 10, maxY + 7);
    generateBlock(maxX - 9, maxY + 7);
    generateBlock(maxX - 8, maxY + 8);
    generateBlock(maxX - 7, maxY + 9);
    generateBlock(maxX - 6, maxY + 10);
    generateBlock(maxX - 5, maxY + 11);
    generateBlock(maxX - 4, maxY + 12);
    generateBlock(maxX - 3, maxY + 13);
    generateBlock(maxX - 2, maxY + 14);
    generateBlock(maxX - 1, maxY + 15);
    generateBlock(maxX, maxY + 16);
    // Filling in the top
    generateBlock(maxX - 10, maxY + 16);
    generateBlock(maxX - 10, maxY + 15);
    generateBlock(maxX - 10, maxY + 14);
    generateBlock(maxX - 10, maxY + 13);
    generateBlock(maxX - 10, maxY + 12);
    generateBlock(maxX - 10, maxY + 11);
    generateBlock(maxX - 10, maxY + 10);
    generateBlock(maxX - 10, maxY + 9);
    generateBlock(maxX - 10, maxY + 8);

    generateBlock(maxX - 9, maxY + 16);
    generateBlock(maxX - 9, maxY + 15);
    generateBlock(maxX - 9, maxY + 14);
    generateBlock(maxX - 9, maxY + 13);
    generateBlock(maxX - 9, maxY + 12);
    generateBlock(maxX - 9, maxY + 11);
    generateBlock(maxX - 9, maxY + 10);
    generateBlock(maxX - 9, maxY + 9);
    generateBlock(maxX - 9, maxY + 8);

    generateBlock(maxX - 8, maxY + 16);
    generateBlock(maxX - 8, maxY + 15);
    generateBlock(maxX - 8, maxY + 14);
    generateBlock(maxX - 8, maxY + 13);
    generateBlock(maxX - 8, maxY + 12);
    generateBlock(maxX - 8, maxY + 11);
    generateBlock(maxX - 8, maxY + 10);
    generateBlock(maxX - 8, maxY + 9);

    generateBlock(maxX - 7, maxY + 16);
    generateBlock(maxX - 7, maxY + 15);
    generateBlock(maxX - 7, maxY + 14);
    generateBlock(maxX - 7, maxY + 13);
    generateBlock(maxX - 7, maxY + 12);
    generateBlock(maxX - 7, maxY + 11);
    generateBlock(maxX - 7, maxY + 10);

    generateBlock(maxX - 6, maxY + 16);
    generateBlock(maxX - 6, maxY + 15);
    generateBlock(maxX - 6, maxY + 14);
    generateBlock(maxX - 6, maxY + 13);
    generateBlock(maxX - 6, maxY + 12);
    generateBlock(maxX - 6, maxY + 11);

    generateBlock(maxX - 5, maxY + 16);
    generateBlock(maxX - 5, maxY + 15);
    generateBlock(maxX - 5, maxY + 14);
    generateBlock(maxX - 5, maxY + 13);
    generateBlock(maxX - 5, maxY + 12);

    generateBlock(maxX - 4, maxY + 16);
    generateBlock(maxX - 4, maxY + 15);
    generateBlock(maxX - 4, maxY + 14);
    generateBlock(maxX - 4, maxY + 13);

    generateBlock(maxX - 3, maxY + 16);
    generateBlock(maxX - 3, maxY + 15);
    generateBlock(maxX - 3, maxY + 14);
    generateBlock(maxX - 3, maxY + 13);

    generateBlock(maxX - 2, maxY + 16);
    generateBlock(maxX - 2, maxY + 15);
    generateBlock(maxX - 2, maxY + 14);

    generateBlock(maxX - 1, maxY + 16);
    generateBlock(maxX - 1, maxY + 15);

    generateBlock(maxX, maxY + 16);
  }
  public void CreateAnnoyingStairsLeft(int maxX, int maxY) {
    // Start with the top wider part, setting maxY to the starting vertical
    // position

    // Top part (now at the top of the staircase)
    generateBlock(maxX - 10, maxY);
    generateBlock(maxX - 10, maxY + 1);
    generateBlock(maxX - 10, maxY + 2);
    generateBlock(maxX - 10, maxY + 3);
    generateBlock(maxX - 10, maxY + 4);

    generateBlock(maxX - 9, maxY);
    generateBlock(maxX - 9, maxY + 1);
    generateBlock(maxX - 9, maxY + 2);
    generateBlock(maxX - 9, maxY + 3);
    generateBlock(maxX - 9, maxY + 4);

    generateBlock(maxX - 8, maxY);
    generateBlock(maxX - 8, maxY + 1);
    generateBlock(maxX - 8, maxY + 2);
    generateBlock(maxX - 8, maxY + 3);

    generateBlock(maxX - 7, maxY);
    generateBlock(maxX - 7, maxY + 1);
    generateBlock(maxX - 7, maxY + 2);

    generateBlock(maxX - 6, maxY);
    generateBlock(maxX - 6, maxY + 1);

    generateBlock(maxX - 5, maxY);
    generateBlock(maxX - 5, maxY + 1);

    generateBlock(maxX - 4, maxY);
    generateBlock(maxX - 3, maxY);
    generateBlock(maxX - 2, maxY);
    generateBlock(maxX - 1, maxY);
    generateBlock(maxX - 0, maxY);

    // Bottom part (now at the bottom of the staircase)
    generateBlock(maxX, maxY + 7);
    generateBlock(maxX - 1, maxY + 7);
    generateBlock(maxX - 2, maxY + 8);
    generateBlock(maxX - 3, maxY + 9);
    generateBlock(maxX - 4, maxY + 10);
    generateBlock(maxX - 5, maxY + 11);
    generateBlock(maxX - 6, maxY + 12);
    generateBlock(maxX - 7, maxY + 13);
    generateBlock(maxX - 8, maxY + 14);
    generateBlock(maxX - 9, maxY + 15);
    generateBlock(maxX - 10, maxY + 16);

    // Filling in the bottom part
    generateBlock(maxX, maxY + 16);
    generateBlock(maxX, maxY + 15);
    generateBlock(maxX, maxY + 14);
    generateBlock(maxX, maxY + 13);
    generateBlock(maxX, maxY + 12);
    generateBlock(maxX, maxY + 11);
    generateBlock(maxX, maxY + 10);
    generateBlock(maxX, maxY + 9);
    generateBlock(maxX, maxY + 8);

    generateBlock(maxX - 1, maxY + 16);
    generateBlock(maxX - 1, maxY + 15);
    generateBlock(maxX - 1, maxY + 14);
    generateBlock(maxX - 1, maxY + 13);
    generateBlock(maxX - 1, maxY + 12);
    generateBlock(maxX - 1, maxY + 11);
    generateBlock(maxX - 1, maxY + 10);
    generateBlock(maxX - 1, maxY + 9);
    generateBlock(maxX - 1, maxY + 8);

    generateBlock(maxX - 2, maxY + 16);
    generateBlock(maxX - 2, maxY + 15);
    generateBlock(maxX - 2, maxY + 14);
    generateBlock(maxX - 2, maxY + 13);
    generateBlock(maxX - 2, maxY + 12);
    generateBlock(maxX - 2, maxY + 11);
    generateBlock(maxX - 2, maxY + 10);
    generateBlock(maxX - 2, maxY + 9);

    generateBlock(maxX - 3, maxY + 16);
    generateBlock(maxX - 3, maxY + 15);
    generateBlock(maxX - 3, maxY + 14);
    generateBlock(maxX - 3, maxY + 13);
    generateBlock(maxX - 3, maxY + 12);
    generateBlock(maxX - 3, maxY + 11);
    generateBlock(maxX - 3, maxY + 10);

    generateBlock(maxX - 4, maxY + 16);
    generateBlock(maxX - 4, maxY + 15);
    generateBlock(maxX - 4, maxY + 14);
    generateBlock(maxX - 4, maxY + 13);
    generateBlock(maxX - 4, maxY + 12);
    generateBlock(maxX - 4, maxY + 11);

    generateBlock(maxX - 5, maxY + 16);
    generateBlock(maxX - 5, maxY + 15);
    generateBlock(maxX - 5, maxY + 14);
    generateBlock(maxX - 5, maxY + 13);
    generateBlock(maxX - 5, maxY + 12);

    generateBlock(maxX - 6, maxY + 16);
    generateBlock(maxX - 6, maxY + 15);
    generateBlock(maxX - 6, maxY + 14);
    generateBlock(maxX - 6, maxY + 13);

    generateBlock(maxX - 7, maxY + 16);
    generateBlock(maxX - 7, maxY + 15);
    generateBlock(maxX - 7, maxY + 14);
    generateBlock(maxX - 7, maxY + 13);

    generateBlock(maxX - 8, maxY + 16);
    generateBlock(maxX - 8, maxY + 15);
    generateBlock(maxX - 8, maxY + 14);

    generateBlock(maxX - 9, maxY + 16);
    generateBlock(maxX - 9, maxY + 15);

    generateBlock(maxX - 10, maxY + 16);
  }

  public void generateBlock(int maxX, int maxY) // SINGLE;
  {
    tilemap.SetTile(new Vector3Int(maxX, maxY + 2, 0), Tile2);
  }
  public void generateVines(int maxX, int maxY) {
    int rng = Random.Range(1, 4); // Length of vine
    switch (rng) {
    case 1:
      tilemap.SetTile(new Vector3Int(maxX + -6, 25, 0), Vine);
      break;
    case 2:
      tilemap.SetTile(new Vector3Int(maxX + -6, 25, 0), Vine);
      tilemap.SetTile(new Vector3Int(maxX + -6, 24, 0), Vine);
      break;
    case 3:
      tilemap.SetTile(new Vector3Int(maxX + -6, 25, 0), Vine);
      tilemap.SetTile(new Vector3Int(maxX + -6, 24, 0), Vine);
      tilemap.SetTile(new Vector3Int(maxX + -6, 23, 0), Vine);
      break;
    }
  }
  public void generatePlatformField(int maxX, int maxY)

  {
    maxX = extreme().x - 10;

    int temp1 = maxX;
    int temp2 = maxX;
    temp1 -= 22;
    temp1 -= 45;
    temp1 -= 55;

    int b = Mathf.Abs(temp1);
    temp2 -= 30; // Initial adjustment
    int a = Mathf.Abs(temp2);
    ClearTiles(b, a);
    maxX -= 22; // Initial adjustment

    CreateNukeHolder(maxX + 13, maxY);

    // Create platforms in the first segment
    CreatePlatformSegment(maxX + 5, maxY);

    // Adjust maxX for the second segment
    maxX -= 45;
    CreatePlatformSegment(maxX, maxY);

    // Adjust maxX for the third segment
    maxX -= 55;

    CreatePlatformSegment(maxX, maxY);

    // Clear tiles in the specified range
  }

  // Method to create platforms at specified maxX and maxY
  private void CreatePlatformSegment(int maxX, int maxY) {
    Createplatform(maxX - 10, maxY + 4);
    Createplatform(maxX - 1, maxY + 7);
    Createplatform(maxX + 8, maxY + 11);
    Createplatform(maxX + 11, maxY + 20);
    // Createplatform(maxX, maxY + 13);
    Createplatform(maxX - 20, maxY + 8);
    Createplatform(maxX - 27, maxY + 14);
    // Createplatform(maxX - 34, maxY + 16);
    Createplatform(maxX - 17, maxY + 18);
    Createplatform(maxX - 7, maxY + 16);
    Createplatform(maxX - 2, maxY + 23);
    Createplatform(maxX + 16, maxY + 4);
    Createplatform(maxX + 23, maxY + 4);
    Createplatform(maxX + 17, maxY + 13);
    Createplatform(maxX + 17, maxY - 4);
    Createplatform(maxX + 6, maxY - 4);

    Createplatform(maxX + 5, maxY - 4);
    Createplatform(maxX - 3, maxY - 4);
    Createplatform(maxX - 10, maxY - 4);
  }

  // Method to clear tiles in a range
  public void ClearTiles(int start,
                         int end) // This is only for use on our platform field
  {

    for (int i = start; i < end; i++) {
      for (int j = -2; j < 26; j++) {

        tilemap.SetTile(new Vector3Int(i, j, 0), null);
        // Debug.Log("Setting tiles to null");
        tilemap.SetTile(new Vector3Int(i, j, 0), null);
      }
    }
  }

  public void tilebreaker() {
    if (player.GetComponent<Player>().BombCount > 0) {
      for (int i = 0; i < 20; i++) {
        for (int j = 0; j < 6; j++) {

          tilemap.SetTile(new Vector3Int(GetPlayerTilePosition().x + i,
                                         GetPlayerTilePosition().y + j, 0),
                          null);
        }
      }
      player.GetComponent<Player>().BombCount -= 1;
    } else {
      Debug.Log("You dont have any nukes lol");
    }
  }
  public void ClearTilesForRand(int start,
                                int end) // This is only for use on our Rands
  {

    for (int i = start; i < end; i++) {
      for (int j = -0; j < 26; j++) {

        tilemap.SetTile(new Vector3Int(i, j, 0), null);
        // Debug.Log("Setting tiles to null");
        tilemap.SetTile(new Vector3Int(i, j, 0), null);
      }
    }
  }
  public double oddspawns() {
    double prob = 2.0;
    currentKillCount = KillCounter.killCount;
    if (currentKillCount == 0) {
      prob = 0;
    }
    if (currentKillCount > 0 && currentKillCount < 3) {
      prob = 10.0;
    }
    if (currentKillCount > 3 && currentKillCount < 7) {
      prob = 20.0;
    }
    if (currentKillCount > 7) {
      prob = 30.0;
    }
    return prob;
  }
}
}
