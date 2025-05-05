using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volume Sliders")]
    public GameObject AudioSettingsPanel; // Panel UI chứa các slider âm thanh
    public Slider musicVolumeSlider;
    public Slider vfxVolumeSlider;

    [Header("Audio Clips")]
    public List<AudioClip> musicClips = new();  // Danh sách nhạc nền
    public List<AudioClip> vfxClips = new();    // Danh sách các VFX

    [Header("Audio Source")]
    public AudioSource musicSource;             // AudioSource duy nhất để phát nhạc nền
    public GameObject vfxAudioSourcePrefab;     // Prefab chứa AudioSource dùng để phát VFX

    [Header("Volume Values")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float vfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Đọc volume đã lưu (nếu có)
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("VFXVolume"))
            vfxVolume = PlayerPrefs.GetFloat("VFXVolume");

        // Cập nhật UI Slider và âm lượng thực tế
        musicVolumeSlider.value = musicVolume;
        vfxVolumeSlider.value = vfxVolume;

        if (musicSource != null)
            musicSource.volume = musicVolume;

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        vfxVolumeSlider.onValueChanged.AddListener(OnVFXVolumeChanged);
    }

    private void Start()
    {
        PlayMusic("InMenu"); // Phát nhạc nền chính khi bắt đầu
        AudioSettingsPanel.SetActive(false); // Ẩn panel âm thanh khi bắt đầu
    }
    public void ToggleAudioSettingsPanel()
    {
        PlayVFX("Click"); // Phát âm thanh khi nhấn nút
        bool isActive = !AudioSettingsPanel.activeSelf;
        AudioSettingsPanel.SetActive(isActive);

        if (GameplayManager.Instance == null) return;

        GameplayManager.Instance.isPaused = isActive;
        Time.timeScale = isActive ? 0f : 1f;
    }
    private void OnMusicVolumeChanged(float value)
    {
        musicVolume = value;
        if (musicSource != null)
            musicSource.volume = musicVolume;

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    private void OnVFXVolumeChanged(float value)
    {
        vfxVolume = value;

        PlayerPrefs.SetFloat("VFXVolume", vfxVolume);
        PlayerPrefs.Save();
    }


    // === Phát nhạc nền bằng tên ===
    public void PlayMusic(string clipName)
    {
        AudioClip clip = musicClips.Find(c => c != null && c.name == clipName);
        if (clip == null)
        {
            Debug.LogWarning($"Music clip '{clipName}' not found.");
            return;
        }

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    // === Phát VFX bằng tên tại vị trí ===
    public void PlayVFX(string clipName)
    {
        AudioClip clip = vfxClips.Find(c => c != null && c.name == clipName);
        if (clip == null)
        {
            Debug.LogWarning($"VFX clip '{clipName}' not found.");
            return;
        }

        // Spawn làm con của AudioManager
        GameObject go = Instantiate(vfxAudioSourcePrefab, transform);
        AudioSource source = go.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = vfxVolume;
        source.Play();

        Destroy(go, clip.length);
    }

}
