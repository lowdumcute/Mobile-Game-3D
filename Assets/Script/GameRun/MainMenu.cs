using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        AudioManager.Instance.PlayMusic("InMenu"); // Phát nhạc nền chính khi bắt đầu
    }
    public void PlayGame()
    {
        AudioManager.Instance.PlayVFX("Click"); // Phát âm thanh khi nhấn nút
        SceneChangeManager.Instance.LoadScene("GamePlay"); //
        AudioManager.Instance.StopMusic(); // Dừng nhạc nền chính khi bắt đầu trò chơi
    }
    public void QuitGame()
    {
        AudioManager.Instance.PlayVFX("Click"); // Phát âm thanh khi nhấn nút
        Application.Quit(); // Thoát ứng dụng
    }
    public void OpenAudioSettings()
    {
        AudioManager.Instance.ToggleAudioSettingsPanel(); // Mở/đóng panel âm thanh
    }
}
