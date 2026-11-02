using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет игровым циклом: счёт, таймер обратного отсчёта, Game Over и перезапуск.
/// Доступен из других скриптов через синглтон GameManager.Instance.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Единственный экземпляр менеджера на сцене.
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    // Длительность раунда в секундах.
    [SerializeField] private float roundTime = 30f;

    private float timeLeft;
    private int score = 0;
    private bool isRunning = true;

    public bool IsRunning => isRunning;

    // Awake вызывается раньше любых Start/Update, поэтому Instance
    // уже назначен, когда Basket впервые обращается к менеджеру.
    private void Awake()
    {
        Instance = this;
        timeLeft = roundTime;
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateScoreText();
        UpdateTimerText();
    }

    private void Update()
    {
        if (!isRunning) return;

        // Time.deltaTime — время прошлого кадра, таймер не зависит от FPS.
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            GameOver();
        }
        UpdateTimerText();
    }

    public void AddScore(int value)
    {
        if (!isRunning) return;
        score += value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Счёт: " + score;
    }

    private void UpdateTimerText()
    {
        // CeilToInt округляет вверх: 29.2 с показываются как 30, а 0 — только в самом конце.
        timerText.text = "Время: " + Mathf.CeilToInt(timeLeft);
    }

    private void GameOver()
    {
        isRunning = false;
        finalScoreText.text = "Ваш счёт: " + score;
        gameOverPanel.SetActive(true);
    }

    // Вызывается кнопкой «Играть снова»: перезагрузка сцены сбрасывает счёт и таймер.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
