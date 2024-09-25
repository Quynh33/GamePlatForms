using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    [SerializeField]protected PlayerMovement player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerMovement.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        if(isRecoiling)
        {
            if(recoilTimer < recoilLenght)
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
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with: " + other.tag);
        Debug.Log("Player invincible state: " + PlayerMovement.Instance.pState.invincible);
        if (other.CompareTag("Player") && !PlayerMovement.Instance.pState.invincible)
        {
            Attack();
            PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
        }
    }
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
        }
    }
    protected virtual void Attack()
    {
        Debug.Log("Attacking player with damage: " + damage);
        PlayerMovement.Instance.TakeDamage(damage);
    }
}
