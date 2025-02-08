using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.VisualScripting;

using UnityEngine;
using TMPro;

public class BossDecisionMaking : MonoBehaviour {
  public GameObject Player;
  public GameObject Boss;
  public float playerpos;
  public float bosspos;
  public Vector3 moveVector;
  public TextMeshProUGUI thoughtBubble;
  [SerializeField]
  float movementSpeed = 0.00f;
  public int bombCount = 0;
  public Material myMaterial;
  public Color MYcolor;
  private SpriteRenderer bossSpriteRenderer;
  public GameObject enemy;
  public GameObject Nuke;
  public int count;
  public bool SpawnEnemy = false;
  public int lastHealth = -1;
  float initpos;
  float finalpos;
  float countdown = 5f;
  bool measuringSpeed = false;
  public Controller controller;
  void Start() {
    bossSpriteRenderer = Boss.GetComponent<SpriteRenderer>();
    thoughtBubble.text = "";
  }
  // Update is called once per frame
  void Update() {
        if(Player.transform.position.x <= Boss.transform.position.x)
        {
            UpdateThoughtBubble("Sweet dreams bozo");
            Destroy(Player);
            controller.GameOver();
        }
        
    PlayerReactions();
    bossSpriteRenderer.color = MYcolor;
    playerpos = Player.transform.position.x;
    bosspos = Boss.transform.position.y;
    moveVector = new Vector3(playerpos, bosspos, 0);
    Boss.transform.position =
        Vector3.MoveTowards(Boss.transform.position, moveVector, movementSpeed);
    if (Player.GetComponent<Player>().BombCount < 4) {
      BossSpeed();
    } else {
      DynamicSpeed();
    }

    NukeReactions();
    EnemyReactions();
  }
  public void BossSpeed() {
    if (Player.transform.position.x - Boss.transform.position.x > 40) {
      movementSpeed = 0.2f;
      // Debug.Log("I will not let you escape");
      UpdateThoughtBubble("I will not let you escape");
    } else if (Player.transform.position.x - Boss.transform.position.x < 30) {
      movementSpeed = 0.1f;
    }
  }
  private void NukeReactions() {
    bombCount = Player.GetComponent<Player>().BombCount;
    switch (bombCount) {
    case 0:
      // Debug.Log("Bro got no bombs LOL");
      UpdateThoughtBubble("Bro got no bombs LOL");
      MYcolor = Color.white; // White color
      break;
    case 1:
      // Debug.Log("One bomb not gonna kill me lil bro :3");
      UpdateThoughtBubble("One bomb not gonna kill me lil bro :3");
      MYcolor = new Color(0.8943396f, 0.592289f, 0.592289f, 1f); // Light red
      break;
    case 2:
      // Debug.Log("Chill out with the bombs bud");
      UpdateThoughtBubble("Chill out with the bombs bud");
      movementSpeed = 0.5f;
      MYcolor = Color.red;                         // Red color
      StartCoroutine(FlashColor(Color.blue, 300)); // Flash yellow
      break;
    case 3:
      // Debug.Log("That's enough good luck escaping my minions");
      UpdateThoughtBubble("That's enough good luck escaping my minions");
      MYcolor = new Color(0.5018867f, 0.0552f, 0f, 1f); // Dark red
      StartCoroutine(FlashColor(Color.magenta, 3));     // Flash magenta
      break;
    case 4:
      // Debug.Log("Clearly my minions weren't good enough, try these folks out
      // " +
      //           "for size >:(");
      UpdateThoughtBubble("Clearly my minions weren't good enough, try these " +
                          "folks out for size >:(");
      MYcolor = Color.black;                    // Black color
      StartCoroutine(FlashColor(Color.red, 3)); // Flash red
      break;
    case 5:
      UpdateThoughtBubble(
          "You think wiping out my minions will stop me? Think again!");
      // Kill all enemies on screen
      GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
      foreach (GameObject enemy in enemies) {
        Destroy(enemy);
      }
      break;
    }
  }

  public void DynamicSpeed() {
    if (!measuringSpeed) {
      StartCoroutine(CalculateSpeedOverTime());
    }
  }

  private IEnumerator CalculateSpeedOverTime() {
    measuringSpeed = true;
    initpos = Player.transform.position.x;
    countdown = 5f;

    Debug.Log("Starting speed measurement countdown");

    while (countdown > 0) {
      countdown -= Time.deltaTime;
      yield return null;
    }

    finalpos = Player.transform.position.x;
    float speed = (finalpos - initpos) / 5f;
    movementSpeed = (speed * Time.deltaTime) + 0.9f;

    Debug.Log("Count Done - New speed calculated: " + speed);
    measuringSpeed = false;
  }

