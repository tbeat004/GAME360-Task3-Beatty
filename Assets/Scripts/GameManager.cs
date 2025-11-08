using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public int score = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Round Settings")]
    public float roundTime = 120f;
    private float timeLeft;
    private bool isRoundActive;

 void Update()
{
    if (!isRoundActive) return;

    timeLeft -= Time.deltaTime;
    if (timeLeft < 0f) timeLeft = 0f;
    EventManager.Instance.TriggerEvent(GameEvents.onTimerTicked, timeLeft);
    if (timeLeft <= 0f)
        EndRound();
}


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        score = 0;
        timeLeft = roundTime;
        isRoundActive = true;
    }

    public void AddScore(int amount)
    {
        score += amount;
        EventManager.Instance.TriggerEvent(GameEvents.onScoreChanged, score);

    }


    private void EndRound()
    {
        isRoundActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Main Menu");
    }


    
}

