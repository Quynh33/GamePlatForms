using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryFish : Enemy
{
    [SerializeField] private float swimAmplitude = 1f; // Biên độ dao động
    [SerializeField] private float swimFrequency = 1f; // Tần số dao động
    [SerializeField] private PolygonCollider2D swimArea; // Khu vực bơi
    [SerializeField] private GameObject deathEffect; // Hiệu ứng khi sứa chết

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab; // Prefab đạn
    [SerializeField] private float fireRate = 2f; // Thời gian giữa các lần bắn
    [SerializeField] private float bulletSpeed = 5f; // Tốc độ đạn
    [SerializeField] private Transform firePoint; // Điểm bắn đạn

    private float fireTimer = 0f; // Bộ đếm thời gian bắn
    private float originalY; // Lưu vị trí ban đầu theo trục Y
    private Vector2 direction = Vector2.left; // Hướng di chuyển

    protected override void Start()
    {
        base.Start();
        originalY = transform.position.y;

        if (swimArea == null)
        {
            Debug.LogError("Chưa gán PolygonCollider2D cho JerryFish!");
        }

        if (firePoint == null)
        {
            Debug.LogError("Chưa gán firePoint cho JerryFish!");
        }
    }

    protected override void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        base.Update();
        Swim();
        CheckBounds();
        HandleShooting(); // Xử lý bắn đạn
    }

    private void Swim()
    {
        // Chuyển động dạng sóng
        float offsetY = Mathf.Sin(Time.time * swimFrequency) * swimAmplitude;
        Vector2 newPosition = (Vector2)transform.position + direction * speed * Time.deltaTime;
        newPosition.y = originalY + offsetY;
        transform.position = newPosition;
    }

    private void CheckBounds()
    {
        // Kiểm tra xem sứa có nằm trong vùng bơi không
        if (swimArea != null && !swimArea.OverlapPoint(transform.position))
        {
            // Đảo hướng khi sứa đi ra ngoài vùng bơi
            direction = -direction;
            Flip();
        }
    }

    private void Flip()
    {
        // Lật sprite khi đổi hướng
        sr.flipX = !sr.flipX;
    }

    private void HandleShooting()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            FireBulletPattern(); // Xử lý bắn đạn theo mẫu
        }
    }

    private void FireBulletPattern()
    {
        // Bắn đạn theo chiều dọc
        FireBullet(Vector2.down);

        // Bắn đạn theo hình tròn
        for (int angle = 0; angle < 360; angle += 45) // Chia thành 8 viên đạn
        {
            float radian = angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            FireBullet(direction); // Gọi hàm FireBullet với hướng tính toán từ góc
        }
    }

    private void FireBullet(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Tạo đạn tại vị trí firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction.normalized * bulletSpeed;  // Tính toán tốc độ và hướng cho đạn
        }

        // Hủy đạn sau 5 giây
        Destroy(bullet, 5f);
    }

    public override void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (health <= 0) return; // Nếu đã chết, bỏ qua

        health -= _damageDone; // Giảm máu
        Debug.Log(gameObject.name + " nhận sát thương: " + _damageDone);

        // Hiệu ứng máu (tùy chỉnh hoặc sử dụng sẵn từ Enemy)
        if (orangerBlood != null)
        {
            GameObject bloodEffect = Instantiate(orangerBlood, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 2f); // Xóa hiệu ứng máu sau 2 giây
        }

        // Xử lý trạng thái chết
        if (health <= 0)
        {
            Death(0f);
        }
    }

    public override void Death(float _destroyTime)
    {
        Debug.Log(gameObject.name + " đã chết!");

        // Hiệu ứng khi sứa chết
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Gọi loot nếu có
        MakeLoot();

        // Xóa GameObject
        Destroy(gameObject, _destroyTime);
    }

    protected override void UpdateEnemyStates()
    {
        base.UpdateEnemyStates();
        // Cập nhật trạng thái JerryFish nếu cần
    }
}