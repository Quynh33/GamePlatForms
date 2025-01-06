using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Boss : Enemy
{
    public static Boss Instance;
    [SerializeField] private GameObject slashEffect;
    public Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    public Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    public float attackRange;
    public float attackTimer;
    [HideInInspector] public bool facingRight;

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    int hitCounter;
    bool stunned, canStun;
    bool alive;
    [HideInInspector] public float runSpeed;
    protected override void OnCollisionStay2D(Collision2D _other)
    {

    }
    #region attacking
    #region variables
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool damagedPlayer = false;
    [HideInInspector] public bool parrying;
    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject pillar;
    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireBall;
    public GameObject barrageFireBall1;
    [HideInInspector] public bool outbreakAttack;
    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionTarget;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        anmin = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.Boss_Stage1);
        alive = true;
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(health <= 0 && alive )
        {
            Death(0);
        }
        if(!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
        if (stunned)
        {
            rb.velocity = Vector2.zero;
        }
    }
    public void Flip()
    {
        if (PlayerMovement.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
    }
    protected override void UpdateEnemyStates()
    {


        if (PlayerMovement.Instance != null)
        {
            switch (getCurrentEnemyState)
            {
                case EnemyStates.Boss_Stage1:
                    canStun = true;
                    attackTimer = 6;
                    runSpeed = speed;
                    break;
                case EnemyStates.Boss_Stage2:
                    attackTimer = 5;
                    canStun = true;
                    break;
                case EnemyStates.Boss_Stage3:
                    attackTimer = 8;
                    canStun = false;
                    break;
                case EnemyStates.Boss_Stage4:
                    canStun = false;
                    attackTimer = 10;
                    runSpeed = speed / 2;
                    break;

            }
        }
    }

    #endregion
    #region Control
    public void AttackHandler()
    {
        if(currentEnemyState == EnemyStates.Boss_Stage1)
        {
            if(Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                DiveAttackJump();

            }
        }
       if (currentEnemyState == EnemyStates.Boss_Stage2)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                    BarrageBendDown();
                

            }


        }
        if (currentEnemyState == EnemyStates.Boss_Stage3)
        {


           
             OutBreakBendDown();

          
        }
        if (currentEnemyState == EnemyStates.Boss_Stage4)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(Slash());
            }
            else
            {
                BounceAttack();

            }


        }
    }
    public void ResetAllAttack()
    {
        attacking = false;
        StopCoroutine(TripleSlash());
        StopCoroutine(Lunge());
        StopCoroutine(Parry());
        StopCoroutine(Slash());

        diveAttack = false;
        barrageAttack = false;
        outbreakAttack = false;
        bounceAttack = false;
    }
    #endregion
    #region Stage 2
    void DiveAttackJump()
    {
        attacking = true;
        moveToPosition = new Vector2(PlayerMovement.Instance.transform.position.x, rb.position.y + 5f);
        diveAttack = true;
        anmin.SetBool("Jump", true);


    }

    
    public void Dive()
    {
        anmin.SetBool("Dive",true);
        anmin.SetBool("Jump", false);
    }
    public void DivingPillar()
    {
        Debug.Log("DivingPillar has been called");
        Vector2 _ImpactPoint = groundCheckPoint.position;
        float _spawnDistance = 5;
        for(int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnRight = _ImpactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnLeft = _ImpactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnRight, Quaternion.identity);  // Không quay
            Instantiate(pillar, _pillarSpawnLeft, Quaternion.identity);  // Không quay

            _spawnDistance += 8;
        }
        ResetAllAttack();
    }

    void BarrageBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        barrageAttack = true;
        Debug.Log("BarrageBendDown called, barrageAttack: " + barrageAttack); // Kiểm tra giá trị
        anmin.SetTrigger("BendDown");
    }
    public IEnumerator Barrage()
    {
        rb.velocity = Vector2.zero;
        float _currentAngle = 10f;

        for(int i = 0;i < 5;i++)
        {
            GameObject _projectile = Instantiate(barrageFireBall, transform.position, Quaternion.Euler(0, 0, _currentAngle));
            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
            }
            _currentAngle += 5;
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.1f);
        anmin.SetBool("Cast", false);
        ResetAllAttack() ;
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<PlayerMovement>() != null &&(diveAttack || bounceAttack))
        {
            _other.GetComponent<PlayerMovement>().TakeDamage(damage * 2);
            PlayerMovement.Instance.pState.recoilingX = true;
        }
    }
    #endregion
    #region Stage 1
    IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anmin.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.3f);

        anmin.ResetTrigger("Slash");

        anmin.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.5f);
        anmin.ResetTrigger("Slash");

        anmin.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        anmin.ResetTrigger("Slash");
        damagedPlayer = false;
        ResetAllAttack();
    }
    void SlashAngle()
    {
        Vector2 playerDistance = PlayerMovement.Instance.transform.position - transform.position;

        // Tấn công ngang (Side attack)
        if (Mathf.Abs(playerDistance.x) > Mathf.Abs(playerDistance.y))
        {
            Instantiate(slashEffect, SideAttackTransform.position, Quaternion.identity);
        }
        else if (playerDistance.y > 0)
        {
            // Tấn công lên (Up attack)
            SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        }
        else if (playerDistance.y < 0)
        {
            // Tấn công xuống (Down attack)
            SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
        }
    }

    #endregion
    #endregion
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        GameObject instantiatedEffect = Instantiate(_slashEffect, _attackTransform.position, Quaternion.identity);
        instantiatedEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        instantiatedEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
    IEnumerator Lunge()
    {
        Flip();
        attacking = true;
        anmin.SetBool("Lunge", true);
        yield return new WaitForSeconds(1f);
        anmin.SetBool("Lunge", false);

        ResetAllAttack();
    }
    IEnumerator Parry()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anmin.SetBool("Parry", true);
        yield return new WaitForSeconds(0.8f);
        anmin.SetBool("Parry", false);
        parrying = false;
        ResetAllAttack();
    }
    IEnumerator Slash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anmin.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        anmin.ResetTrigger("Slash");
        ResetAllAttack();
    }
    #region Stage 3
    void OutBreakBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        moveToPosition = new Vector2(transform.position.x, transform.position.y + 5);
        outbreakAttack = true;
        anmin.SetTrigger("BendDown");

    }
    public IEnumerator OutBreak()
    {
        yield return new WaitForSeconds(1f);
        anmin.SetBool("Cast", true);

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 5; i++)
        {
            GameObject projectile1 = Instantiate(barrageFireBall1, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130)));
            GameObject projectile2 = Instantiate(barrageFireBall1, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70)));
            GameObject projectile3 = Instantiate(barrageFireBall1, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280)));

            // Thêm lực cho từng đạn theo hướng quay của nó
            projectile1.GetComponent<Rigidbody2D>().AddForce(projectile1.transform.right * 25f, ForceMode2D.Impulse);
            projectile2.GetComponent<Rigidbody2D>().AddForce(projectile2.transform.right * 25f, ForceMode2D.Impulse);
            projectile3.GetComponent<Rigidbody2D>().AddForce(projectile3.transform.right * 25f, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(rb.velocity.x, -10);
        yield return new WaitForSeconds(0.1f);
        anmin.SetBool("Cast", false);
        ResetAllAttack();
    }
    void BounceAttack()
    {
        attacking = true;
        BounceBendDown();
    }
    public void BounceBendDown()
    {   rb.velocity = Vector2.zero;
        Vector3 _directionToTarrget = new Vector2(PlayerMovement.Instance.transform.position.x, rb.position.y * 10);
        bounceAttack = true;
        anmin.SetTrigger("BendDown");

    }
    public void CalculateTargetAngle()
    {
        Vector3 _directionToTarget = (PlayerMovement.Instance.transform.position - transform.position).normalized;
        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionTarget = _angleOfTarget;
    }
    #endregion
    public override void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (!stunned)
        {
            if (!parrying)
            {
                if (canStun)
                {
                    hitCounter++;
                    if (hitCounter >= 3)
                    {
                        ResetAllAttack();
                        StartCoroutine(Stunned());
                    }
                }
                base.EnemyGetHit(_damageDone, _hitDirection, _hitForce);
                if (currentEnemyState != EnemyStates.Boss_Stage4)
                {
                    StartCoroutine(Parry());
                    ResetAllAttack();
                }

            }

            else
            {
                StopCoroutine(Parry());
                parrying = false;
                ResetAllAttack();
                StartCoroutine(Slash());
            }
        }
        else
        {
            StopCoroutine(Stunned());
            anmin.SetBool("Stunned", false);
            stunned = false;
        }
            #region health to state
            Debug.Log($"Boss Health: {health}"); // Log giá trị máu hiện tại

            if (health > 20)
            {
                ChangeState(EnemyStates.Boss_Stage1);
                Debug.Log("Changed to Boss_Stage1");
            }
            else if (health > 15)
            {
                ChangeState(EnemyStates.Boss_Stage2);
                Debug.Log("Changed to Boss_Stage2");
            }
            else if (health > 10)
            {
                ChangeState(EnemyStates.Boss_Stage3);
                Debug.Log("Changed to Boss_Stage3");
            }
            else if (health > 5)
            {
                ChangeState(EnemyStates.Boss_Stage4);
                Debug.Log("Changed to Boss_Stage4");
            }
            else if (health <= 5)
            {
                ChangeState(EnemyStates.Boss_Stage4);
                Debug.Log("Boss Critical - Changed to Boss_Stage4");
            }

            if (health <= 0)
            {
                Debug.Log("Boss Died!");
                Death(0);
            }
        #endregion
        
    }
    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        anmin.SetBool("Stunned", true);

        yield return new WaitForSeconds(3f);
        anmin.SetBool("Stunned", false);
        stunned = false;
    }
    public override void Death (float _detroyTimer)
    {
        ResetAllAttack();
        alive = false;
        rb.velocity = new Vector2(rb.velocity.x, -25);
        anmin.SetTrigger("Die");
        MakeLoot();

    }
    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}