using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageFireBall : MonoBehaviour
{
    [SerializeField] Vector2 startForceMinMax;
    [SerializeField] float turnSpeed = 1f;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 4f);

        // Tính toán hướng từ boss đến player
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 direction = (playerPosition - transform.position).normalized;

        // Thêm lực vào đạn để nó bay về phía player
        rb.AddForce(direction * Random.Range(startForceMinMax.x, startForceMinMax.y), ForceMode2D.Impulse);
    }

    void Update()
    {
        var _dir = rb.velocity;
        if (_dir != Vector2.zero)
        {
            Vector3 _frontVector = Vector3.right;

            // Tính toán góc quay dựa trên hướng di chuyển
            Quaternion _targetRotation = Quaternion.FromToRotation(_frontVector, _dir);

            // Quay mượt mà từ góc hiện tại đến góc mục tiêu
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the barrage fireball");
            other.GetComponent<PlayerMovement>().TakeDamage(1);
        }
        Destroy(gameObject);
    }
}