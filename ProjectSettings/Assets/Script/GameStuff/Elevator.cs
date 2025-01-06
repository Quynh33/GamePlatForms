using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform topPosition; // Vị trí trên cùng
    public Transform bottomPosition; // Vị trí dưới cùng
    public float speed = 2.0f; // Tốc độ di chuyển của thang máy
    private bool goingUp = false; // Biến kiểm tra hướng di chuyển
    private bool isMoving = false; // Biến kiểm tra trạng thái di chuyển của thang máy
    private bool canActivate = true; // Biến kiểm tra xem thang máy có thể kích hoạt

    void Update()
    {
        // Kiểm tra nếu người chơi bấm phím E
        if (Input.GetKeyDown(KeyCode.E) && canActivate)
        {
            isMoving = true; // Bắt đầu di chuyển thang máy
            canActivate = false; // Ngăn không cho thang máy kích hoạt lại ngay lập tức
        }

        // Nếu thang máy đang di chuyển
        if (isMoving)
        {
            // Nếu thang máy đang đi lên
            if (goingUp)
            {
                transform.position = Vector3.MoveTowards(transform.position, topPosition.position, speed * Time.deltaTime);
                if (transform.position == topPosition.position)
                {
                    goingUp = false; // Khi đến vị trí trên cùng, đảo chiều
                    isMoving = false; // Ngừng di chuyển
                    canActivate = true; // Cho phép kích hoạt lại
                }
            }
            // Nếu thang máy đang đi xuống
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, bottomPosition.position, speed * Time.deltaTime);
                if (transform.position == bottomPosition.position)
                {
                    goingUp = true; // Khi đến vị trí dưới cùng, đảo chiều
                    isMoving = false; // Ngừng di chuyển
                    canActivate = true; // Cho phép kích hoạt lại
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Gắn player vào thang máy
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Gắn player vào thang máy
        }
    }
}