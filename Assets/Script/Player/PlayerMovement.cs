using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    Animator anim;
    public static PlayerMovement Instance;
    private Rigidbody2D rb;
    public float speed;
    private float xAxis, yAxis;
    private int jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    private int airJumpCounter = 0;
    [HideInInspector] public PlayerStateList pState;
    private float gravity;
    private bool canDash;
    private bool dashed;
    bool attack = false;
    float timeBetweenAttack, timeSinceAtttack;
    int stepsXRecoiled, stepsYRecoiled;
    private SpriteRenderer sr;

    [Header("Horizontal Movement Setting")]
    [SerializeField] private int jumpBufferFrames;
    [SerializeField] private float coyoteTime;
    [SerializeField] private int maxAirJump;
    [Space(3)]

    [Header("Ground Check Setting")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [Space(3)]

    [Header("Dash setting")]
    [SerializeField] GameObject dashEffect;
    [Space(3)]

    [Header("Attack")]
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(3)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    [Space(3)]

    [Header("Health")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(3)]

    [Header("ManaSetting")]
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [SerializeField] Image manaStorage;
    [Space(3)]

    [Header("Spell Setting")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;

    [SerializeField] GameObject sideSpellFireBall;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireBall;




    private void Awake()
    {
       if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        health = maxHealth;
    }
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }
    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariable();
        if (pState.dashing) return;
        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        Recoil();
        RestoreTimeScale();
        FlashWhiteInvincible();
        Heal();
        CastSpell();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null && pState.casting)
        {
            other.GetComponent<Enemy>().EnemyHit(spellDamage, (other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    void GetInputs()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        attack = Input.GetMouseButtonDown(0);
    }
    void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }else if(xAxis > 0) 
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }
    private void Move()
    {
        rb.velocity = new Vector2(speed* xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
        anim.SetBool("Falling", rb.velocity.y < 0 && !Grounded());
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if(Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        Vector3 spawnPosition = transform.position;
        Instantiate(dashEffect, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    void Attack()
    {
        timeSinceAtttack += Time.deltaTime;
        if (attack && timeSinceAtttack >= timeBetweenAttack)
        {
            timeSinceAtttack = 0;
            anim.SetTrigger("Attacking");

            // Chỉ khi thực sự tấn công mới gọi phương thức Hit và tạo hiệu ứng
            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSteps);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea,ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrenght)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if(objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage,(transform.position - objectsToHit[i].transform.position).normalized,_recoilStrenght );
                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        Debug.Log("Taking damage: " + _damage);
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt,transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }
    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }

        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    void FlashWhiteInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 0.1f)): Color.white;
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if(onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
    float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }
    }
    void CastSpell()
    {
        if(Input.GetButtonDown("CastSpell") && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            Debug.Log("CastSpell called");
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
        if (Grounded()) 
        {
            downSpellFireBall.SetActive(false);
        }
        if (downSpellFireBall.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }
    IEnumerator CastCoroutine()
    {
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            anim.SetBool("Casting", true);
            GameObject fireball = Instantiate(sideSpellFireBall, SideAttackTransform.position, Quaternion.identity);
            if (pState.lookingRight)
            {
                fireball.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                fireball.transform.eulerAngles = new Vector2(fireball.transform.eulerAngles.x,180 );
            }
            pState.recoilingX = true;
        }
        else if (yAxis > 0)
        {
            anim.SetBool("Casting", true);
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }
        else if(yAxis <0 && !Grounded())
        {
            anim.SetTrigger("CastingDown");
            downSpellFireBall.SetActive(true);
        }
        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("CastingDown", false);
        anim.SetBool("Casting", false);
        pState.casting = false;
    }
    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Mana >0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;  
            }
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;  
        }
    }
    public bool Grounded()
    {
        if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX,0,0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }
        if (!Grounded() && airJumpCounter < maxAirJump && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        if (Input.GetButtonDown("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        anim.SetBool("Jumping", !Grounded());
    }
    void UpdateJumpVariable()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
