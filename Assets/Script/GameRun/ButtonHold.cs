using UnityEngine;
using UnityEngine.UI;  // Để sử dụng UI Button
using UnityEngine.EventSystems;  // Để sử dụng sự kiện pointer

public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMovement playerMovement;  // Tham chiếu đến script PlayerMovement (để gọi phương thức Slide/StopSlide)

    public void OnPointerDown(PointerEventData eventData)
    {
        // Gọi phương thức Slide khi nhấn nút
        playerMovement.Slide();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Gọi phương thức StopSlide khi thả nút
        playerMovement.StopSlide();
    }
}
