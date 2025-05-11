using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    [SerializeField]private RoadSpawner roadSpawner;        // Tham chiếu đến RoadSpawner
    public PlayerMovement playerMovement;        // Tham chiếu đến PlayerMovement
    [Header("Text")]
    public TextMeshProUGUI countdownText;        // TMP Text đếm ngược
    public TextMeshProUGUI scoreText;            // TMP Text điểm
    public TextMeshProUGUI highScoreText;
    [Header("")]
    public Transform playerTransform;            // Vị trí nhân vật
    public GameObject gameOverPanel;            // Panel Game Over

    private bool gameStarted = false;
    private float startX;
    private bool isDead = false;
    public bool isPaused = false;
    private int highScore;

    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        playerMovement.enabled = false;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        StartCoroutine(StartCountdown());
    }


    IEnumerator StartCountdown()
    {
        countdownText.text = "Get Ready!";
        roadSpawner.CreatedRoad();
        yield return new WaitForSeconds(2f);
        gameOverPanel.SetActive(false);
        countdownText.gameObject.SetActive(true);
        AudioManager.Instance.PlayMusic("CountDown");
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayMusic("InGame");
        countdownText.gameObject.SetActive(false);

        playerMovement.enabled = true;
        startX = playerTransform.position.x;
        gameStarted = true;
    }

    void Update()
    {
        if (gameStarted && !isDead)
        {
            float distance = playerTransform.position.x - startX;
            scoreText.text = Mathf.FloorToInt(distance).ToString() + " M";
        }
    }

    public IEnumerator GameOver()
    {
        isDead = true;
        playerMovement.enabled = false;

        // Tính điểm
        float distance = playerTransform.position.x - startX;
        int score = Mathf.FloorToInt(distance);

        // Cập nhật High Score nếu điểm cao hơn
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        countdownText.text = "Game Over!";
        yield return new WaitForSeconds(1f);
        gameOverPanel.SetActive(true);
        countdownText.gameObject.SetActive(false);
        highScoreText.text = "High Score: " + highScore.ToString() + " M";
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlayVFX("Click");
        gameOverPanel.GetComponentInChildren<Animator>().SetTrigger("End");
        playerMovement.ResetPlayer();
        isDead = false;

        // Reset player position
        playerTransform.position = Vector3.zero;

        // Thay vì Destroy, dùng SetActive(false) và đưa lại vào pool
        foreach (GameObject tile in roadSpawner.GetActiveTiles())
        {
            if (tile != null)
            {
                tile.SetActive(false);
            }
        }

        // Reset đường (tự clear activeTiles và đưa về pool)
        roadSpawner.ResetRoad();

        StartCoroutine(StartCountdown());
    }

    public void BackHome()
    {
        AudioManager.Instance.PlayVFX("Click");
        SceneChangeManager.Instance.LoadScene("MainMenu");
        AudioManager.Instance.StopMusic();
    }
    public void TogglePausePanel()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            AudioManager.Instance.ToggleAudioSettingsPanel();
        }
        else
        {
            AudioManager.Instance.ToggleAudioSettingsPanel();
        }
    }
}
