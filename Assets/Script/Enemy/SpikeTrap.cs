using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        // Không có sức khỏe, không thể bị phá hủy
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Xóa logic liên quan đến health
        if (isRecoiling)
        {
            if (recoilTimer < recoilLenght)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("VAcham");
        // Tấn công người chơi nếu chạm phải
        if (other.CompareTag("Player") && !PlayerMovement.Instance.pState.invincible)
        {
            Attack();
            PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        // Spike Trap không bị ảnh hưởng bởi đòn tấn công, vì vậy logic này có thể bỏ qua
        // Nếu muốn giữ lại hiệu ứng va chạm (recoil), có thể để nguyên phần mã recoil.
    }
}


