using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    Animator anim;
    public static PlayerMovement Instance;
    public Rigidbody2D rb;
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
    public VectorValue playerStorage;
    public VectorValue StartPoint;
    private AudioSource audioSource;
    private bool landingSoundPlayed;
    private bool isRunningSoundPlaying = false;

    [Header("Horizontal Movement Setting")]
    [SerializeField] private int jumpBufferFrames;
    [SerializeField] private float coyoteTime;
    [SerializeField] private int maxAirJumps;
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
    public float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public Image manaStorage;
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

    [Header("Audio")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip spellCastSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip hurtSound;
    private bool openInventory = false;
    public GameObject doubleJumpEffect; // Tham chiếu tới prefab hiệu ứng
    public Transform effectSpawnPoint; // Vị trí spawn hiệu ứng (nếu khác vị trí nhân vật)




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

        // Đảm bảo rằng Rigidbody2D được gán
        rb = GetComponent<Rigidbody2D>();

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
        audioSource = GetComponent<AudioSource>();
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
        if (GameManager.Instance.gameIsPaused) return;
        if (pState.cutscene) return;
        if (pState.alive)
        {
            GetInputs();
        }
        UpdateJumpVariable();
        RestoreTimeScale();
        if (pState.dashing) return;
        if (pState.alive)
        {
            Flip();
            Move();
            Jump();
            StartDash();
            Attack();
            Recoil();
            Heal();
            CastSpell();
            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleInventory(); // Chuyển trạng thái inventory
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() != null && pState.casting)
        {
            other.GetComponent<Enemy>().EnemyGetHit(spellDamage, (other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    private void FixedUpdate()
    {
        if (pState.dashing || pState.healing || pState.cutscene) return;
        Recoil();
    }
    void GetInputs()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        attack = Input.GetMouseButtonDown(1);

    }
    void ToggleInventory()
    {
        // Đổi trạng thái inventory
        openInventory = !openInventory;

        // Bật hoặc tắt UI inventory
        UIManager.Instance.inventory.SetActive(openInventory);
    }
    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }
    private void Move()
    {
        rb.velocity = new Vector2(speed * xAxis, rb.velocity.y);
        bool isWalking = rb.velocity.x != 0 && Grounded();
        anim.SetBool("Falling", rb.velocity.y < 0 && !Grounded());
        anim.SetBool("Walking", isWalking);
        // Handle walking sound based on grounded and walking state
        if (isWalking && anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            if (!isRunningSoundPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = landingSound; // Replace with walking sound clip if different
                audioSource.Play();
                isRunningSoundPlaying = true;
            }
        }
        else
        {
            // Stop the walking sound immediately if the player leaves the ground or stops moving
            if (isRunningSoundPlaying)
            {
                audioSource.Stop();
                isRunningSoundPlaying = false;
            }
        }
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
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
            audioSource.PlayOneShot(attackSound);
            // Chỉ khi thực sự tấn công mới gọi phương thức Hit và tạo hiệu ứng
            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSteps);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrenght)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyGetHit(damage, _recoilDir, _recoilStrenght);
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
        if (pState.alive)
        {
            health -= Mathf.RoundToInt(_damage); // Giảm máu thực tế
            audioSource.PlayOneShot(hurtSound); // Phát âm thanh bị thương
        }

        if (health <= 0) // Kiểm tra trạng thái chết
        {
            health = 0;
            StartCoroutine(Death()); // Gọi hành động chết
        }
        else
        {
            StartCoroutine(StopTakingDamage()); // Bắt đầu hành động sau khi nhận sát thương
        }
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


    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (onHealthChangedCallback != null)
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
        if (Input.GetButtonDown("CastSpell") && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
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
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            GameObject fireball = Instantiate(sideSpellFireBall, SideAttackTransform.position, Quaternion.identity);
            if (pState.lookingRight)
            {
                fireball.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                fireball.transform.eulerAngles = new Vector2(fireball.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
        }
        else if (yAxis > 0)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }
        else if (yAxis < 0 && !Grounded())
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetTrigger("CastingDown");
            downSpellFireBall.SetActive(true);
        }
        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("CastingDown", false);
        anim.SetBool("Casting", false);
        pState.casting = false;
    }
    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
    }
    public void Respawn()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            anim.Play("Idle");
        }
    }
    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
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
    void Jump()
    {
        if (!pState.jumping)
        {
            // Nhảy lần đầu (khi trên mặt đất hoặc trong thời gian coyote)
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                audioSource.PlayOneShot(jumpSound); // Phát âm thanh khi nhảy
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Tốc độ nhảy
                pState.jumping = true;
                landingSoundPlayed = false; // Đặt lại biến landingSound
                Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
                Instantiate(doubleJumpEffect, spawnPosition, Quaternion.identity);
            }
            // Nhảy giữa không trung (double jump)
            else if ( airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                Debug.Log("DoubleJump");
                audioSource.PlayOneShot(jumpSound); 
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
                airJumpCounter++; 
                pState.jumping = true;
                Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
                Instantiate(doubleJumpEffect, spawnPosition, Quaternion.identity);
            }
        }

        // Khi nhả nút nhảy, giảm tốc độ nhảy (nhảy thấp hơn nếu nhả nút sớm)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        anim.SetBool("Jumping", !Grounded()); // Cập nhật animation
    }

    void UpdateJumpVariable()
    {
        if (Grounded())
        {
            // Khi chạm đất, reset các biến liên quan đến nhảy
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound); // Phát âm thanh hạ cánh
                landingSoundPlayed = true; // Đánh dấu đã phát âm thanh
            }

            pState.jumping = false; // Đặt trạng thái nhảy về false
            coyoteTimeCounter = coyoteTime; // Đặt lại coyote time
            airJumpCounter = 0; // Đặt lại số lần nhảy giữa không trung
        }
        else
        {
            // Nếu không ở trên mặt đất, giảm dần coyote time
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false; // Sẵn sàng phát lại âm thanh khi chạm đất
        }

        // Đếm buffer nhảy khi người chơi nhấn phím nhảy
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames; // Đặt lại bộ đếm jump buffer
        }
        else if (jumpBufferCounter > 0)
        {
            jumpBufferCounter--; // Giảm bộ đếm buffer
        }
    }
}

