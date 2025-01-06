using UnityEngine;

public class SmallJerry : Enemy
{
    [SerializeField] private float swimAmplitude = 1f; // Biên độ dao động
    [SerializeField] private float swimFrequency = 1f; // Tần số dao động
    [SerializeField] private CircleCollider2D eggSwimArea; // Vùng bơi ban đầu (quả trứng)
    [SerializeField] private PolygonCollider2D freeSwimArea; // Vùng bơi tự do (lớn hơn)
    [SerializeField] private GameObject deathEffect; // Hiệu ứng khi sứa chết
    private float originalY; // Lưu vị trí ban đầu theo trục Y
    private Vector2 direction = Vector2.left; // Hướng di chuyển
    private bool isFreeSwimming = false; // Trạng thái bơi tự do

    protected override void Start()
    {
        base.Start();
        originalY = transform.position.y;

        if (eggSwimArea == null)
        {
            Debug.LogError("Chưa gán PolygonCollider2D cho vùng trứng!");
        }
    }

    protected override void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        base.Update();

        // Nếu sứa đã thoát khỏi vùng trứng, cho phép bơi tự do
        if (isFreeSwimming)
        {
            Swim(); // Sứa bơi tự do khi không còn trong vùng trứng
            CheckBounds(freeSwimArea); // Kiểm tra giới hạn vùng bơi lớn hơn
        }
        else
        {
            CheckIfInsideEggArea(); // Kiểm tra xem sứa có còn trong vùng trứng không
        }
    }

    private void Swim()
    {
        // Chuyển động dạng sóng
        float offsetY = Mathf.Sin(Time.time * swimFrequency) * swimAmplitude;
        Vector2 newPosition = (Vector2)transform.position + direction * speed * Time.deltaTime;
        newPosition.y = originalY + offsetY;
        transform.position = newPosition;
    }

    private void CheckBounds(PolygonCollider2D swimArea)
    {
        // Kiểm tra xem sứa có nằm trong vùng bơi không
        if (swimArea != null && !swimArea.OverlapPoint(transform.position))
        {
            // Đảo hướng khi sứa đi ra ngoài vùng bơi
            direction = -direction;
            Flip();
        }
    }

    private void CheckIfInsideEggArea()
    {
        // Kiểm tra nếu sứa vẫn ở trong vùng trứng
        if (eggSwimArea != null && eggSwimArea.OverlapPoint(transform.position))
        {
            // Dừng mọi chuyển động và giữ sứa ở vị trí ban đầu
            transform.position = new Vector2(transform.position.x, originalY);
        }
        else
        {
            // Nếu sứa ra ngoài vùng trứng, cho phép bơi tự do
            isFreeSwimming = true;
        }
    }

    private void Flip()
    {
        // Lật sprite khi đổi hướng
        sr.flipX = !sr.flipX;
    }

    // Gọi khi quả trứng bị phá hủy
    public void ReleaseFromEggArea()
    {
        isFreeSwimming = true; // Chuyển sang trạng thái bơi tự do
        eggSwimArea = null; // Bỏ tham chiếu đến vùng trứng
    }

    public override void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (health <= 0) return; // Nếu đã chết, bỏ qua

        // Kiểm tra xem sứa có đang ở trong vùng trứng hay không
        if (eggSwimArea != null && eggSwimArea.OverlapPoint(transform.position))
        {
            // Nếu sứa đang trong vùng trứng, không nhận sát thương
            return;
        }

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