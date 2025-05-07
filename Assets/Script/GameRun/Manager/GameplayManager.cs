using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    [SerializeField]private RoadSpawner roadSpawner;        // Tham chiếu đến RoadSpawner
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
            scoreText.text = Mathf.FloorToInt(distance).ToString();
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
    public void RestartGame()
    {
        AudioManager.Instance.PlayVFX("Click");
        gameOverPanel.GetComponentInChildren<Animator>().SetTrigger("End");
        playerMovement.ResetPlayer();
        isDead = false;
        // Reset player position
        playerTransform.position = Vector3.zero;
        // Reset đường cũ
        foreach (GameObject tile in roadSpawner.GetActiveTiles())
        {
            Destroy(tile);
        }
        roadSpawner.ResetRoad(); // Gọi hàm này để reset spawnX và tạo lại đường mới
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
