using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gameOverUI; // assign your Game Over panel here

    [Header("References")]
    public EnemySpawner enemySpawner; // assign your EnemySpawner GameObject here
    public GameObject player;         // assign your Player GameObject here

    [Header("Scenes")]
    public string startMenuSceneName = "StartMenu"; // set this to your actual start menu scene name

    public bool IsGameOver { get; private set; } = false;

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
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (enemySpawner == null)
            enemySpawner = FindObjectOfType<EnemySpawner>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    public void GameOver()
{
    if (IsGameOver) return;

    IsGameOver = true;

    // Unlock and show cursor immediately, first thing
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    Debug.Log("GAME OVER");

    if (enemySpawner != null)
        enemySpawner.StopSpawning();

    ClearAllEnemies();
    DisablePlayerMovement();

    if (gameOverUI != null)
        gameOverUI.SetActive(true);

    // Optional: freeze the game entirely
    // Time.timeScale = 0f;
}

    void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    void DisablePlayerMovement()
    {
        if (player == null) return;

        PlayerXY movementScript = player.GetComponent<PlayerXY>();
        if (movementScript != null)
            movementScript.enabled = false;
    }

    public void RestartGame()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartMenu()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(startMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}