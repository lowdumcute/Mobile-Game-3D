using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu đối tượng chạm vào có component PlayerMovement
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            StartCoroutine(player.Die());
        }
    }
}
