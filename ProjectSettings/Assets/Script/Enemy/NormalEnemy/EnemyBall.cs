using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBall : Enemy
{
    [SerializeField] private float attackRange = 5f; // Phạm vi tấn công
    [SerializeField] private GameObject projectilePrefab; // Prefab đạn
    [SerializeField] private int projectileCount = 8; // Số lượng đạn bắn ra
    [SerializeField] private float projectileSpeed = 5f; // Tốc độ của đạn
    [SerializeField] private float attackCooldown = 2f; // Thời gian chờ giữa các đợt bắn
    private float lastAttackTime = 0f;

    protected override void Update()
    {
        base.Update();

        if (isRecoiling) return;

        // Kiểm tra khoảng cách với người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            // Chuyển sang trạng thái tấn công
            ChangeState(EnemyStates.Insect_Chase);
            anmin.SetBool("attacking", true);
            ShootProjectiles();
            lastAttackTime = Time.time;
        }
        else
        {
            anmin.SetBool("attacking", false);
        }
    }

    private void ShootProjectiles()
    {
        float angleStep = 360f / projectileCount; // Góc giữa các viên đạn
        float angle = 0f; // Bắt đầu từ góc 0 độ

        for (int i = 0; i < projectileCount; i++)
        {
            // Tạo đạn
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Tính toán hướng đạn
            float projectileDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 projectileDirection = new Vector2(projectileDirX, projectileDirY).normalized;

            // Thiết lập vận tốc cho đạn
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0; // Vô hiệu hóa trọng lực
                rb.velocity = projectileDirection * projectileSpeed;
            }

            angle += angleStep; // Cập nhật góc cho viên đạn tiếp theo
        }
    }


    public override void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (health <= 0) return; 

    
        health -= _damageDone; 
     
        if (orangerBlood != null)
        {
            GameObject bloodEffect = Instantiate(orangerBlood, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 2f);
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
        MakeLoot();

        Destroy(gameObject, _destroyTime);
    }
}