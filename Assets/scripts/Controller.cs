using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameOverScreen GameOverScreen;

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
       // Debug.Log("--------------------Game Over--------------------");
        GameOverScreen.Setup();
    }
}
