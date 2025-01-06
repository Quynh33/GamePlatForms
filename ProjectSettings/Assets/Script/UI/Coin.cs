using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Coin : Powerup
{
    public Inventory playerInventory;       // Để lưu số coin của người chơi
    [Header("Coin Animation Settings")]
    [SerializeField] GameObject animatedCoinPrefab; // Prefab đồng xu để làm hiệu ứng
    [SerializeField] Transform target;             // Vị trí UI mục tiêu (nơi coin sẽ bay đến)
    [SerializeField] int numberOfCoins = 5;        // Số lượng đồng xu hiệu ứng
    [SerializeField] float animationDuration = 1f; // Thời gian bay
    [SerializeField] float spread = 0.5f;          // Độ lan của vị trí coin khi xuất hiện
    [SerializeField] Ease easeType = Ease.InOutQuad; // Kiểu chuyển động

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            // Hiệu ứng nhiều đồng xu bay lên
            StartCoroutine(CreateFlyingCoinEffect());

            // Cập nhật số coin trong Inventory
            if (playerInventory != null)
            {
                playerInventory.coins += numberOfCoins;
            }

            // Gửi tín hiệu cập nhật UI hoặc các sự kiện khác
            if (powerupSignal != null)
            {
                powerupSignal.Raise();
            }

            // Hủy đối tượng đồng xu
            Destroy(gameObject);
        }
    }

    // Coroutine để tạo hiệu ứng đồng xu trải đều qua nhiều khung hình
    IEnumerator CreateFlyingCoinEffect()
    {
        if (animatedCoinPrefab != null && target != null)
        {
            for (int i = 0; i < numberOfCoins; i++)
            {
                // Tạo một coin hiệu ứng tại vị trí của đồng xu hiện tại
                GameObject flyingCoin = Instantiate(animatedCoinPrefab, transform.position, Quaternion.identity);

                // Tạo vị trí ban đầu có độ lan (spread)
                Vector3 startPosition = transform.position + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
                flyingCoin.transform.position = startPosition;

                // Di chuyển đồng xu tới mục tiêu (UI)
                flyingCoin.transform.DOMove(target.position, animationDuration)
                    .SetEase(easeType)
                    .OnComplete(() =>
                    {
                        // Hủy đối tượng coin hiệu ứng sau khi đến UI
                        Destroy(flyingCoin);
                    });

                // Delay giữa các lần tạo đồng xu để giảm tải
                yield return null; // Dừng lại và tiếp tục trong khung hình tiếp theo
            }
        }
    }
}