  public void EnemyReactions() {
    int ens = enemyCount();
    if (ens < 5) {
      // Debug.Log(
      //     "It seems all my minions have been slain, I think I can fix
      //     that!");
      UpdateThoughtBubble("Spawning more enemies");
      // Instantiate(enemy, Player.transform.position + new Vector3(25, 0, 0),
      //           Quaternion.identity);
    } else {
      // Debug.Log("Keep fighting minions!!!!");
      UpdateThoughtBubble("Keep fighting minions!!!!");
    }
  }

  public void PlayerReactions() {
    int health = Player.GetComponent<Player>().health;

    if (health != lastHealth) // Only react if health has changed
    {
      lastHealth = health; // Update lastHealth to current health

      if (health == 0) {
        // Debug.Log("Sweet dreams bozo");
        UpdateThoughtBubble("Sweet dreams bozo");
      } else if (health > 0 && health < 30) {
        // Debug.Log("You're getting weaker, I'm going in for the finishing
        // move");
        UpdateThoughtBubble(
            "You're getting weaker, I'm going in for the finishing move");
        movementSpeed += 0.08f;
      } else if (health > 30 && health < 60) {
        // Debug.Log("A few more enemies will sort out that high health of
        // yours");
        UpdateThoughtBubble(
            "A few more enemies will sort out that high health of yours");
        Instantiate(enemy, Player.transform.position + new Vector3(45, 60, 0),
                    Quaternion.identity);
      } else {
        // Debug.Log("That health won't be that high for too long");
        UpdateThoughtBubble("That health won't be that high for too long");
      }
    }
  }
  public void OnCollisionEnter2D(Collision2D coll) {
        switch (coll.gameObject.tag)
        {
            case "Player":
                // Debug.Log("Sweet dreams bozo");
                UpdateThoughtBubble("Sweet dreams bozo");
                Destroy(coll.gameObject);
                controller.GameOver();
                break;

            case "Nuke":
                Destroy(coll.gameObject);
                Debug.Log("Looks like you missed a nuke dummy");
                UpdateThoughtBubble("Looks like you missed a nuke dummy");
                break;

            case "Enemy":
                
                    // Debug.Log("Rest in peace my loyal minion");
                    UpdateThoughtBubble("Rest in peace my loyal minion");
                    Destroy(coll.gameObject);
                break;
            

                
        }
    }

  public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Enemy")
        {
            // Debug.Log("Rest in peace my loyal minion");
            UpdateThoughtBubble("Rest in peace my loyal minion");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
             UpdateThoughtBubble("Sweet dreams bozo");
             Destroy(collision.gameObject);
             controller.GameOver();

        
        }
  }
  public int enemyCount() {

    count = (GameObject.FindGameObjectsWithTag("Enemy")).Length;
    //Debug.Log(count);
    return count;
  }

  private IEnumerator FlashColor(Color flashColor, int flashes) {
    Color originalColor =
        MYcolor; // Store the original color
                 // Temporarily disable Animator to allow color changes
    Animator animator = Boss.GetComponent<Animator>();
    animator.enabled = false;

    for (int i = 0; i < flashes; i++) {
      bossSpriteRenderer.color = flashColor;
      yield return new WaitForSeconds(0.2f);
      bossSpriteRenderer.color = originalColor;
      yield return new WaitForSeconds(0.2f);
    }

    // Re-enable Animator
    animator.enabled = true;
  }

  private Coroutine thoughtBubbleCoroutine;
  private Queue<(string message, float duration)> messageQueue =
      new Queue<(string, float)>();
  private bool isDisplayingMessage = false;

  private void UpdateThoughtBubble(string message, float duration = 2f) {
    // Enqueue the message and duration
    messageQueue.Enqueue((message, duration));

    // If not currently displaying a message, start the coroutine
    if (!isDisplayingMessage) {
      thoughtBubbleCoroutine = StartCoroutine(ProcessThoughtBubbleQueue());
    }
  }

  private IEnumerator ProcessThoughtBubbleQueue() {
    isDisplayingMessage = true;

    // Process each message in the queue
    while (messageQueue.Count > 0) {
      var (message, duration) = messageQueue.Dequeue();
      thoughtBubble.text = message; // Display the message

      yield return new WaitForSeconds(
          duration); // Wait for the specified duration

      thoughtBubble.text = ""; // Clear the message
    }

    isDisplayingMessage = false;
  }
    

}
