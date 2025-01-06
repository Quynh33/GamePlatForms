using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject border; // Gán GameObject làm viền vào đây

    void Start()
    {
        // Ẩn viền khi bắt đầu
        border.SetActive(false);
    }

    // Khi chuột di vào button
    public void OnPointerEnter(PointerEventData eventData)
    {
        border.SetActive(true); // Hiện viền lên
    }

    // Khi chuột rời khỏi button
    public void OnPointerExit(PointerEventData eventData)
    {
        border.SetActive(false); // Ẩn viền đi
    }
}

