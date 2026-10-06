using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int pontosParaGanhar = 10;
    public string finalSceneName = "FinalScene";

    public Text scoreText;
    public Text bonusText;

    private int score = 0;
    private bool gameFinished = false;

    [Header("Bônus de velocidade dos inimigos")]
    public float enemySpeedMultiplier = 1f;
    [Tooltip("Multiplicador aplicado durante o bônus.")]
    public float speedBonusMultiplier = 2f;
    [Tooltip("Duração somada a cada bônus coletado, em segundos.")]
    public float speedBonusDuration = 5f;
    [Tooltip("Tempo máximo acumulado ao pegar vários bônus seguidos.")]
    public float maxSpeedBonusDuration = 15f;

    private bool speedBonusActive = false;
    private float speedBonusEndTime = 0f;
    private float baseSpeedMultiplier = 1f;

    private Shooter shooter;
    private PlayerHealth playerHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        score = 0;
        PlayerPrefs.SetInt("Score", score);
        UpdateScoreUI();
        bonusText.text = "";

        shooter = FindObjectOfType<Shooter>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void EnemyDestroyed()
    {
        if (gameFinished) return;

        score++;
        PlayerPrefs.SetInt("Score", score);

        UpdateScoreUI();

        if (score >= pontosParaGanhar)
        {
            WinGame();
        }
    }

    public void PlayerDied()
    {
        if (gameFinished) return;

        gameFinished = true;

        PlayerPrefs.SetString("GameResult", "lose");
        PlayerPrefs.SetInt("Score", score);

        SceneManager.LoadScene(2);
    }

    private void WinGame()
    {
        if (gameFinished) return;

        gameFinished = true;

        PlayerPrefs.SetString("GameResult", "win");
        PlayerPrefs.SetInt("Score", score);

        SceneManager.LoadScene(2);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Quadrados Atingidos: " + score + "/" + pontosParaGanhar;
        }
    }

    public void ActivateDoubleShotBonus()
    {
        if (shooter != null)
        {
            shooter.ActivateDoubleShot(); // o Shooter controla os 5 segundos
        }

        Debug.Log("Bônus de tiro em leque ativado!");
    }

    void Update()
    {
        if (speedBonusActive && Time.time >= speedBonusEndTime)
        {
            enemySpeedMultiplier = baseSpeedMultiplier;
            speedBonusActive = false;

            Debug.Log("Bônus de velocidade terminou.");
        }
    }

    // Cada bônus coletado soma speedBonusDuration ao tempo restante (limitado ao máximo)
    public void ActivateSpeedBonus()
    {
        if (!speedBonusActive)
        {
            baseSpeedMultiplier = enemySpeedMultiplier;
            enemySpeedMultiplier = speedBonusMultiplier;
            speedBonusActive = true;
            speedBonusEndTime = Time.time;
        }

        speedBonusEndTime = Mathf.Min(
            speedBonusEndTime + speedBonusDuration,
            Time.time + maxSpeedBonusDuration);

        Debug.Log($"Bônus de velocidade ativado! Restam {speedBonusEndTime - Time.time:F1}s");
    }

    public void ActivateShieldBonus()
    {
        if (playerHealth != null)
        {
            playerHealth.ActivateShield();
        }

        Debug.Log("Bônus de escudo ativado!");
    }

    public void HealPlayer(int amount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
        }

        Debug.Log("Bônus de vida ativado!");
    }

    public void ShowBonusText(string message)
    {
        if (bonusText != null)
        {
            bonusText.text = message;

            CancelInvoke(nameof(HideBonusText));
            Invoke(nameof(HideBonusText), 2f);
        }
    }

    private void HideBonusText()
    {
        if (bonusText != null)
        {
            bonusText.text = "";
        }
    }
}