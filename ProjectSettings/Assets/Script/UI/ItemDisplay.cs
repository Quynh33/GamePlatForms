using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;

    public void SetupItem(Sprite image, string name)
    {
        itemImage.sprite = image;
        itemNameText.text = name;

        // Hiển thị object và có thể thêm hiệu ứng fade-in nếu muốn
        gameObject.SetActive(true);
    }

    public void HideItem()
    {
        // Tùy chọn: Thêm hiệu ứng fade-out
        gameObject.SetActive(false);
    }
}

