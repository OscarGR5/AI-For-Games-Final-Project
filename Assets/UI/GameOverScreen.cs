using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI killsText;
    public void Setup()
    {
        gameObject.SetActive(true);
        killsText.text = KillCounter.killCount.ToString() + " KILLS";
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("bigScene");   
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
