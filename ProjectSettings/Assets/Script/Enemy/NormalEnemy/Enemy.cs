using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected GameObject orangerBlood;
    [SerializeField] protected bool isRecoiling = false;
    protected float recoilTimer;
    [HideInInspector] public Rigidbody2D rb;
    [SerializeField] protected PlayerMovement player;
     public float speed;
    [SerializeField] public float damage;
    [SerializeField] protected int coinReward;
    public LootTable thisLoot;



    protected EnemyStates currentEnemyState;
    protected SpriteRenderer sr;
    public Animator anmin;
    protected enum EnemyStates
    {
        Enemy_Idle,
        Enemy_Flip,

        Insect_Idle,
        Insect_Chase,
        Insect_Stunned,
        Insect_Death,

        Boss_Stage1,
        Boss_Stage2,
        Boss_Stage3,
        Boss_Stage4,
    }
    protected virtual EnemyStates getCurrentEnemyState
    {
        get { return currentEnemyState; }
        set {
             if(currentEnemyState != value)
             {
                currentEnemyState = value;
                ChangeCurrentAnimaton();
             }
            }
    }

// Start is called before the first frame update
    protected virtual void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anmin = GetComponent<Animator>();
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerMovement.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
        if (isRecoiling)
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
        else
        {
            UpdateEnemyStates();
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !PlayerMovement.Instance.pState.invincible)
        {
            Attack();
            if(PlayerMovement.Instance.pState.alive) 
            {
                PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }
    public virtual void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {   GameObject _orangeBlood = Instantiate(orangerBlood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 2.0f);
            rb.velocity = -_hitForce * recoilFactor * _hitDirection;
        }
    }
    public void MakeLoot()
    {
        if (thisLoot != null)
        {
            Powerup current = thisLoot.LootPowerup();
            if (current != null)
            {
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }
    public virtual void Death(float _destroyTime)
    {
        MakeLoot();
        Destroy(gameObject, _destroyTime);

    }
    protected virtual void UpdateEnemyStates() 
    {
    }
    protected void ChangeState(EnemyStates _newState)
    {
        getCurrentEnemyState = _newState;
    }
    protected virtual void ChangeCurrentAnimaton()
    {
 
    }
    protected virtual void Attack()
    {
        PlayerMovement.Instance.TakeDamage(damage);
    }
}
