using UnityEngine;
using TMPro;

public class Result : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        // Lấy TMP Text nếu chưa có
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();

        // Cập nhật điểm mỗi lần được bật
        scoreText.text = "Your Score: " + GameplayManager.Instance.scoreText.text;
    }
}
