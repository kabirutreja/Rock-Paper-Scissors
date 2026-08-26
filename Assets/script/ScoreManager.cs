using UnityEngine;
using TMPro; // if using legacy UI Text, use "using UnityEngine.UI;" instead

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText; // drag your UI Text here

    private float score = 0f;

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
        UpdateScoreUI();
    }

    public void AddScore(float amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        int displayScore = Mathf.RoundToInt(score);
        scoreText.text = displayScore.ToString("000"); // 0 -> 000, 1 -> 001, 101 -> 101
    }
}