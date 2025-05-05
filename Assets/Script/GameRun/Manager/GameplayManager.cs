using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    public PlayerMovement playerMovement;        // Tham chiếu đến PlayerMovement
    public TextMeshProUGUI countdownText;        // TMP Text đếm ngược
    public TextMeshProUGUI scoreText;            // TMP Text điểm
    public Transform playerTransform;            // Vị trí nhân vật
    public GameObject gameOverPanel;            // Panel Game Over

    private bool gameStarted = false;
    private float startX;
    private bool isDead = false;
    public bool isPaused = false;
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
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(2f);
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
            scoreText.text = "Score: " + Mathf.FloorToInt(distance).ToString();
        }
    }

    public IEnumerator GameOver()
    {
        isDead = true;
        playerMovement.enabled = false;
        countdownText.text = "Game Over!";
        yield return new WaitForSeconds(1f);
        gameOverPanel.SetActive(true);
        countdownText.gameObject.SetActive(true);
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
