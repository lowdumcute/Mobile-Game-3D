using UnityEngine;
using UnityEngine.Events;

public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField] private string toggleKey = "ShowFPS"; // Khóa lưu trong PlayerPrefs
    [SerializeField] private GameObject targetObject;
    private bool isOn = true;


    void Start()
    {
        isOn = PlayerPrefs.GetInt(toggleKey, 1) == 1; // Mặc định là bật (1)
        UpdateState();
    }

    public void Toggle()
    {
        AudioManager.Instance.PlayVFX("Click");
        isOn = !isOn;
        PlayerPrefs.SetInt(toggleKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateState();
    }

    private void UpdateState()
    {
        if (targetObject != null)
            targetObject.SetActive(isOn);
    }
}
