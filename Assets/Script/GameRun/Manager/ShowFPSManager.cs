using UnityEngine;
using TMPro;

public class ShowFPSManager : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;

    private float deltaTime;
    private bool isOn;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0; // Tắt VSync để targetFrameRate có hiệu lực
        isOn = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        fpsText.gameObject.SetActive(isOn);

        // Nếu không bật thì vô hiệu hóa script để ngăn chạy Update
        if (!isOn)
            enabled = false;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps) + " FPS";
    }

    public void ToggleFPSDisplay()
    {
        isOn = PlayerPrefs.GetInt("ShowFPS", 1) == 1;
        fpsText.gameObject.SetActive(isOn);
        enabled = isOn; // Bật/tắt Update tuỳ trạng thái
    }
}
