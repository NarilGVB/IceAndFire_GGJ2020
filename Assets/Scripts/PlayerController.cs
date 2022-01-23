using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    public GameObject dashPlayer;
    Vector2 movement;
    private Vector2 moveDirection;
    private Animator playerAnimator;
    bool canBeDamaged = true;

    public Image cooldownTimer;
    bool cooldownUp = false;

    GameManager gameManager;

    public SpriteRenderer currentSprite;
    float oldPosition;
    public Sprite baseSprite;
    public Sprite fireSprite;
    public Sprite iceSprite;
    public Sprite iceDeathSprite;
    public Sprite fireDeathSprite;
    public Sprite fireDashlvl1;
    public Sprite fireDashlvl2;
    public Sprite fireDashlvl3;

    [Header ("Ice Wave")]
    public float sphereRadius;
    public float iceWaveCooldown;
    public float rootDuration;

    [Header("Fire Dash")]
    //public float dashSpeed;
    bool isDashing;
    public float fireDashCooldown;
    public float fireDashDuration;
    public float dashMultiplier;
    public float dashRadius;

    private Rigidbody2D _rigidbody;

    //public static bool alive;
    
    // Start is called before the first frame update
    void Start()
    {
        //GameManager._isDead = false;
        _rigidbody = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsDead)
        {
            //_rigidbody.enabled = false;
            //_rigidbody.MovePosition(_rigidbody.position);
            movement = Vector2.zero;
            return;

        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (gameManager.Temperature < 10 && gameManager.Temperature > -10)
            currentSprite.sprite = baseSprite;
        else if (gameManager.Temperature <= -10)
            currentSprite.sprite = iceSprite;
        else if (gameManager.Temperature >= 10 && !isDashing)
            currentSprite.sprite = fireSprite;

        if (movement.x > 0)
        {
            currentSprite.flipX = false;
        }

        else if (movement.x < 0)
        {
            currentSprite.flipX = true;
        }

        if (Input.GetButtonDown("Jump") && cooldownUp == false)
        {
            //StartCoroutine(IceWave());
            if (gameManager.Temperature <= -10)
            {
                AudioManager.instance.PlayIceAbility();
                IceWave();
            }
            else if (gameManager.Temperature >= 10)
            {
                AudioManager.instance.PlayFireAbility();
                StartCoroutine(FireDash());
            }
            //Debug.Log("Movement = "+movement);
        }

        if (cooldownUp == false)
            cooldownTimer.fillAmount = 0;
        else
            cooldownTimer.fillAmount = 1;
    }
    void FixedUpdate()
    {
        moveDirection = new Vector2(movement.x, movement.y).normalized;
        _rigidbody.MovePosition(_rigidbody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void IceWave()
    {

        if (gameManager.Temperature <= -80)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, sphereRadius, transform.forward * 0.1f);

            if (hits.Length > 0)
                AudioManager.instance.PlayEnemyDies();
            
            playerAnimator.SetTrigger("IceWave");
            StartCoroutine(Cooldown(iceWaveCooldown));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.layer == 6)
                {
                    EnemyController enemyController = hit.transform.GetComponent<EnemyController>();
                    enemyController.instantiateDeathAnimation();
                    enemyController.DestroyEnemy();
                    gameManager.points += 2;
                    AudioManager.instance.PlayGetPoints();
                }

            }
        }
        else if (gameManager.Temperature <= -45)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, sphereRadius, transform.forward * 0.1f);

            if (hits.Length > 0)
                AudioManager.instance.PlayIceTouchEnemy();

            playerAnimator.SetTrigger("IceWave");
            StartCoroutine(Cooldown(iceWaveCooldown));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.layer == 6 && hit.transform != null)
                {
                    StartCoroutine(PushEnemy(hit));
                }
            }
        }
        else if (gameManager.Temperature <= -10)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(transform.position, sphereRadius, transform.forward * 0.1f);

            if (hits.Length > 0)
                AudioManager.instance.PlayIceTouchEnemy();

            playerAnimator.SetTrigger("IceWave");
            StartCoroutine(Cooldown(iceWaveCooldown));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.layer == 6)
                {
                    StartCoroutine(FreezeEnemy(hit));
                }
            }
        }
        

    }

    IEnumerator FreezeEnemy(RaycastHit2D hit)
    {
        //if (hit.transform != null)
            

        EnemyController enemyController = hit.transform.GetComponent<EnemyController>();
        hit.transform.GetChild(0).gameObject.SetActive(true);
        enemyController.canMove = false;

        Animator anim = hit.transform.GetComponent<Animator>();

        float prevSpeed = anim.speed;
        anim.speed = 0;

        

        yield return new WaitForSeconds(rootDuration);

        if (hit.transform != null)
        {
            hit.transform.GetChild(0).gameObject.SetActive(false);
            anim.speed = prevSpeed;
        }
        enemyController.canMove = true;
    }
    



    IEnumerator PushEnemy(RaycastHit2D hit)
    {

        Vector2 dir = (hit.transform.position - transform.position).normalized;
        Vector2 targetPos = (Vector2)hit.transform.position + dir * sphereRadius*2/3;

        for (Vector2 hitPosition = hit.transform.position; Vector2.Distance(hitPosition, targetPos) > 0.1f; hitPosition += dir*0.05f)
        {
            if (hit.transform == null)
                break;
            
            if (hitPosition.x > GameLimits.limitRoom.x || hitPosition.x < -GameLimits.limitRoom.x)
            {
                hitPosition.x = hit.transform.position.x;
                targetPos.x = hitPosition.x;
            }

            if (hitPosition.y > GameLimits.limitRoom.y || hitPosition.y < -GameLimits.limitRoom.y)
            {
                hitPosition.y = hit.transform.position.y;
                targetPos.y = hitPosition.y;
            }

            hit.transform.position = hitPosition;
            yield return null;

        }

        if (hit.transform != null)
            StartCoroutine(FreezeEnemy(hit));
                
    }

    bool invulnerable = false;

    IEnumerator FireDash()
    {
        isDashing = true;
        if (gameManager.Temperature > 80)
        {
            StartCoroutine(Cooldown(fireDashCooldown));
            Vector2 dashMoveDirection = new Vector2(movement.x, movement.y).normalized;

            float baseSpeed = speed;
            Vector2 baseMoveDirection = moveDirection;

            dashPlayer.SetActive(true);
            currentSprite.sprite = fireDashlvl3;
            invulnerable = true;
            speed = speed * dashMultiplier;
            moveDirection = dashMoveDirection;

            yield return new WaitForSeconds(fireDashDuration);
            speed = baseSpeed;
            moveDirection = baseMoveDirection;

            yield return new WaitForSeconds(0.3f);
            dashPlayer.SetActive(false);
            currentSprite.sprite = baseSprite;
            invulnerable = false;

        }

        else if (gameManager.Temperature >= 45)
        {
            StartCoroutine(Cooldown(fireDashCooldown));
            Vector2 dashMoveDirection = new Vector2(movement.x, movement.y).normalized;

            float baseSpeed = speed;
            Vector2 baseMoveDirection = moveDirection;

            currentSprite.sprite = fireDashlvl2;
            canBeDamaged = false;
            speed = speed * dashMultiplier;
            moveDirection = dashMoveDirection;

            yield return new WaitForSeconds(fireDashDuration);
            speed = baseSpeed;
            moveDirection = baseMoveDirection;

            yield return new WaitForSeconds(0.3f);
            currentSprite.sprite = baseSprite;
            canBeDamaged = true;
        }

        else if (gameManager.Temperature >= 10)
        {
            StartCoroutine(Cooldown(fireDashCooldown));
            Vector2 dashMoveDirection = new Vector2(movement.x, movement.y).normalized;

            float baseSpeed = speed;
            Vector2 baseMoveDirection = moveDirection;

            Debug.Log("Speed " + speed);
            currentSprite.sprite = fireDashlvl1;
            speed = speed * dashMultiplier;
            moveDirection = dashMoveDirection;

            yield return new WaitForSeconds(fireDashDuration);

            speed = baseSpeed;
            moveDirection = baseMoveDirection;

            yield return new WaitForSeconds(0.3f);
            currentSprite.sprite = baseSprite;
        }
        isDashing = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
        Gizmos.DrawWireSphere(transform.position, dashRadius);
    }

    IEnumerator Cooldown(float seconds)
    {
        float timer = 0;
        cooldownUp = true;

        while(timer < seconds)
        {
            cooldownTimer.fillAmount = timer / seconds;
            yield return null;
            timer += Time.deltaTime;
        }

        cooldownUp = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBeDamaged || GameManager.IsDead)
            return;
        EnemyController otherController = other.gameObject.GetComponent<EnemyController>();
        if (otherController != null)
        {
            if (!invulnerable)
                GameManager.Instance.ChangeTemperature(otherController.temperature);
            otherController.instantiateDeathAnimation();
            otherController.DestroyEnemy();
            gameManager.points += 2;
            AudioManager.instance.PlayGetPoints();
            if (isDashing)
                AudioManager.instance.PlayFireTouchEnemy();
        }
    }
}
