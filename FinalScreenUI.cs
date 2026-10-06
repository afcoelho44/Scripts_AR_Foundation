using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FinalScreenUI : MonoBehaviour
{
    public Text titleText;
    public Text scoreText;

    public Image backgroundImage;

    public Color winColor = new Color(0.1f, 0.5f, 0.2f);
    public Color loseColor = new Color(0.5f, 0.1f, 0.1f);

    public string gameSceneName = "GameScene";
    public string startSceneName = "StartScene";

    void Start()
    {
        string result = PlayerPrefs.GetString("GameResult", "lose");
        int score = PlayerPrefs.GetInt("Score", 0);

        if (result == "win")
        {
            titleText.text = "Você ganhou!";

            if (backgroundImage != null)
                backgroundImage.color = winColor;
        }
        else
        {
            titleText.text = "Você perdeu!";

            if (backgroundImage != null)
                backgroundImage.color = loseColor;
        }

        scoreText.text = "Quadrados destruídos: " + score;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}